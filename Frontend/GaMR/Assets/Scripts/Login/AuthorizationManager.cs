using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AuthorizationManager : Singleton<AuthorizationManager>
{

    [SerializeField]
    private string clientId = "c5389d2a-0074-4c45-9e9e-2eb1571e99bd";
    private string clientSecret;
    [SerializeField]
    private string debugToken;
    private string accessToken;
    private string authorizationCode;

    const string learningLayersAuthorizationEndpoint = "https://api.learning-layers.eu/o/oauth2/authorize";
    const string learningLayersTokenEndpoint = "https://api.learning-layers.eu/o/oauth2/token";
    const string learningLayersUserInfoEndpoint = "https://api.learning-layers.eu/o/oauth2/userinfo";

    const string scopes = "openid%20profile%20email";

    const string gamrRedirectURI = "gamr://";

    public string AccessToken { get { return accessToken; } }

    private void Start()
    {
        // skip the login by using the debug token
        if (Application.isEditor)
        {
            if (accessToken == null || accessToken == "")
            {
                accessToken = debugToken;
                AddAccessTokenToHeader();
                RestManager.Instance.GET(learningLayersUserInfoEndpoint + "?access_token=" + accessToken, GetUserInfoForDebugToken);
            }
        }
        else // else: fetch the client secret
        {
            TextAsset secretAsset = (TextAsset)Resources.Load("values/client_secret");
            clientSecret = secretAsset.text;
            Debug.Log("Read in client secret: " + clientSecret);
        }
    }

    private void GetUserInfoForDebugToken(UnityWebRequest req)
    {
        if (req.responseCode == 200)
        {
            Debug.Log("Received user info for debug token successfully.");
            string json = req.downloadHandler.text;
            Debug.Log(json);
            UserInfo info = JsonUtility.FromJson<UserInfo>(json);
            InformationManager.Instance.UserInfo = info;
        }
        else if (req.responseCode == 401)
        {
            Debug.LogError("Unauthorized: access token is wrong");
        }
    }

    public void Login()
    {
        if (Application.isEditor)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
            return;
        }
        string url = learningLayersAuthorizationEndpoint + "?response_type=code&scope=" + scopes + "&client_id=" + clientId + "&redirect_uri=" + gamrRedirectURI;
        Debug.Log("Open login screen with url: " + url);
        Application.OpenURL(url);
    }

    public void Logout()
    {
        accessToken = "";
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    private void StartedByProtocol(Uri uri)
    {
        Debug.Log("Started by protocol");
        if (uri.Fragment != null)
        {
            char[] splitters = { '?', '&' };
            string[] arguments = uri.AbsoluteUri.Split(splitters);
            foreach (string argument in arguments)
            {
                if (argument.StartsWith("code="))
                {
                    authorizationCode = argument.Replace("code=", "");
                    Debug.Log("authorizationCode: " + authorizationCode);
                    // now exchange authorization code for access token
                    RestManager.Instance.POST(learningLayersTokenEndpoint + "?code=" + authorizationCode + "&client_id=" + clientId +
                        "&client_secret=" + clientSecret + "&redirect_uri=" + gamrRedirectURI + "&grant_type=authorization_code", (req) =>
                        {
                            Debug.Log("Requested URL: " + req.url);
                            if (req.responseCode != 200) {
                                Debug.Log("An error occured: " + req.error + " (Code: " + req.responseCode + ")");
                                return;
                            }
                                
                            string json = req.downloadHandler.text;
                            Debug.Log("Token json: " + json);
                            AuthorizationFlowAnswer answer = JsonUtility.FromJson<AuthorizationFlowAnswer>(json);
                            if (!string.IsNullOrEmpty(answer.error))
                            {
                                MessageBox.Show(answer.error_description, MessageBoxType.ERROR);
                            }
                            else
                            {
                                // extract access token and check it
                                accessToken = answer.access_token;

                                Debug.Log("The access token is " + accessToken);

                                AddAccessTokenToHeader();
                                CheckAccessToken();
                            }
                        }
                        );
                    break;
                }
            }

        } else {
            Debug.Log("Uri fragment empty");
        }
    }

    private void AddAccessTokenToHeader()
    {
        if (RestManager.Instance.StandardHeader.ContainsKey("access_token"))
        {
            RestManager.Instance.StandardHeader["access_token"] = accessToken;
        }
        else
        {
            RestManager.Instance.StandardHeader.Add("access_token", accessToken);
        }
    }

    private void CheckAccessToken()
    {
        RestManager.Instance.GET(learningLayersUserInfoEndpoint + "?access_token=" + accessToken, OnLogin);
    }

    private void OnLogin(UnityWebRequest result)
    {
        if (result.responseCode == 200)
        {
            string json = result.downloadHandler.text;
            Debug.Log(json);
            UserInfo info = JsonUtility.FromJson<UserInfo>(json);

            InformationManager.Instance.UserInfo = info;
            GamificationFramework.Instance.ValidateLogin(LoginValidated);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Error while retrieving the user data. Login failed"), MessageBoxType.ERROR);
        }
    }

    private void LoginValidated(UnityWebRequest req)
    {
        Debug.Log("Requested Gamification-Framework: " + req.url + "(code: " + req.responseCode + ")");

        if (req.responseCode == 200)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        }
        else if (req.responseCode == 401)
        {
            Debug.Log("Error Message: " + req.error);
            MessageBox.Show(LocalizationManager.Instance.ResolveString("The login could not be validated"), MessageBoxType.ERROR);
        }
        else
        {
            Debug.Log("Error Message: " + req.error);
            MessageBox.Show(LocalizationManager.Instance.ResolveString("An error concerning the user data occured. The login failed.\nCode: ") + req.responseCode + "\n" + req.downloadHandler.text, MessageBoxType.ERROR);
        }
    }
}
