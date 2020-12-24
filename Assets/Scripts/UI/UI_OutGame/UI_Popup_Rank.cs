using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Popup_Rank : UI_Popup
{
    public Text text_MyRankMessage;
    public Text text_RankMessage;
    
    public Text text_Season;
    public Text text_SeasonRemainTime;
    public Text text_MyRanking;
    public Text text_MyTrophy;
    
    public UI_Rank_Slot[] arrSlot;

    public void Initialize()
    {
        NetworkManager.Get().GetRankReq(UserInfoManager.Get().GetUserInfo().userID, GetRankCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        StartCoroutine(WaitCoroutine());
    }

    public void GetRankCallback(MsgGetRankAck msg)
    {
        StopAllCoroutines();
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        switch ((SEASON_STATE)msg.SeasonState)
        {
            case SEASON_STATE.NONE:
                break;
            case SEASON_STATE.PRE:
                break;
            case SEASON_STATE.GOING:
                text_RankMessage.gameObject.SetActive(false);
                
                text_Season.text = $"SEASON {msg.SeasonIndex}";
                //text_SeasonRemainTime.text = msg.SeasonRemainTime.ToString();
                StartCoroutine(TimerCoroutine(msg.SeasonRemainTime));
                text_MyRanking.text = msg.myRanking.ToString();
                text_MyTrophy.text = msg.myTrophy.ToString();
            
                for (int i = 0; i < arrSlot.Length && i < msg.TopRankInfo.Length; i++)
                {
                    arrSlot[i].Initialize(
                        msg.TopRankInfo[i].Ranking,
                        msg.TopRankInfo[i].Trophy,
                        msg.TopRankInfo[i].Name,
                        msg.TopRankInfo[i].Class,
                        msg.TopRankInfo[i].DeckInfo
                    );
                }
                break;
            case SEASON_STATE.ACCOUNT:
                break;
            case SEASON_STATE.END:
                break;
        }
    }

    IEnumerator TimerCoroutine(int seconds)
    {
        while (true)
        {
            text_SeasonRemainTime.text = $"{seconds / 60 / 60 / 24}:{seconds / 60 / 60 % 24}:{seconds / 60 % 60}:{seconds % 60}";

            yield return new WaitForSeconds(1f);
            
            seconds--;
        }
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(10f);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
    }
}
