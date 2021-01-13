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
    }
}
