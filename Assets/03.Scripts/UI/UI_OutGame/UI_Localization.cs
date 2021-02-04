using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_Localization : MonoBehaviour
{
    public string localizationKey;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(localizationKey) == false)
        {
            text.text = LocalizationManager.GetLangDesc(localizationKey);
        }
    }
}
