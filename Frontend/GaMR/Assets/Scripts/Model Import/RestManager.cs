﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the requests to the backend REST-service
/// </summary>
public class RestManager : Singleton<RestManager>
{

    private Dictionary<string, string> standardHeader;

    public Dictionary<string, string> StandardHeader
    {
        get
        {
            if (standardHeader == null)
            {
                standardHeader = new Dictionary<string, string>();
            }

            return standardHeader;
        }
        set { standardHeader = value; }
    }

    private void Start()
    {
        //ContentType = "application/json";
    }

    public string ContentType
    {
        get
        {
            if (StandardHeader.ContainsKey("Content-Type"))
            {
                return StandardHeader["Content-Type"];
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (StandardHeader.ContainsKey("Content-Type"))
            {
                StandardHeader["Content-Type"] = value;
            }
            else
            {
                StandardHeader.Add("Content-Type", value);
            }
        }
    }

    public void GET(string url, System.Action<UnityWebRequest> callback)
    {
        object[] passOnArgs = { callback };
        GET(url, ReduceCallback, passOnArgs);
    }

    /// <summary>
    /// creates a coroutine which will query the url and return the result to the callback function
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
    /// <param name="passOnArgs">Further arguments which are passed on to the callback-method</param>
    public void GET(string url, System.Action<UnityWebRequest, object[]> callback, object[] passOnArgs)
    {
        StartCoroutine(CallWWW(url, "GET", callback, passOnArgs));
    }

    /// <summary>
    /// reduces the callback from an Action(UnityWebRequest, object[]) to an Action(UnityWebRequest)
    /// </summary>
    /// <param name="req">The finished web request</param>
    /// <param name="passOnArgs">Contains the next callback</param>
    private void ReduceCallback(UnityWebRequest req, object[] passOnArgs)
    {
        if (passOnArgs != null && passOnArgs.Length > 0)
        {
            try {
                ((Action<UnityWebRequest>)passOnArgs[0])(req);
            } catch (Exception e) {
                Debug.Log("Failed reducing callback: " + e.Message);
            }
        }
    }

    public void DELETE(string url, Action<UnityWebRequest, object[]> callback, object[] passOnArgs)
    {
        StartCoroutine(CallWWW(url, "DELETE", callback, passOnArgs));
    }

    public void DELETE(string url, Action<UnityWebRequest> callback)
    {
        object[] passOnArgs = { callback };
        DELETE(url, ReduceCallback, passOnArgs);
    }

    /// <summary>
    /// Creates a coroutine which will post the specified data to the url
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="json">The body of the post</param>
    public void POST(string url, string json)
    {
        StartCoroutine(UploadWWW(url, "POST", json, null));
    }

    public void POST(string url, string json, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", json, callback));
    }

    public void POST(string url, List<IMultipartFormSection> formData, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", formData, callback));
    }

    public void POST(string url, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", "", callback));
    }

    public void PUT(string url, string json)
    {
        StartCoroutine(UploadWWW(url, "PUT", json, null));
    }

    public void PUT(string url, string json, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", json, callback));
    }

    public void PUT(string url, List<IMultipartFormSection> formData, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", formData, callback));
    }

    public void PUT(string url, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", "", callback));
    }

    /// <summary>
    /// Called as a coroutine and posts the data to the url.
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="callback">Called when the operation finished</param>
    /// <returns></returns>
    private IEnumerator UploadWWW(string url, string requestType, string body, System.Action<UnityWebRequest> callback)
    {
        WaitCursor.Show();
        UnityWebRequest req = new UnityWebRequest(url, requestType);
        req.chunkedTransfer = false;
        if (body != "")
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(body));
            req.uploadHandler.contentType = "application/json";
        }
        req.downloadHandler = new DownloadHandlerBuffer();
        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }
        yield return req.Send();

        WaitCursor.Hide();
        if (callback != null)
        {
            callback(req);
        }
    }

    private IEnumerator UploadWWW(string url, string requestType, List<IMultipartFormSection> body, System.Action<UnityWebRequest> callback)
    {
        WaitCursor.Show();
        byte[] byteBoundary = UnityWebRequest.GenerateBoundary();
        byte[] generated = UnityWebRequest.SerializeFormSections(body, byteBoundary);

        List<byte> generatedList = new List<byte>();

        for (int i = 0; i < generated.Length; i++)
        {
            generatedList.Add(generated[i]);
        }

        // add end-boundary
        byte[] endBoundary = Encoding.ASCII.GetBytes("\r\n--" + Encoding.ASCII.GetString(byteBoundary) + "--");
        for (int i = 0; i < endBoundary.Length; i++)
        {
            generatedList.Add(endBoundary[i]);
        }


        UnityWebRequest req = new UnityWebRequest(url);
        req.method = requestType;

        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }

        //string boundary = Encoding.ASCII.GetString(UnityWebRequest.GenerateBoundary());

        //string strEndBoundary = "\r\n--" + boundary + "--";

        //string formdataTemplate = "\r\n--" + boundary +
        //                         "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

        //Dictionary<string, string> fields = new Dictionary<string, string>();
        //fields.Add("actionid", "testaction2");
        //fields.Add("actionname", "testaction2");
        //fields.Add("actiondesc", "a new action");

        //string strBody = "";

        //foreach (KeyValuePair<string, string> pair in fields)
        //{
        //    string formitem = string.Format(formdataTemplate, pair.Key, pair.Value);
        //    strBody += formitem;
        //}

        //strBody += strEndBoundary;

        //Debug.Log(strBody);

        //byte[] bytes = Encoding.ASCII.GetBytes(strBody);

        req.uploadHandler = new UploadHandlerRaw(generatedList.ToArray());
        // req.uploadHandler.contentType = "multipart/form-data; boundary=" + boundary;
        req.uploadHandler.contentType = "multipart/form-data; boundary=" + Encoding.ASCII.GetString(byteBoundary);

        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.Send();

        WaitCursor.Hide();

        if (callback != null)
        {
            callback(req);
        }
    }

    public void GetTexture(string url, System.Action<UnityWebRequest, Texture> callback)
    {
        StartCoroutine(GetWWWTexture(url, callback));
    }

    public void GetTexture(string url, System.Action<UnityWebRequest, Texture, object[]> callback, object[] passOnArgs)
    {
        StartCoroutine(GetWWWTexture(url, callback, passOnArgs));
    }

    /// <summary>
    /// Called as a coroutine and queries the url. When data are downloaded, they are returned to the callback-method
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
    /// <returns></returns>
    IEnumerator CallWWW(string url, string requestType, System.Action<UnityWebRequest, object[]> callback, object[] passOnArgs)
    {
        WaitCursor.Show();
        UnityWebRequest req = new UnityWebRequest(url, requestType);
        req.downloadHandler = new DownloadHandlerBuffer();

        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }

        yield return req.Send();

        WaitCursor.Hide();

        if (callback != null)
        {
            callback(req, passOnArgs);
        }
    }

    /// <summary>
    /// Called as a Coroutine and queries the url. Expects an image as the query-result
    /// When the image is downloaded, it is returned to the specified callback method
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded image</param>
    /// <returns></returns>
    IEnumerator GetWWWTexture(string url, System.Action<UnityWebRequest, Texture> callback)
    {
        WaitCursor.Show();
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }

        yield return req.Send();

        WaitCursor.Hide();

        if (callback != null)
        {
            if (req.responseCode == 200)
            {
                callback(req, DownloadHandlerTexture.GetContent(req));
            }
            else
            {
                callback(req, null);
            }
        }
    }

    IEnumerator GetWWWTexture(string url, Action<UnityWebRequest, Texture, object[]> callback, object[] passOnArgs)
    {
        yield return GetWWWTexture(url,
            (req, tex) =>
            {
                if (callback != null)
                {
                    callback(req, tex, passOnArgs);
                }
            }
            );
    }

    public void GetAudioClip(string url, Action<AudioClip, long> callback)
    {
        StartCoroutine(GetAudioData(url, callback));
    }

    private IEnumerator GetAudioData(string url, Action<AudioClip, long> callback)
    {
        // Alternative: get formatted audio file ========================================
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);

        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }

        yield return req.Send();

        AudioClip clip = null;
        if (req.responseCode == 200)
        {
            clip = DownloadHandlerAudioClip.GetContent(req);
        }

        if (callback != null)
        {
            callback(clip, req.responseCode);
        }
    }

    public void SendAudioClip(string url, AudioClip clip, Action<long> callback)
    {
        StartCoroutine(SendAudioData(url, clip, callback));
    }

    private IEnumerator SendAudioData(string url, AudioClip clip, Action<long> callback)
    {
        byte[] oggBytes;
        SavWav.ConvertToWav(clip, out oggBytes);
        UnityWebRequest post = new UnityWebRequest(url, "POST");

        foreach (KeyValuePair<string, string> header in StandardHeader)
        {
            post.SetRequestHeader(header.Key, header.Value);
        }

        post.uploadHandler = new UploadHandlerRaw(oggBytes);
        yield return post.Send();

        Debug.Log("Audio send code " + post.responseCode);

        if (callback != null)
        {
            callback(post.responseCode);
        }
    }
}
