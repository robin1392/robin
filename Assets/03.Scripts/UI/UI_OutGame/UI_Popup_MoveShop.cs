using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform.InAppPurchase;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UI_Popup_MoveShop : UI_Popup
{
    public RectTransform rts_VerticalGroup;
    public Text text_Message;

    private UI_BoxOpenPopup.COST_TYPE type;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        var v = rts_Frame.sizeDelta;
        v.y = rts_VerticalGroup.sizeDelta.y;
        rts_Frame.sizeDelta = v;
    }

    public void Initialize(UI_BoxOpenPopup.COST_TYPE type)
    {
        gameObject.SetActive(true);
        this.type = type;
        switch (type)
        {
            case UI_BoxOpenPopup.COST_TYPE.KEY:
                break;
            case UI_BoxOpenPopup.COST_TYPE.GOLD:
                text_Message.text = string.Format(LocalizationManager.GetLangDesc("Shop_Needgoodsmessage"),
                    LocalizationManager.GetLangDesc("Gamemoney_Gold"));
                break;
            case UI_BoxOpenPopup.COST_TYPE.DIAMOND:
                text_Message.text = string.Format(LocalizationManager.GetLangDesc("Shop_Needgoodsmessage"),
                    LocalizationManager.GetLangDesc("Gamemoney_Diamond"));
                break;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_Message.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)text_Message.rectTransform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)text_Message.rectTransform.parent.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)text_Message.rectTransform.parent.parent.parent);
    }

    public void Click_MoveButton()
    {
        switch (type)
        {
            case UI_BoxOpenPopup.COST_TYPE.KEY:
                break;
            case UI_BoxOpenPopup.COST_TYPE.GOLD:
                AllClose();
                ShopManager.Get().ShowGoldShop();
                break;
            case UI_BoxOpenPopup.COST_TYPE.DIAMOND:
                AllClose();
                ShopManager.Get().ShowDiamondShop();
                break;
        }
    }
}
