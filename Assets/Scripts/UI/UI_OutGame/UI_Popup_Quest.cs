using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Popup_Quest : UI_Popup
{
    public Text text_RemainTime;
    
    [Header("Daily Reward")]
    public Image[] arrImage_Reward;
    public Text[] arrText_Reward;
    
    [Header("Slot")]
    public List<UI_Quest_Slot> listSlot;
    
    [SerializeField]
    private int remainTime;

    private DateTime dateTime;

    private float remainTimeCooltime = 1f;

    private void Update()
    {
        if (dateTime != null)
        {
            remainTimeCooltime -= Time.deltaTime;
            if (remainTimeCooltime <= 0)
            {
                remainTimeCooltime = 1f;

                var span = dateTime.Subtract(DateTime.Now);
                if (span.TotalSeconds > 0)
                {
                    text_RemainTime.text =
                        string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
                }
            }
        }
    }
    
    public void Initialize()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        NetworkManager.Get().QuestInfoReq(UserInfoManager.Get().GetUserInfo().userID, InfoCallback);
    }

    public void InfoCallback(MsgQuestInfoAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            dateTime = DateTime.Now.AddSeconds(msg.QuestInfo.RemainResetTime);

            var dataDailyReward = new TDataQuestDayReward();
            if (TableManager.Get().QuestDayReward.GetData(msg.QuestInfo.DayRewardInfo.DayRewardId, out dataDailyReward))
            {
                arrText_Reward[0].text = $"{dataDailyReward.rewardItem01}\nx{dataDailyReward.rewardItemValue01}";
                arrText_Reward[1].text = $"{dataDailyReward.rewardItem02}\nx{dataDailyReward.rewardItemValue02}";
                arrText_Reward[2].text = $"{dataDailyReward.rewardItem03}\nx{dataDailyReward.rewardItemValue03}";
            }
            
            for (int i = 0; i < msg.QuestInfo.QuestData.Length || i < listSlot.Count; i++)
            {
                if (i >= msg.QuestInfo.QuestData.Length)
                {
                    listSlot[i].gameObject.SetActive(false);
                    continue;
                }
                listSlot[i].gameObject.SetActive(true);
                listSlot[i].Initialize(msg.QuestInfo.QuestData[i]);
            }
        }
        
        Open();
    }
}
