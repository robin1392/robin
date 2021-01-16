using System.Collections;
using System.Collections.Generic;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardItem_Slot : MonoBehaviour
{
    public Image image_Icon;
    public Text text_Count;

    public void Initialize(int id, int count = 1)
    {
        TDataItemList data;
        if (TableManager.Get().ItemList.GetData(item => item.id == id, out data))
        {
            image_Icon.sprite = FileHelper.GetIcon(data.itemIcon);
            image_Icon.SetNativeSize();
            text_Count.text = $"x{count}";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
