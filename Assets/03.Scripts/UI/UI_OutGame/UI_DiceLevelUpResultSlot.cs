using System;
using DG.Tweening;
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
    
    public void Initialize(Global.E_DICEINFOSLOT type, float current, float add, float delay)
    {
        this.type = type;
        this.current = current;
        this.add = add;

        image_Icon.sprite = arrSprite_StatTypeIcon[(int) type];
        switch (type)
        {
            case Global.E_DICEINFOSLOT.Info_Hp:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Hp");
                break;
            case Global.E_DICEINFOSLOT.Info_MoveSpeed:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Movespd");
                break;
            case Global.E_DICEINFOSLOT.Info_AtkPower:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Atk");
                break;
            case Global.E_DICEINFOSLOT.Info_AtkSpeed:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Atkspd");
                break;
            case Global.E_DICEINFOSLOT.Info_Skill:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Effect");
                break;
            case Global.E_DICEINFOSLOT.Info_Cooltime:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Effectcooltime");
                break;
            case Global.E_DICEINFOSLOT.Info_Range:
                text_Type.text = LocalizationManager.GetLangDesc("Minioninfo_Atkrange");
                break;
        }
        text_Value.text = $"{current - add:F1}";
        text_AddValue.text = $"+{add:F1}";

        StartCoroutine(AddCoroutine(delay));
    }

    private IEnumerator AddCoroutine(float delay)
    {
        float t = 0;
        float max = Mathf.Clamp(add * 0.01f, 0.3f, 0.8f);

        yield return new WaitForSeconds(delay);
        
        while (t < max)
        {
            text_Value.text = $"{Mathf.Lerp(current - add, current, t / max):F1}";
            text_AddValue.text = $"+{Mathf.Lerp(add, 0, t / max):F1}";

            text_Value.rectTransform.localScale = Vector3.one * 1.2f;
            text_Value.rectTransform.DOScale(1f, 0.05f);

            t += 0.05f;
            SoundManager.instance?.Play(Global.E_SOUND.SFX_UI_DICE_POINT_UP);
            yield return new WaitForSeconds(0.05f);
        }

        text_Value.text = $"{current:F1}";
        text_AddValue.text = string.Empty;
    }
}
