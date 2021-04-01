using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_DeckInfo : MonoBehaviour
{
    [Header("Dice")]
    public Button[] arrButton_Dice;
    public Image[] arrImage_Dice;
    public Image[] arrImage_DiceEyes;
    public Text[] arrText_DiceClass;

    [Header("Deck Select")]
    public Button[] arrButton_Deck;
    public Image[] arrImage_DeckButton;
    public Sprite[] arrSprite_DeckButtonOnOff;
    public Text[] arrText_Deck;
    public Image image_CurrentDeckSlot;

    [Header("Guardian")]
    public Button btn_Guardian;
    public Text text_GuardianName;
    public Image image_GuardianIcon;

    private void Start()
    {
        RefreshDeckButton(true);
    }

    public void Click_DeckSlot(int index)
    {
        UserInfoManager.Get().SetActiveDeckIndex(index);
        UI_Main.Get().panel_Dice.SetActiveDeck();
        UI_Main.Get().panel_Dice.ui_MainStage.Set();
        UI_Main.Get().panel_Dice.RefreshEquipedMarkObject();
    }

    public void SetActiveDeck()
    {
        RefreshDeckButton();
        RefreshDiceIcon();
    }

    public void RefreshDiceIcon(bool isImmediate = false)
    {
        int active = UserInfoManager.Get().GetActiveDeckIndex();
        var deck = UserInfoManager.Get().GetSelectDeck(active);
         
        // Dice
        for (var i = 0; i < 5; i++)
        {
            RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
            if (TableManager.Get().DiceInfo.GetData(deck[i], out dataDiceInfo) == false)
            {
                return;
            }

            arrImage_Dice[i].sprite =
                FileHelper.GetIcon(dataDiceInfo.iconName);
            arrImage_Dice[i].SetNativeSize();
            arrImage_DiceEyes[i].color = FileHelper.GetColor(dataDiceInfo.color);
            arrText_DiceClass[i].text = UserInfoManager.Get().GetUserInfo().dicGettedDice[deck[i]][0].ToString();

            if (isImmediate == false)
            {
                arrButton_Dice[i].transform.DOScale(new Vector3(0f, 1f, 1f), 0f);
                arrButton_Dice[i].transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
            }
        }
        
        // Guardian
        RandomWarsResource.Data.TDataGuardianInfo dataGuardianInfo;
        if (TableManager.Get().GuardianInfo.GetData(deck[5], out dataGuardianInfo) == false)
        {
            return;
        }
        image_GuardianIcon.sprite = FileHelper.GetIcon(dataGuardianInfo.iconName);
        text_GuardianName.text = dataGuardianInfo.name;
    }
    public void RefreshDeckButton(bool isImmediate = false)
    {
        int index = UserInfoManager.Get().GetActiveDeckIndex();
        
        // image_CurrentDeckSlot.rectTransform.DOAnchorPos(
        //     ((RectTransform) arrButton_Deck[index].transform).anchoredPosition,
        //     isImmediate ? 0f : 0.2f).SetEase(Ease.OutBack);

        Color gray = new Color(146, 137, 227);
        for (int i = 0; i < arrText_Deck.Length; i++)
        {
            arrImage_DeckButton[i].sprite = arrSprite_DeckButtonOnOff[i == index ? 1 : 0];
            arrText_Deck[i].color = i == index ? Color.white : gray;
        }
    }
}
