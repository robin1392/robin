using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ED;
//using RandomWarsProtocol;
//using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Service.Core;
using Template.Quest.RandomwarsQuest.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest_Slot : MonoBehaviour
{
    public Text text_Title;
    public Slider slider;
    public Color[] arrColor_Slider;
    public Image image_Slider;
    public Text text_Slider;

    [Header("Reward")]
    public Button btn_Reward;
    public Image image_Reward;
    public Text text_Reward;

    private QuestData msg;
    private TDataQuestData data;
    private Vector2 mousePos;

    public void Initialize(QuestData msg)
    {
        this.msg = msg;
        data = new TDataQuestData();
        //var langData = new TDataLangKO();
        if (TableManager.Get().QuestData.GetData(msg.QuestId, out data))
        {
            //TableManager.Get().LangKO.GetData(x => x.name == data.questStringKey, out langData);
            //text_Title.text = langData.textDesc;
            text_Title.text = LocalizationManager.GetLangDesc(data.questStringKey);
            slider.value = msg.Value / (float)data.questEndValue;
            text_Slider.text = $"{msg.Value}/{data.questEndValue}";
            image_Slider.color = arrColor_Slider[0];

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
                    image_Slider.color = arrColor_Slider[1];
                    btn_Reward.interactable = true;
                    break;
                case QUEST_STATUS.CLOSE:
                    image_Slider.color = arrColor_Slider[1];
                    btn_Reward.interactable = false;
                    break;
            }

            TDataItemList itemData;
            if (TableManager.Get().ItemList.GetData(item => item.id == data.ItemId, out itemData))
            {
                image_Reward.sprite = FileHelper.GetIcon(itemData.itemIcon);
                image_Reward.SetNativeSize();
                text_Reward.text = $"x{data.ItemValue}";
            }
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
        //NetworkManager.Get().QuestRewardReq(UserInfoManager.Get().GetUserInfo().userID, data.id, QuestRewardCallback);
        NetworkManager.session.QuestTemplate.QuestRewardReq(NetworkManager.session.HttpClient, data.id, OnReceiveQuestRewardAck);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        mousePos = btn_Reward.transform.position;
    }

    public bool OnReceiveQuestRewardAck(ERandomwarsQuestErrorCode errorCode, QuestData[] arrayQuestData, ItemBaseInfo[] arrayRewardInfo)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (errorCode == ERandomwarsQuestErrorCode.Success)
        {
            UI_Main.Get().AddReward(arrayRewardInfo, btn_Reward.transform.position);

            UI_Popup_Quest.QuestUpdate(arrayQuestData);
            UI_Main.Get().questPopup.InfoCallback();
        }

        return true;
    }
}
