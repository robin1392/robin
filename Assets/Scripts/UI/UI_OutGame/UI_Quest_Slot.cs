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

            text_Reward.text = $"{data.ItemId}\nx{data.ItemValue}";
        }
        else
        {
            text_Title.text = string.Empty;
            slider.value = 0;
            text_Slider.text = string.Empty;
        }
    }
}
