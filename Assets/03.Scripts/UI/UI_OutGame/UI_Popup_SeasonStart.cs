using System;
using System.Collections;
using System.Collections.Generic;
using ED;
//using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using Service.Core;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_SeasonStart : UI_Popup
{
    [Header("UI")]
    public Text text_Discription;

    public void Initialize(MsgSeasonInfo msg)
    {
        gameObject.SetActive(true);
        
        DateTime dateTime = DateTime.Now.AddSeconds(msg.SeasonResetRemainTime);
        text_Discription.text += $"\nSeason {msg.SeasonId}\n{dateTime}까지\n상태 {(SEASON_STATE) msg.SeasonState}";
        
        // setting
        UserInfoManager.Get().GetUserInfo().seasonPassId = msg.SeasonId;
        UserInfoManager.Get().GetUserInfo().seasonTrophy = 0;
        UserInfoManager.Get().GetUserInfo().seasonEndTime = DateTime.Now.AddSeconds(msg.SeasonResetRemainTime);
        UserInfoManager.Get().GetUserInfo().seasonPassRewardIds = new List<int>(new int[]{ 0, 0});
        UserInfoManager.Get().GetUserInfo().buySeasonPass = msg.IsFreeSeason && UserInfoManager.Get().GetUserInfo().buySeasonPass;
        UserInfoManager.Get().GetUserInfo().needSeasonReset = false;
        UserInfoManager.Get().GetUserInfo().seasonPassRewardStep = msg.SeasonPassRewardStep;
        
        // refresh
        FindObjectOfType<UI_Panel_Reward>().InitializeSeasonPass();
        UI_Main.Get().rankPopup.isInitialized = false;
    }
}
