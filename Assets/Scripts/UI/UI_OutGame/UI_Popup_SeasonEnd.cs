using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_SeasonEnd : UI_Popup
{
    public UI_Rank_Reward_Slot rewardSlot;

    [Header("UI")]
    public Text text_MyRank;
    public Text text_MyTrophy;
    
    public void Initialize()
    {
        gameObject.SetActive(true);
        NetworkManager.Get().SeasonResetReq(UserInfoManager.Get().GetUserInfo().userID, ResetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void ResetCallback(MsgSeasonResetAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        text_MyRank.text = msg.myRanking.ToString();
        text_MyTrophy.text = msg.myTrophy.ToString();
        
        Open();
    }
}
