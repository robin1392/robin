using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Popup_Rank : UI_Popup
{
    public Text text_Season;
    public Text text_SeasonRemainTime;
    public Text text_MyRanking;
    public Text text_MyTrophy;
    
    public UI_Rank_Slot[] arrSlot;

    public void Initialize()
    {
        // for (int i = 0; i < arrSlot.Length; ++i)
        // {
        //     arrSlot[i].Initialize(i + 1, -1, string.Empty);
        // }
        
        NetworkManager.Get().GetRankReq(UserInfoManager.Get().GetUserInfo().userID, GetRankCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        StartCoroutine(WaitCoroutine());
    }

    public void GetRankCallback(MsgGetRankAck msg)
    {
        StopAllCoroutines();
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        if (msg.SeasonState == 0)
        {
            
        }
        else
        {
            text_Season.text = msg.SeasonIndex.ToString();
            text_SeasonRemainTime.text = msg.SeasonRemainTime.ToString();
            text_MyRanking.text = msg.myRanking.ToString();
            text_MyTrophy.text = msg.myTrophy.ToString();
        
            for (int i = 0; i < arrSlot.Length; i++)
            {
                arrSlot[i].Initialize(
                    msg.TopRankInfo[i].Ranking,
                    msg.TopRankInfo[i].Trophy,
                    msg.TopRankInfo[i].Name,
                    msg.TopRankInfo[i].Class,
                    msg.TopRankInfo[i].DeckInfo
                );
            }
        }
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(10f);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
    }
}
