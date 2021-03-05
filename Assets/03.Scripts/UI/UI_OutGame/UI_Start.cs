using System;
using System.Collections;
using System.Collections.Generic;
using Template.Account.GameBaseAccount.Common;
using UnityEngine;
using UnityEngine.UI;


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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
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
