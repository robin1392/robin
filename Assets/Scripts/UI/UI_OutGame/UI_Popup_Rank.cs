using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Rank : UI_Popup
{
    public UI_Rank_Slot[] arrSlot;

    public void Initialize()
    {
        for (int i = 0; i < arrSlot.Length; ++i)
        {
            arrSlot[i].Initialize(i + 1, -1, string.Empty);
        }
        
        NetworkManager.Get().GetRankReq(UserInfoManager.Get().GetUserInfo().userID, GetRankCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        StartCoroutine(WaitCoroutine());
    }

    public void GetRankCallback(MsgGetRankAck msg)
    {
        StopAllCoroutines();
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        //for (int i = 0; i < arrSlot.Length; i++)
        //{
        //    if (msg.RankInfo.Length > i)
        //    {
        //        arrSlot[i].Initialize(i + 1, msg.RankInfo[i].Trophy, msg.RankInfo[i].UserName);
        //    }
        //    else
        //    {
        //        arrSlot[i].Initialize(i + 1, -1, string.Empty);
        //    }
        //}
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(10f);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
    }
}
