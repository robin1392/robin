using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using Debug = ED.Debug;

public class UI_Popup_Quest : UI_Popup
{
    [SerializeField]
    private int remainTime;
    
    public void Initialize()
    {
        NetworkManager.Get().QuestInfoReq(UserInfoManager.Get().GetUserInfo().userID, InfoCallback);
    }

    public void InfoCallback(MsgQuestInfoAck msg)
    {
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            remainTime = msg.QuestInfo.RemainResetTime;

            for (int i = 0; i < msg.QuestInfo.QuestData.Length; i++)
            {
                Debug.Log($"ID:{msg.QuestInfo.QuestData[i].QuestId}\nValue:{msg.QuestInfo.QuestData[i].Value}\nStatus:{(QUEST_STATUS)msg.QuestInfo.QuestData[i].Status}");
            }
        }
    }
}
