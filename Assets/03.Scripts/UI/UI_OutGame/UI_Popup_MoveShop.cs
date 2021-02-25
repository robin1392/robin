using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform.InAppPurchase;
using UnityEngine;

public class UI_Popup_MoveShop : UI_Popup
{
    public RectTransform rts_VerticalGroup;

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
    }

    public void Click_MoveButton()
    {
        switch (type)
        {
            case UI_BoxOpenPopup.COST_TYPE.KEY:
                break;
            case UI_BoxOpenPopup.COST_TYPE.GOLD:
                ShopManager.Get().ShowGoldShop();
                break;
            case UI_BoxOpenPopup.COST_TYPE.DIAMOND:
                ShopManager.Get().ShowDiamondShop();
                break;
        }
    }
}
