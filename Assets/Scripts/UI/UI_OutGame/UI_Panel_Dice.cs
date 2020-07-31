﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace ED
{
    public class UI_Panel_Dice : MonoBehaviour
    {
        public Data_AllDice dataAllDice;
        public UI_Popup_Dice_Info ui_Popup_Dice_Info;
        public Image[] arrImageDeck;
        public Image[] arrImageDeck_Main;
        public RectTransform tsGettedDiceParent;
        public UI_Getted_Dice[] arrGettedDice;
        public GameObject objSelectBlind;
        public RectTransform rts_ScrollView;
        public ScrollRect scrollView;
        public RectTransform rts_Content;
        public Text text_Getted;
        public GameObject obj_Ciritical;
        
        [Header("Prefabs")]
        public GameObject prefGettedDice;

        private bool _isSelectMode;
        private int _selectedDiceId;

        private void Start()
        {
            RefreshDeck();
            RefreshGettedDice();
            
            var safeArea = Screen.safeArea;
            var canvas = GetComponentInParent<Canvas>();
            
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;
 
            //SafeAreaRect.anchorMin = anchorMin;
            rts_ScrollView.anchorMax = anchorMax;
            
            //scrollView.OnDrag(data => { GetComponentInParent<UI_Main>().OnDrag((PointerEventData)data);});
            
        }

        private void RefreshDeck()
        {
            var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
            var splitDeck = deck.Split('/');

            for (var i = 0; i < arrImageDeck.Length; i++)
            {
                var num = int.Parse(splitDeck[i]);
                arrImageDeck[i].sprite = dataAllDice.listDice.Find(data => data.id == num).icon;
                arrImageDeck_Main[i].sprite = dataAllDice.listDice.Find(data => data.id == num).icon;
            }
        }

        private void RefreshGettedDice()
        {
            var isCreated = false;
            if (arrGettedDice == null)
            {
                isCreated = true;
                arrGettedDice = new UI_Getted_Dice[dataAllDice.listDice.Count];
            }
            else if (arrGettedDice.Length < dataAllDice.listDice.Count)
            {
                isCreated = true;
                arrGettedDice = new UI_Getted_Dice[dataAllDice.listDice.Count];
            }

            if (isCreated)
            {
                for (var i = 0; i < dataAllDice.listDice.Count; i++)
                {
                    var obj = Instantiate(prefGettedDice, tsGettedDiceParent);
                    arrGettedDice[i] = obj.GetComponent<UI_Getted_Dice>();
                    arrGettedDice[i].slotNum = i;
                }
            }

            for (var i = 0; i < dataAllDice.listDice.Count; i++)
            {
                arrGettedDice[i].Initialize(dataAllDice.listDice[i]);
            }
            
            // Grid 즉시 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedDiceParent);
            rts_Content.sizeDelta = new Vector2(0, tsGettedDiceParent.sizeDelta.y + 1460 + 300);
        }

        public void Click_Dice_Use(int diceId)
        {
            _isSelectMode = true;
            _selectedDiceId = diceId;
            DeactivateSelectedObjectChild();
            tsGettedDiceParent.gameObject.SetActive(false);
            text_Getted.gameObject.SetActive(false);
            obj_Ciritical.SetActive(false);
            objSelectBlind.SetActive(true);
            objSelectBlind.transform.GetChild(0).GetComponent<Image>().sprite
                = dataAllDice.listDice.Find(data => data.id == diceId).icon;
                //= dataAllDice.listDice[diceId].icon;

            rts_Content.DOAnchorPosY(0, 0.1f);
        }

        public void Click_Dice_Info(int diceId)
        {
            DeactivateSelectedObjectChild();
            ui_Popup_Dice_Info.gameObject.SetActive(true);
            ui_Popup_Dice_Info.Initialize(dataAllDice.listDice.Find(data=>data.id == diceId));
        }

        public void DeactivateSelectedObjectChild()
        {
            BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void Click_Deck(int deckNum)
        {
            if (_isSelectMode)
            {
                var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
                var splitDeck = deck.Split('/');
                var intDeck = new int[5];
                var isChanged = false;
                
                for (var i = 0; i < intDeck.Length; i++) intDeck[i] = int.Parse(splitDeck[i]);
                for (var i = 0; i < intDeck.Length; i++)
                {
                    if (i == deckNum) continue;
                    if (intDeck[i] == _selectedDiceId)
                    {
                        var temp = intDeck[deckNum];
                        intDeck[deckNum] = _selectedDiceId;
                        intDeck[i] = temp;
                        isChanged = true;
                        break;
                    }
                }
                if (!isChanged) intDeck[deckNum] = _selectedDiceId;
                ObscuredPrefs.SetString("Deck", $"{intDeck[0]}/{intDeck[1]}/{intDeck[2]}/{intDeck[3]}/{intDeck[4]}");

                tsGettedDiceParent.gameObject.SetActive(true);
                text_Getted.gameObject.SetActive(true);
                obj_Ciritical.SetActive(true);
                objSelectBlind.SetActive(false);
                _isSelectMode = false;

                RefreshDeck();
            }
        }

        public void HideSelectPanel()
        {
            tsGettedDiceParent.gameObject.SetActive(true);
            text_Getted.gameObject.SetActive(true);
            obj_Ciritical.SetActive(true);
            objSelectBlind.SetActive(false);
        }
    }
}