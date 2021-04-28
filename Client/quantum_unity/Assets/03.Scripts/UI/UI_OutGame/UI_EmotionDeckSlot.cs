using System.Collections;
using System.Collections.Generic;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_EmotionDeckSlot : MonoBehaviour
{
    public Image image_Icon;

    public void SetIcon(int emotionId)
    {
        TDataItemList data;
        if (TableManager.Get().ItemList.GetData(emotionId, out data))
        {
            image_Icon.sprite = FileHelper.GetIcon(data.itemIcon);
        }
    }
}
