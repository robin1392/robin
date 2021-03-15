using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ED;

public class UI_DiceTab : MonoBehaviour
{
    public Image image_Frame;
    public Text text_Name;
    public List<UI_DiceTab> list_OtherButtons;
    public bool isOnStart;

    private UI_Panel_Dice panel;

    private void Start()
    {
        panel = GetComponentInParent<UI_Panel_Dice>();
        if (isOnStart == false) Unclick();
    }

    public void Click(int tabNum)
    {
        if (panel._isSelectMode) return;
        
        text_Name.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        image_Frame.rectTransform.DOSizeDelta(new Vector2(242, 134), 0.2f).SetEase(Ease.OutBack);
        
        foreach (var otherButton in list_OtherButtons)
        {
            otherButton.Unclick();
        }

        panel.rts_ScrollView.gameObject.SetActive(tabNum == 0);
        panel.rts_ScrollViewGuardian.gameObject.SetActive(tabNum == 1);
        panel.rts_ScrollViewEmotion.gameObject.SetActive(tabNum == 2);
        panel.listDeckInfo[0].gameObject.SetActive(tabNum < 2);
        panel.obj_EmotionDeckInfo.SetActive(tabNum == 2);
        
        // Grid 즉시 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsGettedDiceParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsUngettedDiceParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsGettedGuardianParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsUngettedGuardianParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsGettedEmotionParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.tsUngettedEmotionParent);
    }

    public void Unclick()
    {
        text_Name.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack);
        image_Frame.rectTransform.DOSizeDelta(new Vector2(242, 100), 0.2f).SetEase(Ease.OutBack);
    }
}
