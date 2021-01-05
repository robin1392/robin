using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest_Slot : MonoBehaviour
{
    public Text text_Title;
    public Slider slider;
    public Text text_Slider;

    private TDataQuestData data;

    public void Initialize(MsgQuestData msg)
    {
        data = new TDataQuestData();
        //TableManager.Get().
        text_Title.text = msg.QuestId.ToString();
        slider.value = Random.value;
        text_Slider.text = $"{msg.Value}/999";
    }
}
