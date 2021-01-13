using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_SeasonStart : UI_Popup
{
    [Header("UI")]
    public Text text_Discription;

    public void Initialize(MsgSeasonResetAck msg)
    {
        gameObject.SetActive(true);
        
        DateTime dateTime = DateTime.Now.AddSeconds(msg.SeasonRemainTime);
        text_Discription.text += $"\nSeason {msg.SeasonId}\n{dateTime}까지\n상태 {(SEASON_STATE) msg.SeasonState}";
        
        // setting
        UserInfoManager.Get().GetUserInfo().seasonPassId = msg.SeasonId;
        UserInfoManager.Get().GetUserInfo().seasonTrophy = 0;
        UserInfoManager.Get().GetUserInfo().seasonEndTime = DateTime.Now.AddSeconds(msg.SeasonRemainTime);
        UserInfoManager.Get().GetUserInfo().seasonPassRewardIds.Clear();
        UserInfoManager.Get().GetUserInfo().buySeasonPass = false;
        UserInfoManager.Get().GetUserInfo().needSeasonReset = false;
        
        // refresh
        FindObjectOfType<UI_Panel_Reward>().InitializeSeasonPass();
        UI_Main.Get().rankPopup.isInitialized = false;
    }
}
