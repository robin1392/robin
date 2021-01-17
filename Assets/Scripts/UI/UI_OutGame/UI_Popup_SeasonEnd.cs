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
    public Text text_Unranked;
    public Button btn_GetReward;

    private MsgSeasonResetAck msg;
    
    public void Initialize()
    {
        gameObject.SetActive(true);
        NetworkManager.Get().SeasonResetReq(UserInfoManager.Get().GetUserInfo().userID, ResetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void ResetCallback(MsgSeasonResetAck msg)
    {
        this.msg = msg;
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        if (msg.myTrophy > 0)
        {
            rewardSlot.gameObject.SetActive(true);
            text_Unranked.gameObject.SetActive(false);
            
            text_MyRank.text = msg.myRanking.ToString();
            text_MyTrophy.text = msg.myTrophy.ToString();
            
            rewardSlot.Initialize(msg.arraySeasonReward);
            btn_GetReward.interactable = true;
        }
        else
        {
            rewardSlot.gameObject.SetActive(false);
            text_Unranked.gameObject.SetActive(true);
            btn_GetReward.interactable = false;
        }
        
        Open();
    }

    public override void Close()
    {
        base.Close();
        
        UI_Main.Get().seasonStartPopup.Initialize(msg);
    }
}
