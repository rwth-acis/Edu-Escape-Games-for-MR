﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private Dictionary<string, string> dictionary;
    private List<string> keyboardLayout;
    private Language language;
    private List<ILanguageUpdater> updateReceivers = new List<ILanguageUpdater>();

    public void Start()
    {
        UpdateLanguage();
    }

    public void AddUpdateReceiver(ILanguageUpdater receiver)
    {
        updateReceivers.Add(receiver);
    }

    public void RemoveUpdateReceiver(ILanguageUpdater receiver)
    {
        updateReceivers.Remove(receiver);
    }
    


    private void InformReceivers()
    {
        foreach(ILanguageUpdater receiver in updateReceivers)
        {
            receiver.OnUpdateLanguage();
        }
    }

    private void LoadDictionary()
    {
        dictionary = new Dictionary<string, string>();
        TextAsset jsonData = Resources.Load<TextAsset>("values/strings-" + GetLanguageCode(language));
        JsonStringItemArray array = JsonUtility.FromJson<JsonStringItemArray>(jsonData.text);
        foreach(StringItem item in array.strings)
        {
            if (!dictionary.ContainsKey(item.key))
            {
                dictionary.Add(item.key, item.value);
            }
            else
            {
                Debug.LogError("The language file for " + GetLanguageCode(language) + " contains the same name-id multiple times: " + item.key);
            }
        }
    }

    private void LoadKeyboardLayout()
    {
        keyboardLayout = new List<string>();
        TextAsset jsonData = Resources.Load<TextAsset>("values/keyboardLayout-" + GetKeyboardLanguageCode(language));
        JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(jsonData.text);
        keyboardLayout = array.array;
    }

    public List<string> KeyboardLayout
    {
        get { return keyboardLayout; }
    }

    private string GetKeyboardLanguageCode(Language language)
    {
        if (language == Language.GERMAN)
        {
            return GetLanguageCode(Language.GERMAN);
        }
        else
        {
            return GetLanguageCode(Language.ENGLISH);
        }
    }

    public string ResolveString(string text)
    {
        if (dictionary.ContainsKey(text))
        {
            return dictionary[text];
        }
        else
        {
            Debug.LogWarning("Requested string could not be translated: " + text);
            return text;
        }
    }

    public void UpdateLanguage()
    {
        language = InformationManager.Instance.Language;
        LoadDictionary();
        LoadKeyboardLayout();

        InformReceivers();

    }

    private string GetLanguageCode(Language language)
    {
        switch (language)
        {
            case Language.ENGLISH:
                return "en";
            case Language.GERMAN:
                return "de";
            case Language.DUTCH:
                return "nl";
            default:
                return "en";
        }
    }
}
