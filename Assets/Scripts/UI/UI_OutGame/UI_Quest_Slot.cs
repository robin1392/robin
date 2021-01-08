using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using RandomWarsProtocol;
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
        
    }
}
