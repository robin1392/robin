using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_SeasonStart : UI_Popup
{
    [Header("UI")]
    public Text text_Discription;

    public void Initialize(MsgSeasonResetAck msg)
    {
        
    }
}
