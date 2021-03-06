using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Service.Core;
using Template.MailBox.GameBaseMailBox.Common;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Mailbox : UI_Popup
{
    public GameObject pref_MailSlot;
    public Transform ts_Content;
    
    [Header("Refresh")]
    public Button btn_Refresh;
    public Text text_RefreshCooltime;

    private static List<MailInfo> list_MailInfo = new List<MailInfo>();
    private DateTime refreshTime;
    private static bool isInitialized;

    private void Update()
    {
        double totalSeconds = DateTime.Now.Subtract(refreshTime).TotalSeconds;
        btn_Refresh.interactable = totalSeconds > 10;
        text_RefreshCooltime.text = totalSeconds > 10 ? string.Empty : $"{10 - totalSeconds:00}";
    }
    
    public void Initialize()
    {
        gameObject.SetActive(true);
        if (isInitialized == false)
        {
            isInitialized = true;
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
            NetworkManager.session.MailBoxTemplate.MailBoxInfoReq(NetworkManager.session.HttpClient, MailBoxInfoAck);
        }
        else
        {
            RefreshSlots();
        }
        Open();
    }

    private bool MailBoxInfoAck(GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (errorCode == GameBaseMailBoxErrorCode.Success && arrayMailInfo != null)
        {
            Debug.Log($"Mail Count : {arrayMailInfo.Length}");
            UpdateMailbox(arrayMailInfo);
            RefreshSlots();
            
            return true;
        }
        
        Debug.Log("Can't get mail info.");
        Close();
        return false;
    }

    public void RefreshSlots()
    {
        // int childCount = ts_Content.childCount;
        // if (childCount > 0)
        // {
        //     for (int i = childCount - 1; i >= 0; i--)
        //     {
        //         Destroy(ts_Content.GetChild(i).gameObject);
        //     }
        // }
        var arrTs = ts_Content.GetComponentsInChildren<Transform>();
        foreach (var ts in arrTs)
        {
            if (ts == ts_Content) continue;
            Destroy(ts.gameObject);
        }
        foreach (var info in list_MailInfo)
        {
            var mail = Instantiate(pref_MailSlot, Vector3.zero, Quaternion.identity, ts_Content).GetComponent<UI_MailSlot>();
            mail.transform.localPosition = Vector3.zero;
            mail.Initialize(info);
        }
    }
    
    public static void UpdateMailbox(MailInfo[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            list_MailInfo.Add(infos[i]);
        }
        
        list_MailInfo.Sort(delegate(MailInfo a, MailInfo b)
        {
             return string.Compare(a.mailId, b.mailId);
        });
    }

    public static void RemoveMailInfo(MailInfo info)
    {
        list_MailInfo.Remove(info);
    }

    public static int GetMailCount()
    {
        return list_MailInfo.Count;
    }

    public void Click_RefreshButton()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        NetworkManager.session.MailBoxTemplate.MailBoxRefreshReq(NetworkManager.session.HttpClient, MailBoxInfoAck);

        btn_Refresh.interactable = false;
        refreshTime = DateTime.Now;
    }
}
