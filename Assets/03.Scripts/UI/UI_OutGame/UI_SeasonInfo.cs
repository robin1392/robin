using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ED;
using UnityEngine;

public class UI_SeasonInfo : UI_Popup
{
    public GameObject pref_Message;
    public RectTransform rts_Parent;

    public void Start()
    {
        string str = LocalizationManager.GetLangDesc("Seasonpass_Info");
        str = str.Replace("\\n", "|");
        var arr = str.Split('|');
        for (int i = 0; i < arr.Length; i++)
        {
            GameObject obj = Instantiate(pref_Message, Vector3.zero, Quaternion.identity, rts_Parent);
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponentInChildren<Text>().text = arr[i];
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_Parent);
    }
}
