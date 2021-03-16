using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Template.Account.GameBaseAccount.Common;
using UnityEngine;
using UnityEngine.UI;
using ED;


public class UI_Start : SingletonDestroy<UI_Start>
{
    public UI_CommonMessageBox commonMessageBox;
    public Button btn_GooglePlay;
    public Button btn_GameCenter;
    public Button btn_GuestAccount;

    private Text textGameStatus;


    #region unity base

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UI_Popup.stack.Count > 0)
            {
                UI_Popup.ClosePop();
            }
            else
            {
                commonMessageBox.Initialize(LocalizationManager.GetLangDesc("Option_Gamequit"),
                    LocalizationManager.GetLangDesc("Option_Gamequitquestion"), 
                    LocalizationManager.GetLangDesc("Option_Quit"), null, () =>
                    {
                        Application.Quit();
                    });
            }
        }
    }
    #endregion
    
    
    #region ui component
    public void InitUIElement()
    {
        textGameStatus = this.transform.Find("PanelTitle/Text_Status").GetComponent<Text>();
    }

    public void Click_GuestButton()
    {
        commonMessageBox.Initialize("Guest Account", "Are you sure?", "OK", null, () =>
        {
            btn_GuestAccount.gameObject.SetActive(false);
            btn_GooglePlay.gameObject.SetActive(false);
            btn_GameCenter.gameObject.SetActive(false);
            SetTextStatus(Global.g_startStatusUserData);
            
            ObscuredPrefs.SetInt("PlatformType", (int)EPlatformType.Guest);
            NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, string.Empty, (int)EPlatformType.Guest, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, GameStateManager.Get().OnReceiveAccountLoginAck);
        });
    }
    #endregion
    
    
    #region system

    public void SetTextStatus(string statusText)
    {
        textGameStatus.text = $"{statusText}";
    }
    #endregion
    
    
}
