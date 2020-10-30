#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
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

    public void Initialize(int id, int count, int needKeyCount)
    {
        boxID = id;
        needKey = needKeyCount;
        //Debug.LogFormat("ID:{0}, COUNT:{1}, KEY:{2}", id, count, needKeyCount);

        bool isEnable = needKeyCount <= UserInfoManager.Get().GetUserInfo().key;
        btn.interactable = isEnable;
        text_Cost.text = needKeyCount.ToString();
        text_Count.text = $"x{count}";
        
        text_Cost.color = isEnable ? Color.white : ParadoxNotion.ColorUtils.HexToColor("DF362D");
        // text_Name.color = isEnable ? Color.white : Color.gray;
        // text_Count.color = isEnable ? Color.white : Color.gray;

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
