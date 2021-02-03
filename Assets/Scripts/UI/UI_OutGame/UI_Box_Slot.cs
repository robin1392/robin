#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Box_Slot : MonoBehaviour
{
    [Header("Link")]
    public UI_BoxOpenPopup ui_BoxOpenPopup;
    public Button btn;
    public Image image_BG;
    public Image image_Icon;
    public Text text_Name;
    public Image image_CostIcon;
    public Text text_Cost;
    public Text text_Count;

    [Header("Resources")]
    public Sprite[] arrSprite_BG;
    public Sprite[] arrSprite_BoxIcon;

    [Space]
    public Material mtl_Grayscale;

    private int boxID;
    private int needKey;

    public void Initialize(int id, int count)
    {
        RandomWarsResource.Data.TDataItemList tDataItemList;
        if (TableManager.Get().ItemList.GetData(id, out tDataItemList) == false)
        {
            return;
        }


        boxID = id;
        needKey = tDataItemList.openKeyValue;
        //Debug.LogFormat("ID:{0}, COUNT:{1}, KEY:{2}", id, count, needKeyCount);
        bool isEnable = false;


        isEnable = needKey <= UserInfoManager.Get().GetUserInfo().key;
        text_Name.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
        text_Cost.text = needKey.ToString();
        text_Count.text = $"x{count}";

        text_Cost.color = isEnable ? Color.white : ParadoxNotion.ColorUtils.HexToColor("DF362D");
        text_Name.color = isEnable ? Color.white : Color.gray;
        text_Count.color = isEnable ? Color.white : Color.gray;
        image_CostIcon.color = isEnable ? Color.white : Color.gray;
        //image_Icon.sprite = arrSprite_BoxIcon[id - (int)RandomWarsResource.Data.EItemListKey.boss01box];
        image_Icon.sprite = FileHelper.GetIcon(tDataItemList.itemIcon);


        //if (needKey >= 0)
        //{
        //    isEnable = needKey <= UserInfoManager.Get().GetUserInfo().key;
        //    text_Name.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
        //    text_Cost.text = needKey.ToString();
        //    text_Count.text = $"x{count}";

        //    text_Cost.color = isEnable ? Color.white : ParadoxNotion.ColorUtils.HexToColor("DF362D");
        //    text_Name.color = isEnable ? Color.white : Color.gray;
        //    text_Count.color = isEnable ? Color.white : Color.gray;
        //    image_CostIcon.color = isEnable ? Color.white : Color.gray;
        //    image_Icon.sprite = arrSprite_BoxIcon[id/1000 - 1];
        //}
        //else
        //{
        //    text_Name.text = $"NULL";
        //    image_Icon.sprite = FileHelper.GetNullIcon();
        //    image_Icon.SetNativeSize();
        //    text_Count.text = string.Empty;
        //    text_Cost.text = string.Empty;
        //}
        btn.interactable = isEnable;

        if (isEnable == false)
        {
            image_BG.material = mtl_Grayscale;
            image_Icon.material = mtl_Grayscale;
        }
    }

    public void Click_Open()
    {
        UI_Main.Get().Click_BoxOpen(boxID, UI_BoxOpenPopup.COST_TYPE.KEY, needKey);
    }
}
