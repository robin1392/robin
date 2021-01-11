using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest_Slot : MonoBehaviour
{
    public Text text_Title;
    public Slider slider;
    public Text text_Slider;

    [Space]
    public Button btn_Reward;
    public Image image_Reward;
    public Text text_Reward;

    private TDataQuestData data;
    private Vector2 mousePos;

    public void Initialize(MsgQuestData msg)
    {
        data = new TDataQuestData();
        var langData = new TDataLangKO();
        if (TableManager.Get().QuestData.GetData(msg.QuestId, out data))
        {
            TableManager.Get().LangKO.GetData(x => x.name == data.questStringKey, out langData);
            text_Title.text = langData.textDesc;
            slider.value = msg.Value / (float)data.questEndValue;
            text_Slider.text = $"{msg.Value}/{data.questEndValue}";

            switch ((QUEST_STATUS)msg.Status)
            {
                case QUEST_STATUS.NONE:
                    btn_Reward.interactable = false;
                    break;
                case QUEST_STATUS.LOCK:
                    btn_Reward.interactable = false;
                    break;
                case QUEST_STATUS.OPEN:
                    btn_Reward.interactable = false;
                    break;
                case QUEST_STATUS.COMPLETE:
                    btn_Reward.interactable = true;
                    break;
                case QUEST_STATUS.CLOSE:
                    btn_Reward.interactable = false;
                    break;
            }
            text_Reward.text = $"{data.ItemId}\nx{data.ItemValue}";
        }
        else
        {
            text_Title.text = string.Empty;
            slider.value = 0;
            text_Slider.text = string.Empty;
        }
    }

    public void Click_RewardButton()
    {
        NetworkManager.Get().QuestRewardReq(UserInfoManager.Get().GetUserInfo().userID, data.id, QuestRewardCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        mousePos = btn_Reward.transform.position;
    }

    public void QuestRewardCallback(MsgQuestRewardAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            List<MsgReward> list = new List<MsgReward>();

            foreach (var reward in msg.RewardInfo)
            {
                var data = new TDataItemList();
                if (TableManager.Get().ItemList.GetData(reward.ItemId, out data))
                {
                    switch (data.id)
                    {
                        case 1:             // 골드
                            UserInfoManager.Get().GetUserInfo().gold += reward.Value;
                            UI_GetProduction.Get().Initialize(ITEM_TYPE.GOLD, mousePos, 20);
                            break;
                        case 2:             // 다이아
                            UserInfoManager.Get().GetUserInfo().diamond += reward.Value;
                            UI_GetProduction.Get().Initialize(ITEM_TYPE.DIAMOND, mousePos, 20);
                            break;
                        default: // 주사위
                        {
                            MsgReward rw = new MsgReward();
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
                UI_Main.Get().gerResult.gameObject.SetActive(true);
                UI_Main.Get().gerResult.Initialize(list.ToArray(), false, false);
            }

            btn_Reward.interactable = false;
        }
    }
}
