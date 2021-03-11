using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CodeStage.AntiCheat.ObscuredTypes;
using RandomWarsResource.Data;
using Service.Core;
using Sirenix.Utilities;
using Template.MailBox.GameBaseMailBox.Common;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_MailSlot : MonoBehaviour
{
    public Text text_From;
    public Text text_Title;

    [Header("Item")] 
    public Button btn_Item;
    public Image image_ItemIcon;
    public Text text_ItemCount;

    private MailInfo info;
    
    public void Initialize(MailInfo info)
    {
        this.info = info;
        TDataMailInfo data;
        if (TableManager.Get().MailInfo.GetData(info.mailTableId, out data))
        {
            string countryCode = "EN";
            switch (ObscuredPrefs.GetString("CountryCode", "EN"))
            {
                case "KO":
                    countryCode = "kr";
                    break;
                default:
                    countryCode = "en";
                    break;
            }
            
            string sender = $"mailSender_{countryCode}";
            string title = $"mailTitle_{countryCode}";
            string text = $"mailText_{countryCode}";

            Debug.Log("======= Mail info ======");
            //foreach (var member in data.GetType().GetProperty(sender).GetValue(data))
            {
                
                text_From.text = $"From: {data.GetType().GetProperty(sender)?.GetValue(data)}";
                text_Title.text = data.GetType().GetProperty(title)?.GetValue(data).ToString();
            }
            
            // Item setting
            if (info.mailItems != null && info.mailItems.Length > 0)
            {
                int id = info.mailItems[0].ItemId;
                int value = info.mailItems[0].Value;

                TDataItemList itemData;
                if (TableManager.Get().ItemList.GetData(id, out itemData))
                {
                    image_ItemIcon.sprite = FileHelper.GetIcon(itemData.itemIcon);
                    text_ItemCount.text = $"x{value}";
                }
                else
                {
                    btn_Item.gameObject.SetActive(false);
                }
            }
            else
            {
                btn_Item.gameObject.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Click_ItemButton()
    {
        NetworkManager.session.MailBoxTemplate.MailReceiveReq(NetworkManager.session.HttpClient,
            NetworkManager.session.HttpClient.GetAccessToken(),
            info.mailId,
            RecieveItemCallback);
    }

    private bool RecieveItemCallback(GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo,
        MailInfo[] arrayMailInfo)
    {
        if (errorCode == GameBaseMailBoxErrorCode.Success)
        {
            UserInfoManager.Get().GetUserInfo().AddItem(arrayMailItemInfo, image_ItemIcon.transform.position);
            
            UI_Mailbox.RemoveMailInfo(info);
            Destroy(gameObject);
            
            return true;
        }

        return false;
    }
}
