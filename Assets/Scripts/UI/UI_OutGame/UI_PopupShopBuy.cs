using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform;
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

    [Header("Dice Gauge")] 
    public GameObject obj_Gauge;
    public Slider slider_Gauge;
    public Text text_Class;
    public Text text_DiceCount;
    public GameObject obj_UpgradeIcon;

    public void Initialize(Sprite icon, string count, BuyType priceType, string price, UnityAction callback)
    {
        gameObject.SetActive(true);
        
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
                break;
            case BuyType.dia:
                image_PriceIcon.enabled = true;
                image_PriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                pos = text_Price.rectTransform.anchoredPosition;
                pos.x = 37.5f;
                text_Price.rectTransform.anchoredPosition = pos;
                text_Price.text = price;
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
        
        btn_Buy.onClick.RemoveAllListeners();
        btn_Buy.onClick.AddListener(() =>
        {
            callback?.Invoke();
            Close();
        });
    }
}
