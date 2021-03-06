using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using UnityEngine;
using UnityEngine.UI;
using RandomWarsProtocol.Msg;

public class UI_Popup_Userinfo : UI_Popup
{
    [Header("Trophy")] 
    public RectTransform rts_Content;
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
    public Button btn_ResetWinLose;

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
        text_MaxTrophy.text = UserInfoManager.Get().GetUserInfo().highTrophy.ToString();
        int winCount = UserInfoManager.Get().GetUserInfo().winCount;
        text_Win.text = winCount.ToString();
        int loseCount = UserInfoManager.Get().GetUserInfo().defeatCount;
        text_Lose.text = loseCount.ToString();
        if (winCount + loseCount > 0) text_WinLose.text = $"{(winCount / (winCount + loseCount)):F1}%";
        else text_WinLose.text = "0.0%";
        foreach (var kvp in TableManager.Get().ClassInfo.KeyValues)
        {
            if (trophy > kvp.Value.trophyPointMinMax[1])
            {
                x += 300f;
            }
            else
            {
                subX = 300f * ((trophy - kvp.Value.trophyPointMinMax[0]) / (float) (kvp.Value.trophyPointMinMax[1] - kvp.Value.trophyPointMinMax[0]));
                break;
            }
        }

        float value = (x + subX) / ((RectTransform) slider.transform).sizeDelta.x;
        slider.value = value;
        var pos = currentTrophy.anchoredPosition;
        pos.x = x + subX;
        currentTrophy.anchoredPosition = pos;
        // var pos2 = rts_Content.anchoredPosition;
        // pos2.x = -pos.x + 300f;
        // rts_Content.anchoredPosition = pos2;
        rts_Content.DOAnchorPosX(Mathf.Clamp(-pos.x + 300f, -float.MaxValue, 0f), 0.5f);

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
        //NetworkManager.Get().EditUserNameReq(UserInfoManager.Get().GetUserInfo().userID, input_Nickname.text, EditNicknameCallback);


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
