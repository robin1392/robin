using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_PopupShopBuy : UI_Popup
{
    public Text text_Title;
    public Image image_Icon;
    public Text text_Count;
    public Button btn_Buy;
    public Image image_PriceIcon;
    public Text text_Price;
    public GameObject obj_Double;

    [Header("Dice Gauge")] 
    public GameObject obj_Gauge;
    public Slider slider_Gauge;
    public Text text_Class;
    public Text text_DiceCount;
    public GameObject obj_UpgradeIcon;

    public void Initialize(Sprite icon, string count, BuyType priceType, string price, TDataItemList item, int tab, UnityAction callback)
    {
        gameObject.SetActive(true);
        //obj_Double.SetActive(isDouble);
        
        image_Icon.sprite = icon;
        text_Count.text = count;
        
        switch (priceType)
        {
            case BuyType.gold:
                image_PriceIcon.enabled = true;
                image_PriceIcon.sprite = FileHelper.GetIcon("icon_gold");
                var pos = text_Price.rectTransform.anchoredPosition;
                pos.x = 37.5f;
                text_Price.rectTransform.anchoredPosition = pos;
                text_Price.text = price;
                //btn_Buy.interactable = UserInfoManager.Get().GetUserInfo().gold >= Int32.Parse(price);
                break;
            case BuyType.dia:
                image_PriceIcon.enabled = true;
                image_PriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                pos = text_Price.rectTransform.anchoredPosition;
                pos.x = 37.5f;
                text_Price.rectTransform.anchoredPosition = pos;
                text_Price.text = price;
                //btn_Buy.interactable = UserInfoManager.Get().GetUserInfo().diamond >= Int32.Parse(price);
                break;
            case BuyType.cash:
                image_PriceIcon.enabled = false;
                text_Price.text = price;
                pos = text_Price.rectTransform.anchoredPosition;
                pos.x = 0;
                text_Price.rectTransform.anchoredPosition = pos;
                break;
            case BuyType.free:
                image_PriceIcon.enabled = false;
                text_Price.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                pos = text_Price.rectTransform.anchoredPosition;
                pos.x = 0;
                text_Price.rectTransform.anchoredPosition = pos;
                break;
            case BuyType.ad:
                image_PriceIcon.enabled = false;
                break;
        }

        if (item != null)
        {
            text_Title.text = LocalizationManager.GetLangDesc(item.itemName_langId);
            if ((ITEM_TYPE) item.itemType == ITEM_TYPE.DICE)
            {
                TDataDiceInfo diceInfo;
                if (TableManager.Get().DiceInfo.GetData(dice => dice.id == item.id, out diceInfo))
                {
                    obj_Gauge.SetActive(true);

                    int level = UserInfoManager.Get().GetUserInfo().dicGettedDice[diceInfo.id][0];
                    int diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[diceInfo.id][1];
                    TDataDiceUpgrade dataDiceUpgrade;
                    if (TableManager.Get().DiceUpgrade
                        .GetData(x => x.diceLv == level + 1 && x.diceGrade == diceInfo.grade, out dataDiceUpgrade))
                    {
                        int needDiceCount = dataDiceUpgrade.needCard;
                        slider_Gauge.value = diceCount / (float) needDiceCount;
                        text_Class.text = $"{Global.g_class} {level}";
                        text_DiceCount.text = $"{diceCount}/{needDiceCount}";
                        obj_UpgradeIcon.SetActive(diceCount >= needDiceCount);
                    }
                }
            }
            else
            {
                obj_Gauge.SetActive(false);
            }
        }
        else
        {
            text_Title.text = string.Empty;
            obj_Gauge.SetActive(false);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_Frame);
        
        btn_Buy.onClick.RemoveAllListeners();
        btn_Buy.onClick.AddListener(() =>
        {
            if (priceType == BuyType.gold)
            {
                if (UserInfoManager.Get().GetUserInfo().gold < Int32.Parse(price))
                {
                    UI_Main.Get().moveShopPopup.Initialize(UI_BoxOpenPopup.COST_TYPE.GOLD);
                    return;
                }
            }
            else if (priceType == BuyType.dia)
            {
                if (UserInfoManager.Get().GetUserInfo().diamond < Int32.Parse(price))
                {
                    UI_Main.Get().moveShopPopup.Initialize(UI_BoxOpenPopup.COST_TYPE.DIAMOND);
                    return;
                }
            }
            callback?.Invoke();
            Close();
        });
        SetColor();
    }

    private void SetColor()
    {
        Color color = Color.white * (btn_Buy.interactable ? 1f : 0.5f);
        var texts = btn_Buy.GetComponentsInChildren<Text>();
        var images = btn_Buy.GetComponentsInChildren<Image>();

        foreach (var text in texts)
        {
            var c = text.color;
            color.a = c.a;
            text.color = color;
        }

        foreach (var image in images)
        {
            var c = image.color;
            color.a = c.a;
            image.color = color;
        }
    }
}
