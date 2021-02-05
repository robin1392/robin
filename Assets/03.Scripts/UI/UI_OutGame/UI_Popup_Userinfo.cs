using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using RandomWarsProtocol.Msg;

public class UI_Popup_Userinfo : UI_Popup
{
    [Header("Trophy")] 
    public Slider slider;
    public RectTransform currentTrophy;
    public Text text_CurrentTrophy;

    [Header("Info")] public Text text_Name;
    public Text text_Win;
    public Text text_Lose;
    public Text text_WinLose;
    public Text text_MaxTrophy;
    public Text text_SearchDice;
    public Text text_SearchGaurdian;
    
    [Space]
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

        Initialize();
        
        SetEditButton(false);
    }

    public void Initialize()
    {
        int trophy = UserInfoManager.Get().GetUserInfo().trophy;
        float x = 0f;
        float subX = 0f;
        text_CurrentTrophy.text = trophy.ToString();
        foreach (var kvp in TableManager.Get().RankInfo.KeyValues)
        {
            if (trophy > kvp.Value.rankingPointMinMax[1])
            {
                x += 200f;
            }
            else
            {
                subX = 200f * ((trophy - kvp.Value.rankingPointMinMax[0]) / (float) (kvp.Value.rankingPointMinMax[1] - kvp.Value.rankingPointMinMax[0]));
                break;
            }
        }

        float value = (x + subX) / ((RectTransform) slider.transform).sizeDelta.x;
        slider.value = value;
        var pos = currentTrophy.anchoredPosition;
        pos.x = x + subX;
        currentTrophy.anchoredPosition = pos;

        // Info
        text_Name.text = $"{UserInfoManager.Get().GetUserInfo().userNickName}";

        int dice = 0;
        int gaurdian = 0;
        foreach (var getDice in UserInfoManager.Get().GetUserInfo().dicGettedDice)
        {
            if (getDice.Key >= 5000) gaurdian++;
            else dice++;
        }
        text_SearchDice.text = $"{dice}";
        text_SearchGaurdian.text = $"{gaurdian}";
    }

    private void SetEditButton(bool isEnable)
    {
        btn_EditNickname.interactable = isEnable;
        btn_EditNickname.transform.GetChild(0).GetComponent<Image>().color = isEnable ? Color.white : Color.gray;
        btn_EditNickname.transform.GetChild(0).GetComponentInChildren<Text>().color = isEnable ? Color.white : Color.gray;
    }
    
    public void OnChangeInputNickname(string value)
    {
        SetEditButton(String.Compare(oldNickname, value) != 0 && string.IsNullOrEmpty(value) == false);
    }

    public void Click_EditNickname()
    {
        NetworkManager.Get().EditUserNameReq(UserInfoManager.Get().GetUserInfo().userID, input_Nickname.text, EditNicknameCallback);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void EditNicknameCallback(MsgEditUserNameAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        UserInfoManager.Get().GetUserInfo().SetNickName(msg.UserName);
        oldNickname = msg.UserName;
        UI_Main.Get().RefreshUserInfoUI();
        SetEditButton(false);
    }
}
