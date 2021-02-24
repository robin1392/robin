using System;
using System.Collections;
using System.Collections.Generic;
using ED;
//using RandomWarsProtocol;
//using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Service.Core;
using Template.Quest.RandomwarsQuest.Common;
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
    private static List<QuestData> list = new List<QuestData>();
    private Vector2 mousePos;

    private void Update()
    {
        if (dateTime != null)
        {
            remainTimeCooltime -= Time.deltaTime;
            if (remainTimeCooltime <= 0)
            {
                remainTimeCooltime = 1f;

                //RefreshRemainTime();
            }
        }
    }
    
    public void Initialize()
    {
        gameObject.SetActive(true);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);

        if (list.Count == 0)
        {
            //NetworkManager.Get().QuestInfoReq(UserInfoManager.Get().GetUserInfo().userID, InfoCallback);
            NetworkManager.session.QuestTemplate.QuestInfoReq(NetworkManager.session.HttpClient, OnReceiveQuestInfoAck);
        }
        else
        {
            InfoCallback();
            //RefreshRemainTime();
            Open();
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
        else
        {
            list.Clear();
            Close();
            UI_Main.Get().ShowMessageBox("퀘스트 리셋", "퀘스트가 리셋되었습니다.");
        }
    }

    public static void QuestUpdate(QuestData[] datas)
    {
        if (datas != null)
        {
            for (int i = 0; i < datas.Length; ++i)
            {
                var q = list.Find(m => m.QuestId == datas[i].QuestId);
                if (q != null)
                {
                    q.Value = datas[i].Value;
                    q.Status = datas[i].Status;
                }
            }
        }
    }
    
    public static void QuestUpdate(QuestInfo questInfo)
    {
        if (questInfo != null)
        {
            dateTime = DateTime.Now.AddSeconds(questInfo.RemainResetTime);

            dailyRewardID = questInfo.DayRewardInfo.DayRewardId;
            arrIsDailyRewardGet = questInfo.DayRewardInfo.DayRewardState;

            for (int i = 0; i < questInfo.QuestData.Length; i++)
            {
                list.Add(questInfo.QuestData[i]);
            }
        }
    }

    public static bool IsCompletedQuest()
    {
        foreach (var questData in list)
        {
            if ((QUEST_STATUS) questData.Status == QUEST_STATUS.COMPLETE) return true;
        }

        for (int i = 0; i < arrIsDailyRewardGet.Length; i++)
        {
            if (arrIsDailyRewardGet[i] == false) return true;
        }

        return false;
    }

    public bool OnReceiveQuestInfoAck(ERandomwarsQuestErrorCode errorCode, QuestInfo questInfo)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        // var anchPos = rts_Content.anchoredPosition;
        // anchPos.y = 0;
        // rts_Content.anchoredPosition = anchPos;
        
        if (errorCode == ERandomwarsQuestErrorCode.Success)
        {
            dateTime = DateTime.Now.AddSeconds(questInfo.RemainResetTime);

            dailyRewardID = questInfo.DayRewardInfo.DayRewardId;
            arrIsDailyRewardGet = questInfo.DayRewardInfo.DayRewardState;

            var dataDailyReward = new TDataQuestDayReward();
            if (TableManager.Get().QuestDayReward.GetData(questInfo.DayRewardInfo.DayRewardId, out dataDailyReward))
            {
                TDataItemList itemData;
                if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem01, out itemData))
                {
                    arrImage_Reward[0].sprite = FileHelper.GetIcon(itemData.itemIcon);
                }
                if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem02, out itemData))
                {
                    arrImage_Reward[1].sprite = FileHelper.GetIcon(itemData.itemIcon);
                }
                // if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem03, out itemData))
                // {
                //     arrImage_Reward[2].sprite = FileHelper.GetIcon(itemData.itemIcon);
                // }
                
                arrText_Reward[0].text = $"x{dataDailyReward.rewardItemValue01}";
                arrText_Reward[1].text = $"x{dataDailyReward.rewardItemValue02}";
                arrText_Reward[2].text = $"x{dataDailyReward.rewardItemValue03}";

                arrBtn_Reward[0].interactable = !arrIsDailyRewardGet[0];
                arrBtn_Reward[1].interactable = !arrIsDailyRewardGet[1];
                arrBtn_Reward[2].interactable = !arrIsDailyRewardGet[2];
            }
            
            for (int i = 0; i < questInfo.QuestData.Length || i < listSlot.Count; i++)
            {
                if (i >= questInfo.QuestData.Length)
                {
                    listSlot[i].gameObject.SetActive(false);
                    continue;
                }
                listSlot[i].gameObject.SetActive(true);
                list.Add(questInfo.QuestData[i]);
                listSlot[i].Initialize(questInfo.QuestData[i]);
            }
        }
        
        Open();

        return true;
    }
    
    public void InfoCallback()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        //
        // var anchPos = rts_Content.anchoredPosition;
        // anchPos.y = 0;
        // rts_Content.anchoredPosition = anchPos;
        //
        var dataDailyReward = new TDataQuestDayReward();
        if (TableManager.Get().QuestDayReward.GetData(dailyRewardID, out dataDailyReward))
        {
            TDataItemList itemData;
            if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem01, out itemData))
            {
                arrImage_Reward[0].sprite = FileHelper.GetIcon(itemData.itemIcon);
                arrImage_Reward[0].SetNativeSize();
            }
            if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem02, out itemData))
            {
                arrImage_Reward[1].sprite = FileHelper.GetIcon(itemData.itemIcon);
                arrImage_Reward[1].SetNativeSize();
            }
            //if (TableManager.Get().ItemList.GetData(item => item.id == dataDailyReward.rewardItem03, out itemData))
            {
                arrImage_Reward[2].sprite = FileHelper.GetIcon("icon_unknown_dice");
                arrImage_Reward[2].SetNativeSize();
            }

            arrText_Reward[0].text = $"x{dataDailyReward.rewardItemValue01}";
            arrText_Reward[1].text = $"x{dataDailyReward.rewardItemValue02}";
            arrText_Reward[2].text = $"x{dataDailyReward.rewardItemValue03}";
            
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
    }

    public void Click_DailyRewardButton(int num)
    {
        //NetworkManager.Get().QuestDayRewardReq(UserInfoManager.Get().GetUserInfo().userID, dailyRewardID, num, GetDailyRewardCallback);
        NetworkManager.session.QuestTemplate.QuestDailyRewardReq(NetworkManager.session.HttpClient, dailyRewardID, num, OnReceiveQuestDailyRewardAck);

        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        mousePos = arrBtn_Reward[num].transform.position;
    }

    bool OnReceiveQuestDailyRewardAck(ERandomwarsQuestErrorCode errorCode, QuestData[] arrayQuestData, ItemBaseInfo[] arrayRewardInfo, QuestDayReward dailyRewardInfo)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (errorCode == ERandomwarsQuestErrorCode.Success)
        {
            arrIsDailyRewardGet = dailyRewardInfo.DayRewardState;
            List<ItemBaseInfo> list = new List<ItemBaseInfo>();

            foreach (var reward in arrayRewardInfo)
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
                        default: // 주사위
                        {
                            ItemBaseInfo rw = new ItemBaseInfo();
                            rw.ItemId = reward.ItemId;
                            rw.Value = reward.Value;
                            list.Add(rw);
                        }
                            break;
                    }
                }
            }

            if (list.Count > 0)
            {
                UI_Main.Get().gerResult.Initialize(list.ToArray(), false, false);
            }
            
            QuestUpdate(arrayQuestData);

            InfoCallback();
        }

        return true;
    }
}
