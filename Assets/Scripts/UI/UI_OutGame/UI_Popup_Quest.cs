﻿using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Quest : UI_Popup
{
    public RectTransform rts_Content;
    public Text text_RemainTime;

    [Header("Daily Reward")]
    public Button[] arrBtn_Reward;
    public Image[] arrImage_Reward;
    public Text[] arrText_Reward;
    
    [Header("Slot")]
    public List<UI_Quest_Slot> listSlot;

    private float remainTimeCooltime = 1f;

    private static bool[] arrIsDailyRewardGet = new bool[3];
    private static int dailyRewardID;
    private static DateTime dateTime;
    private static List<MsgQuestData> list = new List<MsgQuestData>();
    private Vector2 mousePos;

    private void Update()
    {
        if (dateTime != null)
        {
            remainTimeCooltime -= Time.deltaTime;
            if (remainTimeCooltime <= 0)
            {
                remainTimeCooltime = 1f;

                RefreshRemainTime();
            }
        }
    }
    
    public void Initialize()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);

        if (list.Count == 0)
        {
            NetworkManager.Get().QuestInfoReq(UserInfoManager.Get().GetUserInfo().userID, InfoCallback);
        }
        else
        {
            InfoCallback();
            RefreshRemainTime();
        }
    }

    private void RefreshRemainTime()
    {
        var span = dateTime.Subtract(DateTime.Now);
        if (span.TotalSeconds > 0)
        {
            text_RemainTime.text =
                string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
        }
    }

    public void InfoCallback(MsgQuestInfoAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        var anchPos = rts_Content.anchoredPosition;
        anchPos.y = 0;
        rts_Content.anchoredPosition = anchPos;
        
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            dateTime = DateTime.Now.AddSeconds(msg.QuestInfo.RemainResetTime);

            dailyRewardID = msg.QuestInfo.DayRewardInfo.DayRewardId;
            arrIsDailyRewardGet = msg.QuestInfo.DayRewardInfo.DayRewardState;

            var dataDailyReward = new TDataQuestDayReward();
            if (TableManager.Get().QuestDayReward.GetData(msg.QuestInfo.DayRewardInfo.DayRewardId, out dataDailyReward))
            {
                arrText_Reward[0].text = $"{dataDailyReward.rewardItem01}\nx{dataDailyReward.rewardItemValue01}";
                arrText_Reward[1].text = $"{dataDailyReward.rewardItem02}\nx{dataDailyReward.rewardItemValue02}";
                arrText_Reward[2].text = $"{dataDailyReward.rewardItem03}\nx{dataDailyReward.rewardItemValue03}";

                arrBtn_Reward[0].interactable = !arrIsDailyRewardGet[0];
                arrBtn_Reward[1].interactable = !arrIsDailyRewardGet[1];
                arrBtn_Reward[2].interactable = !arrIsDailyRewardGet[2];
            }
            
            for (int i = 0; i < msg.QuestInfo.QuestData.Length || i < listSlot.Count; i++)
            {
                if (i >= msg.QuestInfo.QuestData.Length)
                {
                    listSlot[i].gameObject.SetActive(false);
                    continue;
                }
                listSlot[i].gameObject.SetActive(true);
                list.Add(msg.QuestInfo.QuestData[i]);
                listSlot[i].Initialize(msg.QuestInfo.QuestData[i]);
            }
        }
        
        Open();
    }
    
    public void InfoCallback()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        var anchPos = rts_Content.anchoredPosition;
        anchPos.y = 0;
        rts_Content.anchoredPosition = anchPos;
        
        var dataDailyReward = new TDataQuestDayReward();
        if (TableManager.Get().QuestDayReward.GetData(dailyRewardID, out dataDailyReward))
        {
            arrText_Reward[0].text = $"{dataDailyReward.rewardItem01}\nx{dataDailyReward.rewardItemValue01}";
            arrText_Reward[1].text = $"{dataDailyReward.rewardItem02}\nx{dataDailyReward.rewardItemValue02}";
            arrText_Reward[2].text = $"{dataDailyReward.rewardItem03}\nx{dataDailyReward.rewardItemValue03}";
            
            arrBtn_Reward[0].interactable = !arrIsDailyRewardGet[0];
            arrBtn_Reward[1].interactable = !arrIsDailyRewardGet[1];
            arrBtn_Reward[2].interactable = !arrIsDailyRewardGet[2];
        }
        
        for (int i = 0; i < list.Count || i < listSlot.Count; i++)
        {
            if (i >= list.Count)
            {
                listSlot[i].gameObject.SetActive(false);
                continue;
            }
            listSlot[i].gameObject.SetActive(true);
            listSlot[i].Initialize(list[i]);
        }
        
        Open();
    }

    public void Click_DailyRewardButton(int num)
    {
        NetworkManager.Get().QuestDayRewardReq(UserInfoManager.Get().GetUserInfo().userID, dailyRewardID, num, GetDailyRewardCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        mousePos = arrBtn_Reward[num].transform.position;
    }

    public void GetDailyRewardCallback(MsgQuestDayRewardAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            arrIsDailyRewardGet = msg.DayRewardInfo.DayRewardState;
            
            foreach (var reward in msg.RewardInfo)
            {
                var data = new TDataItemList();
                if (TableManager.Get().ItemList.GetData(reward.ItemId, out data))
                {
                    switch (data.id)
                    {
                        case 1:             // 골드
                            UserInfoManager.Get().GetUserInfo().gold += reward.Value;
                            UI_GetProduction.Get().Initialize(ITEM_TYPE.GOLD, mousePos, Mathf.Clamp(reward.Value, 5, 20));
                            break;
                        case 2:             // 다이아
                            UserInfoManager.Get().GetUserInfo().diamond += reward.Value;
                            UI_GetProduction.Get().Initialize(ITEM_TYPE.DIAMOND, mousePos, Mathf.Clamp(reward.Value, 5, 20));
                            break;
                        default:            // 주사위
                            break;
                    }
                }
            }

            InfoCallback();
        }
    }
}