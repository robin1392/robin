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
        //public Data_AllDice dataAllDice;
        public UI_MainStage ui_MainStage;
        public UI_Popup_Dice_Info ui_Popup_Dice_Info;
        public Image[] arrImageDeck;
        public Image[] arrImageDeckEye;
        public Image[] arrImageDeck_Main;
        public Image[] arrImageDeckEye_Main;
        public RectTransform tsGettedDiceParent;
        public RectTransform tsUngettedDiceParent;
        public RectTransform tsUngettedDiceLine;
        public List<UI_Getted_Dice> listGettedDice = new List<UI_Getted_Dice>();
        public List<UI_Getted_Dice> listUngettedDice = new List<UI_Getted_Dice>();
        public GameObject objSelectBlind;
        public RectTransform rts_ScrollView;
        public ScrollRect scrollView;
        public RectTransform rts_Content;
        public Text text_Getted;
        public Text text_Ungetted;
        public GameObject obj_Ciritical;

        public Sprite sprite_Use;
        public Sprite sprite_UnUse;
        public Sprite sprite_MainUse;
        public Sprite sprite_MainUnUse;

        public Image[] arrImageDeckButton;
        public Image[] arrImageDeckButtonMain;
        public Text[] arrTextDeckButton;
        public Text[] arrTextDeckButtonMain;
        
        [Header("Prefabs")]
        public GameObject prefGettedDice;

        private bool _isSelectMode;
        private int _selectedDiceId;

        
        private void Start()
        {
            //RefreshDeck();
            SetActiveDeck();
            RefreshGettedDice();

            RefreshButton();
            
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
            
            if(UserInfoManager.Get() == null)
                print("user null");
            //scrollView.OnDrag(data => { GetComponentInParent<UI_Main>().OnDrag((PointerEventData)data);});
        }

        private void RefreshDeck()
        {
            //var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var deck = UserInfoManager.Get().GetSelectDeck(active);
            
            //var splitDeck = deck.Split('/');

            for (var i = 0; i < arrImageDeck.Length; i++)
            {
                //var num = int.Parse(splitDeck[i]);
                var data = JsonDataManager.Get().dataDiceInfo.GetData(deck[i]);
                arrImageDeck[i].sprite =
                    FileHelper.GetIcon(data.iconName); //dataAllDice.listDice.Find(data => data.id == num).icon;
                arrImageDeckEye[i].color = FileHelper.GetColor(data.color);
                arrImageDeck_Main[i].sprite =
                    FileHelper.GetIcon(data.iconName); //dataAllDice.listDice.Find(data => data.id == num).icon;
                arrImageDeckEye_Main[i].color = FileHelper.GetColor(data.color);
            }
            
            ui_MainStage.Set();
        }

        public void RefreshGettedDice()
        {
            if (listGettedDice.Count > 0)
            {
                for (int i = listGettedDice.Count - 1; i >= 0; --i)
                {
                    DestroyImmediate(listGettedDice[i].gameObject);
                }
            }
            
            if (listUngettedDice.Count > 0)
            {
                for (int i = listUngettedDice.Count - 1; i >= 0; --i)
                {
                    DestroyImmediate(listUngettedDice[i].gameObject);
                }
            }
            
            listGettedDice.Clear();
            listUngettedDice.Clear();
            int gettedSlotCount = 0;
            int ungettedSlotCount = 0;
            
            foreach (KeyValuePair<int,DiceInfoData> info in JsonDataManager.Get().dataDiceInfo.dicData)
            {
                if (info.Value.enableDice)
                {
                    if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(info.Value.id))
                    {
                        var obj = Instantiate(prefGettedDice, tsGettedDiceParent);
                        var ugd = obj.GetComponent<UI_Getted_Dice>();
                        listGettedDice.Add(ugd);
                        ugd.slotNum = gettedSlotCount++;
                        ugd.Initialize(info.Value, UserInfoManager.Get().GetUserInfo().dicGettedDice[info.Value.id][0], UserInfoManager.Get().GetUserInfo().dicGettedDice[info.Value.id][1]);
                        
                        // obj = Instantiate(prefGettedDice, tsUngettedDiceParent);
                        // ugd = obj.GetComponent<UI_Getted_Dice>();
                        // listUngettedDice.Add(ugd);
                        // ugd.slotNum = ungettedSlotCount++;
                        // ugd.Initialize(info.Value, UserInfoManager.Get().GetUserInfo().dicGettedDice[info.Value.id][0], UserInfoManager.Get().GetUserInfo().dicGettedDice[info.Value.id][1]);
                        // ugd.SetGrayscale();
                    }
                    else
                    {
                        var obj = Instantiate(prefGettedDice, tsUngettedDiceParent);
                        var ugd = obj.GetComponent<UI_Getted_Dice>();
                        listUngettedDice.Add(ugd);
                        ugd.slotNum = ungettedSlotCount++;
                        ugd.Initialize(info.Value, 0, 0);
                        ugd.SetGrayscale();
                    }
                }
            }
            
            // Grid 즉시 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedDiceParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsUngettedDiceParent);

            if (ungettedSlotCount > 0)
            {
                var pos = tsUngettedDiceParent.anchoredPosition;
                pos.y = -980 - (tsGettedDiceParent.sizeDelta.y + 300);
                tsUngettedDiceParent.anchoredPosition = pos;
                tsUngettedDiceLine.anchoredPosition = new Vector2(0, pos.y + 150);
                tsUngettedDiceLine.gameObject.SetActive(true);
            }
            else
            {
                tsUngettedDiceLine.gameObject.SetActive(false);
            }
            

            rts_Content.sizeDelta = new Vector2(0, tsGettedDiceParent.sizeDelta.y + tsUngettedDiceParent.sizeDelta.y + 1460 + 300 +
                                                   (ungettedSlotCount > 0 ? 300 : 0));
        }

        public void ResetYPos()
        {
            rts_Content.DOAnchorPosY(0f, 0.1f);
        }

        public void Click_Dice_Use(int diceId)
        {
            if (WebPacket.Get() != null && WebPacket.Get().isPacketSend == true)
                return;
            
            _isSelectMode = true;
            _selectedDiceId = diceId;
            
            DeactivateSelectedObjectChild();
            tsGettedDiceParent.gameObject.SetActive(false);
            text_Getted.gameObject.SetActive(false);
            obj_Ciritical.SetActive(false);
            objSelectBlind.SetActive(true);
            
            objSelectBlind.transform.GetChild(0).GetComponent<Image>().sprite = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.GetData(diceId).iconName);
            
            rts_Content.DOAnchorPosY(0, 0.1f);
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }

        public void Click_Dice_Info(int diceId)
        {
            DeactivateSelectedObjectChild();
            ui_Popup_Dice_Info.gameObject.SetActive(true);
            //ui_Popup_Dice_Info.Initialize(dataAllDice.listDice.Find(data=>data.id == diceId));
            ui_Popup_Dice_Info.Initialize(JsonDataManager.Get().dataDiceInfo.GetData(diceId));

            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }

        public void DeactivateSelectedObjectChild()
        {
            BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void Click_Deck(int deckSlotNum)
        {
            if (_isSelectMode)
            {
                //var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
                int active = UserInfoManager.Get().GetActiveDeckIndex();
                //var deck = UserInfoManager.Get().GetSelectDeck(active);
                
                //var splitDeck = deck.Split('/');
                var intDeck = UserInfoManager.Get().GetSelectDeck(active);
                var isChanged = false;
                
                //for (var i = 0; i < intDeck.Length; i++) intDeck[i] = int.Parse(splitDeck[i]);
                for (var i = 0; i < intDeck.Length; i++)
                {
                    if (i == deckSlotNum) continue;
                    if (intDeck[i] == _selectedDiceId)
                    {
                        var temp = intDeck[deckSlotNum];
                        intDeck[deckSlotNum] = _selectedDiceId;
                        intDeck[i] = temp;
                        isChanged = true;
                        break;
                    }
                }
                if (!isChanged) intDeck[deckSlotNum] = _selectedDiceId;
                
                //
                if (WebPacket.Get() != null)
                {
                    //WebPacket.Get().SendDeckUpdateRequest( active ,intDeck , CallBackDeckUpdate );
                    NetworkManager.Get().UpdateDeckReq(UserInfoManager.Get().GetUserInfo().userID,(sbyte)active, intDeck);
                    UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                }
                else
                {
                    //ObscuredPrefs.SetString("Deck", $"{intDeck[0]}/{intDeck[1]}/{intDeck[2]}/{intDeck[3]}/{intDeck[4]}");
                    //UserInfoManager.Get().GetUserInfo().SetDeck(active, $"{intDeck[0]}/{intDeck[1]}/{intDeck[2]}/{intDeck[3]}/{intDeck[4]}");
                    UserInfoManager.Get().GetUserInfo().SetDeck(active, intDeck);
                }

                tsGettedDiceParent.gameObject.SetActive(true);
                text_Getted.gameObject.SetActive(true);
                obj_Ciritical.SetActive(true);
                objSelectBlind.SetActive(false);
                _isSelectMode = false;

                RefreshDeck();
            }
        }

        public void CallBackDeckUpdate()
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            RefreshDeck();
        }

        public void HideSelectPanel()
        {
            tsGettedDiceParent.gameObject.SetActive(true);
            text_Getted.gameObject.SetActive(true);
            obj_Ciritical.SetActive(true);
            objSelectBlind.SetActive(false);
        }

        #region dice deck select

        public void SetActiveDeck()
        {
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            switch (active)
            {
                case 0:
                    arrImageDeckButton[0].sprite = sprite_Use;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUnUse;
                    arrTextDeckButton[0].color = Color.white;
                    arrTextDeckButton[1].color = Color.gray;
                    arrTextDeckButton[2].color = Color.gray;
                    arrTextDeckButtonMain[0].color = Color.white;
                    arrTextDeckButtonMain[1].color = Color.gray;
                    arrTextDeckButtonMain[2].color = Color.gray;
                    break;
                case 1:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_Use;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUnUse;
                    arrTextDeckButton[0].color = Color.gray;
                    arrTextDeckButton[1].color = Color.white;
                    arrTextDeckButton[2].color = Color.gray;
                    arrTextDeckButtonMain[0].color = Color.gray;
                    arrTextDeckButtonMain[1].color = Color.white;
                    arrTextDeckButtonMain[2].color = Color.gray;
                    break;
                case 2:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_Use;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUse;
                    arrTextDeckButton[0].color = Color.gray;
                    arrTextDeckButton[1].color = Color.gray;
                    arrTextDeckButton[2].color = Color.white;
                    arrTextDeckButtonMain[0].color = Color.gray;
                    arrTextDeckButtonMain[1].color = Color.gray;
                    arrTextDeckButtonMain[2].color = Color.white;
                    break;
            }
            
            RefreshDeck();
        }

        public void RefreshButton()
        {
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            switch (active)
            {
                case 0:
                    arrImageDeckButton[0].sprite = sprite_Use;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUnUse;
                    break;
                case 1:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_Use;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUnUse;
                    break;
                case 2:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_Use;
                    arrImageDeckButtonMain[0].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[1].sprite = sprite_MainUnUse;
                    arrImageDeckButtonMain[2].sprite = sprite_MainUse;
                    break;
            }
        }
        #endregion

        public void OnClickDeck(int index)
        {
            UserInfoManager.Get().SetActiveDeckIndex(index);
            SetActiveDeck();
        }

        public void Click_CancelSelectMode()
        {
            _isSelectMode = false;
        }
    }
}