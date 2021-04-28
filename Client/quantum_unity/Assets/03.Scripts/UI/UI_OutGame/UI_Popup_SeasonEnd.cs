using System.Collections;
using System.Collections.Generic;
using ED;
//using RandomWarsProtocol;
//using RandomWarsProtocol.Msg;
using Service.Core;
using Template.Season.RandomwarsSeason.Common;
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
    UserSeasonInfo seasonInfo;

    public void Initialize()
    {
        gameObject.SetActive(true);
        //NetworkManager.Get().SeasonResetReq(UserInfoManager.Get().GetUserInfo().userID, ResetCallback);
        NetworkManager.session.SeasonTemplate.SeasonResetReq(NetworkManager.session.HttpClient, OnReceiveSeasonResetAck);

        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public bool OnReceiveSeasonResetAck(ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, ItemBaseInfo[] arrayRewardInfo)
    {
        this.seasonInfo = seasonInfo;
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        if (errorCode == ERandomwarsSeasonErrorCode.Success)
        {
            UserInfoManager.Get().GetUserInfo().seasonTrophy = seasonInfo.SeasonTrophy;
            UserInfoManager.Get().GetUserInfo().rankPoint = seasonInfo.RankPoint;
            
            if (arrayRewardInfo != null && arrayRewardInfo.Length > 0)
            {
                rewardSlot.gameObject.SetActive(true);
                text_Unranked.gameObject.SetActive(false);

                text_MyRank.text = seasonInfo.Rank.ToString();

                rewardSlot.Initialize(arrayRewardInfo);
                btn_GetReward.interactable = true;
            }
            else
            {
                rewardSlot.gameObject.SetActive(false);
                text_Unranked.gameObject.SetActive(true);
                btn_GetReward.interactable = false;
            }
        }

        Open();

        return true;
    }

    public override void Close()
    {
        base.Close();
        
        UI_Main.Get().seasonStartPopup.Initialize(seasonInfo);
    }
}
