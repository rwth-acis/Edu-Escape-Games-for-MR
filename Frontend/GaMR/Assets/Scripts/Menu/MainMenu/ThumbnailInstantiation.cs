﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThumbnailInstantiation : BaseMenu
{
    public GameObject startPosition;

    private Vector3 size;
    private List<Thumbnail> thumbnails;
    private int startIndex = 0;
    private FocusableButton upButton;
    private FocusableButton downButton;
    private FocusableButton settingsButton;
    private FocusableButton logoutButton;
    private FocusableButton badgeButton;
    private FocusableButton feedbackButton;

    private List<string> models;
    private bool menuEnabled = true;


    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            foreach (Thumbnail t in thumbnails)
            {
                t.ButtonEnabled = menuEnabled;
            }
            if (menuEnabled)
            {
                SetButtonStates();
            }
            else
            {
                upButton.ButtonEnabled = false;
                downButton.ButtonEnabled = false;
            }
            logoutButton.ButtonEnabled = menuEnabled;
            settingsButton.ButtonEnabled = menuEnabled;
            badgeButton.ButtonEnabled = menuEnabled;
            feedbackButton.ButtonEnabled = menuEnabled;
        }
    }

    protected override void Start()
    {
        base.Start();

        //if (StartMenu.LastPosition != null)
        //{
        //    transform.parent.position = StartMenu.LastPosition;
        //}
        //if (StartMenu.LastRotation != null)
        //{
        //    transform.parent.rotation = StartMenu.LastRotation;
        //}

        thumbnails = new List<Thumbnail>();
        BoxCollider coll = startPosition.GetComponent<BoxCollider>();
        size = coll.size;
        InitializeButtons();
        InstantiateThumbnails();
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void InitializeButtons()
    {
        upButton = transform.Find("Up Button").gameObject.GetComponent<FocusableButton>();
        downButton = transform.Find("Down Button").gameObject.GetComponent<FocusableButton>();
        settingsButton = transform.Find("Settings Button").gameObject.GetComponent<FocusableButton>();
        logoutButton = transform.Find("Logout Button").gameObject.GetComponent<FocusableButton>();
        badgeButton = transform.Find("Badges Button").gameObject.GetComponent<FocusableButton>();
        feedbackButton = transform.Find("Feedback Button").gameObject.GetComponent<FocusableButton>();

        upButton.OnPressed = PageUp;
        downButton.OnPressed = PageDown;
        settingsButton.OnPressed = ShowSettings;
        badgeButton.OnPressed = ShowBadgeOverview;
        feedbackButton.OnPressed = ShowFeedbackForm;
        logoutButton.OnPressed = Logout;

        OnUpdateLanguage();


        // determine badge button visibility and fill the gap
        bool showBadgeButton = InformationManager.Instance.playerType != PlayerType.AUTHOR;

        badgeButton.gameObject.SetActive(showBadgeButton);
        GameObject topMenu2Shorter = transform.Find("Top Menu Medium").gameObject;
        topMenu2Shorter.SetActive(!showBadgeButton);
        GameObject topMenu1Shorter = transform.Find("Top Menu Short").gameObject;
        topMenu1Shorter.SetActive(showBadgeButton);
        if (!showBadgeButton) // shift the next button to fill the gap
        {
            feedbackButton.transform.localPosition = badgeButton.transform.localPosition;
        }


        MenuEnabled = false;
    }

    public override void OnUpdateLanguage()
    {
        // localization:
        upButton.Text = LocalizationManager.Instance.ResolveString("Page up");
        downButton.Text = LocalizationManager.Instance.ResolveString("Page down");
        settingsButton.Text = LocalizationManager.Instance.ResolveString("Settings");
        logoutButton.Text = LocalizationManager.Instance.ResolveString("Logout");
        badgeButton.Text = LocalizationManager.Instance.ResolveString("Badges");
        feedbackButton.Text = LocalizationManager.Instance.ResolveString("Feedback");
    }

    private void Logout()
    {
        AuthorizationManager.Instance.Logout();
    }

    private void ShowSettings()
    {
        GameObject settingsInstance = Instantiate(WindowResources.Instance.SettingsMenu);
        SettingsMenu settings = settingsInstance.GetComponent<SettingsMenu>();
        settings.OnCloseAction = () =>
        {
            MenuEnabled = true;
        };
        settingsInstance.transform.parent = transform;
        settingsInstance.transform.rotation = transform.rotation;
        settingsInstance.transform.localPosition = Vector3.zero;
        settingsInstance.transform.Translate(new Vector3(-0.1f, 0.01f, 0.01f));
        MenuEnabled = false;
    }

    private void ShowBadgeOverview()
    {
        GameObject badgeOverviewInstance = Instantiate(WindowResources.Instance.BadgeOverviewMenu);
        BadgeOverviewMenu badgeOverview = badgeOverviewInstance.GetComponent<BadgeOverviewMenu>();
        badgeOverview.OnCloseAction = () =>
        {
            MenuEnabled = true;
        };
        badgeOverviewInstance.transform.parent = transform;
        badgeOverviewInstance.transform.rotation = transform.rotation;
        badgeOverviewInstance.transform.localPosition = Vector3.zero;
        badgeOverviewInstance.transform.Translate(new Vector3(-0.1f, 0.01f, 0.01f));
        MenuEnabled = false;
    }

    private void ShowFeedbackForm()
    {
        GameObject feedbackFormInstance = Instantiate(WindowResources.Instance.FeedbackForm);
        FeedbackForm feedbackForm = feedbackFormInstance.GetComponent<FeedbackForm>();
        feedbackForm.OnCloseAction = () =>
        {
            MenuEnabled = true;
        };
        feedbackForm.transform.parent = transform;
        feedbackForm.transform.rotation = transform.rotation;
        feedbackForm.transform.localPosition = Vector3.zero;
        feedbackForm.transform.Translate(new Vector3(-0.1f, 0.01f, 0.01f));
        MenuEnabled = false;
    }

    private void PageDown()
    {
        Debug.Log("Down");
        startIndex += thumbnails.Count;
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void PageUp()
    {
        Debug.Log("Up");
        startIndex -= thumbnails.Count;
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void AvailableModelsLoaded(UnityWebRequest res)
    {
        if (res.responseCode == 200)
        {
            JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res.downloadHandler.text);
            models = array.array;
            for(int i=0;i<thumbnails.Count;i++)
            {
                int iModel = i + startIndex;
                if (iModel < array.array.Count)
                {
                    thumbnails[i].Visible = true;
                    thumbnails[i].LoadImage(array.array[iModel]);
                }
                else
                {
                    thumbnails[i].Visible = false;
                }
            }

            SetButtonStates();
        }

        MenuEnabled = true;
    }

    private void SetButtonStates()
    {
        if (models != null && models.Count > startIndex + thumbnails.Count)
        {
            downButton.ButtonEnabled = true;
        }
        else
        {
            downButton.ButtonEnabled = false;
        }

        if (startIndex > 0)
        {
            upButton.ButtonEnabled = true;
        }
        else
        {
            upButton.ButtonEnabled = false;
        }
    }

    private void InstantiateThumbnails()
    {
        for (int i=0;i<2;i++)
        {
            for (int j=0;j<4;j++)
            {
                Vector3 instantiationPosition = new Vector3(
                    0,
                    size.y / 2f * -i - size.y/4f,
                    size.z / 4f * -j - size.z / 8f);
                GameObject thumbnailObj = Instantiate(WindowResources.Instance.Thumbnail, startPosition.transform);

                thumbnailObj.transform.localPosition = instantiationPosition;

                thumbnailObj.transform.localScale = new Vector3(9, 9, 9);

                Thumbnail thumbnailScript = thumbnailObj.GetComponent<Thumbnail>();
                thumbnailScript.InstantiationParent = this;
                thumbnailScript.Visible = false;
                thumbnails.Add(thumbnailScript);
            }
        }
    }

    public void OnThumbnailClicked(string modelName)
    {
        ModelSynchronizer.Instance.LoadModelForAll(modelName);
    }
}
