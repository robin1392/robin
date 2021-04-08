using System.Collections;
using System.Collections.Generic;
using ED;
using Service.Template;
using Template.Season.GameBaseSeason;
using Template.Season.GameBaseSeason.Common;
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
        Percent.GameBase.Get().Season.SeasonResetReq(
            new SeasonResetRequest{}, 
            OnReceiveSeasonResetAck);

        // NetworkManager.session.SeasonTemplate.SeasonResetReq(NetworkManager.session.HttpClient, OnReceiveSeasonResetAck);

        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public bool OnReceiveSeasonResetAck(SeasonResetResponse response)
    {
        this.seasonInfo = response.seasonInfo;
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        if (response.errorCode == (int)EGameBaseSeasonErrorCode.Success)
        {
            UserInfoManager.Get().GetUserInfo().seasonTrophy = seasonInfo.SeasonTrophy;
            UserInfoManager.Get().GetUserInfo().rankPoint = seasonInfo.RankPoint;
            
            if (response.listRewardInfo != null && response.listRewardInfo.Count > 0)
            {
                rewardSlot.gameObject.SetActive(true);
                text_Unranked.gameObject.SetActive(false);

                text_MyRank.text = seasonInfo.Rank.ToString();

                rewardSlot.Initialize(response.listRewardInfo);
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
