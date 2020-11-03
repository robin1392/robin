using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DiceLevelUpResultSlot : MonoBehaviour
{
    public Image image_Icon;
    public Text text_Type;
    public Text text_Value;
    public Text text_AddValue;

    [Space]
    public Sprite[] arrSprite_StatTypeIcon;

    private Global.E_DICEINFOSLOT type;
    private float current;
    private float add;
    
    public void Initialize(Global.E_DICEINFOSLOT type, float current, float add)
    {
        this.type = type;
        this.current = current;
        this.add = add;

        image_Icon.sprite = arrSprite_StatTypeIcon[(int) type];
        text_Type.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_DESC + (int)type);
        text_Value.text = $"{current - add:F1}";
        text_AddValue.text = $"+{add:F1}";
    }
}
