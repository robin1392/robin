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

    private static readonly string newline = "\\n";

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(localizationKey) == false)
        {
            string str = LocalizationManager.GetLangDesc(localizationKey);
            string final = string.Empty;

            if (str.Contains(newline))
            {
                var arr = str.Split(new string[] {newline}, StringSplitOptions.None);
                for (int i = 0; i < arr.Length; ++i)
                {
                    final += $"{arr[i]}";
                    if (i == arr.Length - 1) break;
                    final += "\n";
                }

                text.text = final;
            }
            else
            {
                text.text = str;
            }
        }
    }
}
