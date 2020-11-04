﻿using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using RandomWarsProtocol.Msg;

public class UI_Popup_Userinfo : UI_Popup
{
    public Text text_UID;
    public InputField input_Nickname;
    public Button btn_EditNickname;

    private string oldNickname;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        text_UID.text = $"UID : {UserInfoManager.Get().GetUserInfo().userID}";
        oldNickname = UserInfoManager.Get().GetUserInfo().userNickName;
        input_Nickname.text = $"{oldNickname}";
        
        SetEditButton(false);
    }

    private void SetEditButton(bool isEnable)
    {
        btn_EditNickname.interactable = isEnable;
        btn_EditNickname.transform.GetChild(0).GetComponent<Image>().color = isEnable ? Color.white : Color.gray;
        btn_EditNickname.transform.GetChild(0).GetComponentInChildren<Text>().color = isEnable ? Color.white : Color.gray;
    }
    
    public void OnChangeInputNickname(string value)
    {
        SetEditButton(String.Compare(oldNickname, value) != 0);
    }

    public void Click_EditNickname()
    {
        NetworkManager.Get().EditUserNameReq(UserInfoManager.Get().GetUserInfo().userID, input_Nickname.text, EditNicknameCallback);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void EditNicknameCallback(MsgEditUserNameAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        UI_Main.Get().RefreshUserInfoUI();
        SetEditButton(false);
    }
}