using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    public GameObject windowObject;
    public TMP_Text headerText;
    public TMP_Text bodyText;
    public ModalWindowOption[] options;
    
    private void Awake()
    {
        ModalWindowController.RegisterModalWindow(this);

        for (int i = 0; i < options.Length; i++)
        {
            options[i].onClick.AddListener(OnOptionSelected);
        }
    }

    public void Show(ModalWindowSettings settings)
    {
        ModalWindowController.modalWindowEnabled = true;
        windowObject.SetActive(true);
        headerText.text = settings.headerText;
        bodyText.text = settings.bodyText;
        for (int i = 0; i < options.Length; i++)
        {
            bool showOption = i < settings.optionSettings.Length;
            options[i].gameObject.SetActive(showOption);
            if (showOption)
            {
                options[i].SetChoice(settings.optionSettings[i].text, settings.optionSettings[i].active, settings.optionSettings[i].callback);
            }
        }
    }

    public void Hide()
    {
        ModalWindowController.modalWindowEnabled = false;
        windowObject.SetActive(false);
    }

    public void OnOptionSelected()
    {
        Hide();
    }
}

public struct ModalWindowSettings
{
    public string headerText;
    public string bodyText;
    public ModalWindowOptionSettings[] optionSettings;
}

public struct ModalWindowOptionSettings
{
    public string text;
    public bool active;
    public Action callback;
}
