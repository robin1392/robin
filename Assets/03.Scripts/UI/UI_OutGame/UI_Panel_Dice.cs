#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using RandomWarsResource.Data;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Template.Character.RandomwarsDice.Common;
using Template.Item.RandomwarsItem.Common;
using Image = UnityEngine.UI.Image;

namespace ED
{
    public class UI_Panel_Dice : MonoBehaviour
    {
        //public Data_AllDice dataAllDice;
        public UI_MainStage ui_MainStage;
        public UI_Popup_Dice_Info ui_Popup_Dice_Info;
        public RectTransform tsGettedDiceParent;
        public RectTransform tsGettedGuardianParent;
        public RectTransform tsGettedEmotionParent;
        public RectTransform tsUngettedDiceParent;
        public RectTransform tsUngettedGuardianParent;
        public RectTransform tsUngettedEmotionParent;
        public List<UI_Getted_Dice> listGettedDice = new List<UI_Getted_Dice>();
        public List<UI_Getted_Dice> listUngettedDice = new List<UI_Getted_Dice>();
        public List<UI_Getted_Emotion> listGettedEmotion = new List<UI_Getted_Emotion>();
        public List<UI_Getted_Emotion> listUngettedEmotion = new List<UI_Getted_Emotion>();
        public GameObject objSelectBlind;
        public Image image_SelectDiceIcon;
        public RectTransform rts_ScrollView;
        public RectTransform rts_ScrollViewGuardian;
        public RectTransform rts_ScrollViewEmotion;
        public ScrollRect scrollView;
        public RectTransform rts_Content;
        public Text text_BonusHP;
        public GameObject obj_Ciritical;

        public List<UI_DeckInfo> listDeckInfo = new List<UI_DeckInfo>();
        public GameObject obj_EmotionDeckInfo;

        public Text text_GettedDice;
        public Text text_UngettedDice;
        public Text text_GettedGuardian;
        public Text text_UngettedGuardian;
        public Text text_GettedEmotion;
        public Text text_UngettedEmotion;
        
        [Header("Prefabs")]
        public GameObject prefGettedDice;
        public GameObject prefGettedEmotion;

        [Header("Emotion")] 
        public Image[] arrImage_EmotionDeck;

        public bool _isSelectMode;
        private int _selectedDiceId;
        private int _selectedEmotionId;

        
        private void Start()
        {
            //RefreshDeck();
            SetActiveDeck();
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
            rts_ScrollViewGuardian.anchorMax = anchorMax;
            rts_ScrollViewEmotion.anchorMax = anchorMax;
            
            if(UserInfoManager.Get() == null)
                print("user null");
            //scrollView.OnDrag(data => { GetComponentInParent<UI_Main>().OnDrag((PointerEventData)data);});
            
            // Emotion Deck
            RefreshEmotionDeck();
        }

        public void RefreshEmotionDeck()
        {
            for (int i = 0; i < UserInfoManager.Get().GetUserInfo().emotionDeck.Count; i++)
            {
                TDataItemList data;
                if (TableManager.Get().ItemList.GetData(UserInfoManager.Get().GetUserInfo().emotionDeck[i], out data))
                {
                    arrImage_EmotionDeck[i].sprite = FileHelper.GetIcon(data.itemIcon);
                }
            }
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
            if (listGettedEmotion.Count > 0)
            {
                for (int i = listGettedEmotion.Count - 1; i >= 0; --i)
                {
                    DestroyImmediate(listGettedEmotion[i].gameObject);
                }
            }
            if (listUngettedEmotion.Count > 0)
            {
                for (int i = listUngettedEmotion.Count - 1; i >= 0; --i)
                {
                    DestroyImmediate(listUngettedEmotion[i].gameObject);
                }
            }
            
            listGettedDice.Clear();
            listUngettedDice.Clear();
            listGettedEmotion.Clear();
            listUngettedEmotion.Clear();
            int gettedSlotCount = 0;
            int ungettedSlotCount = 0;
            int bonusHP = 0;


            RandomWarsResource.Data.TDataDiceInfo[] dataDiceInfoArray;
            RandomWarsResource.Data.TDataGuardianInfo[] dataGuardianInfoArray;
            RandomWarsResource.Data.TDataItemList[] dataEmotionInfoArray;
            if (TableManager.Get().DiceInfo.GetData( x => x.enableDice, out dataDiceInfoArray) == false)
            {
                return;
            }
            if (TableManager.Get().GuardianInfo.GetData( x => x.enableDice == false, out dataGuardianInfoArray) == false)
            {
                return;
            }

            if (TableManager.Get().ItemList.GetData(x => x.id > 7000 && x.id < 8000, out dataEmotionInfoArray) == false)
            {
                return;
            }

            foreach (var info in dataDiceInfoArray)
            {
                if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(info.id))
                {
                    var obj = Instantiate(prefGettedDice, tsGettedDiceParent);
                    var ugd = obj.GetComponent<UI_Getted_Dice>();
                    listGettedDice.Add(ugd);
                    ugd.slotNum = gettedSlotCount++;
                    int level = UserInfoManager.Get().GetUserInfo().dicGettedDice[info.id][0];
                    ugd.Initialize(new InfoData(info), level, UserInfoManager.Get().GetUserInfo().dicGettedDice[info.id][1]);
                    
                    RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                    if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level && x.diceGrade == info.grade, out dataDiceUpgrade) == false)
                    {
                        return;
                    }

                    bonusHP += dataDiceUpgrade.getTowerHp;
                }
                else
                {
                    var obj = Instantiate(prefGettedDice, tsUngettedDiceParent);
                    var ugd = obj.GetComponent<UI_Getted_Dice>();
                    listUngettedDice.Add(ugd);
                    ugd.slotNum = ungettedSlotCount++;
                    ugd.Initialize(new InfoData(info), 0, 0);
                    ugd.SetGrayscale();
                }
            }

            foreach (var info in dataGuardianInfoArray)
            {
                if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(info.id))
                {
                    var obj = Instantiate(prefGettedDice, tsGettedGuardianParent);
                    var ugd = obj.GetComponent<UI_Getted_Dice>();
                    listGettedDice.Add(ugd);
                    ugd.slotNum = gettedSlotCount++;
                    int level = UserInfoManager.Get().GetUserInfo().dicGettedDice[info.id][0];
                    ugd.Initialize(new InfoData(info), level, UserInfoManager.Get().GetUserInfo().dicGettedDice[info.id][1]);
                }
                else
                {
                    var obj = Instantiate(prefGettedDice, tsUngettedGuardianParent);
                    var ugd = obj.GetComponent<UI_Getted_Dice>();
                    listUngettedDice.Add(ugd);
                    ugd.slotNum = ungettedSlotCount++;
                    ugd.Initialize(new InfoData(info), 0, 0);
                    ugd.SetGrayscale();
                }
            }

            foreach (var dataEmotion in dataEmotionInfoArray)
            {
                // 보유중
                if (UserInfoManager.Get().GetUserInfo().emotionIds.Contains(dataEmotion.id))
                {
                    var obj = Instantiate(prefGettedEmotion, tsGettedEmotionParent);
                    var uge = obj.GetComponent<UI_Getted_Emotion>();
                    listGettedEmotion.Add(uge);
                    uge.Initialize(dataEmotion.id);
                }
                // 미보유중
                else
                {
                    var obj = Instantiate(prefGettedEmotion, tsUngettedEmotionParent);
                    var uge = obj.GetComponent<UI_Getted_Emotion>();
                    listUngettedEmotion.Add(uge);
                    uge.Initialize(dataEmotion.id);
                    uge.Deactive();
                }
            }
            
            //
            // if (ungettedSlotCount > 0)
            // {
            //     var pos = tsUngettedDiceParent.anchoredPosition;
            //     pos.y = -980 - (tsGettedDiceParent.sizeDelta.y + 300);
            //     tsUngettedDiceParent.anchoredPosition = pos;
            //     tsUngettedDiceLine.anchoredPosition = new Vector2(0, pos.y + 150);
            //     tsUngettedDiceLine.gameObject.SetActive(true);
            // }
            // else
            // {
            //     tsUngettedDiceLine.gameObject.SetActive(false);
            // }
            //
            //
            // rts_Content.sizeDelta = new Vector2(0, tsGettedDiceParent.sizeDelta.y + tsUngettedDiceParent.sizeDelta.y + 1460 + 300 +
            //                                        (ungettedSlotCount > 0 ? 300 : 0));

            bool isUngettedDiceEmpty = tsUngettedDiceParent.childCount == 0;
            text_UngettedDice.gameObject.SetActive(!isUngettedDiceEmpty);
            tsUngettedDiceParent.gameObject.SetActive(!isUngettedDiceEmpty);
            
            bool isUngettedGuardianEmpty = tsUngettedGuardianParent.childCount == 0;
            text_UngettedGuardian.gameObject.SetActive(!isUngettedGuardianEmpty);
            tsUngettedGuardianParent.gameObject.SetActive(!isUngettedGuardianEmpty);
            
            bool isUngettedEmotionEmpty = tsUngettedEmotionParent.childCount == 0;
            text_UngettedEmotion.gameObject.SetActive(!isUngettedEmotionEmpty);
            tsUngettedEmotionParent.gameObject.SetActive(!isUngettedEmotionEmpty);

            // Grid 즉시 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedDiceParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsUngettedDiceParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedGuardianParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsUngettedGuardianParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedEmotionParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsUngettedEmotionParent);

            text_BonusHP.text = $"<color=#9289e3>보유 효과: </color>타워 HP +{bonusHP}";
            
            RefreshEquipedMarkObject();
        }

        public void ResetYPos()
        {
            rts_Content.DOAnchorPosY(0f, 0.1f);
        }

        public void Click_Dice_Use(int diceId)
        {
            //if (WebPacket.Get() != null && WebPacket.Get().isPacketSend == true)
            //    return;

            _isSelectMode = true;
            _selectedDiceId = diceId;
            
            if (diceId < 5000)
            {
                //DeactivateSelectedObjectChild();
                obj_Ciritical.SetActive(false);
                objSelectBlind.SetActive(true);


                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(diceId, out dataDiceInfo) == false)
                {
                    return;
                }

                image_SelectDiceIcon.sprite = FileHelper.GetIcon(dataDiceInfo.iconName);

                //rts_Content.DOAnchorPosY(0, 0.1f);

                // tsGettedDiceParent.gameObject.SetActive(false);
                // tsGettedGuardianParent.gameObject.SetActive(false);
                // tsUngettedDiceParent.gameObject.SetActive(false);
                // tsUngettedGuardianParent.gameObject.SetActive(false);
                // text_GettedDice.gameObject.SetActive(false);
                // text_UngettedDice.gameObject.SetActive(false);
                // text_GettedGuardian.gameObject.SetActive(false);
                // text_UngettedGuardian.gameObject.SetActive(false);
            }
            else
            {
                Click_Deck(5);
            }

            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }

        public void Click_Dice_Info(int diceId)
        {
            DeactivateSelectedObjectChild();
            //ui_Popup_Dice_Info.Initialize(dataAllDice.listDice.Find(data=>data.id == diceId));

            // 수호자
            if (diceId > 5000)
            {
                RandomWarsResource.Data.TDataGuardianInfo dataGuardianInfo;
                if (TableManager.Get().GuardianInfo.GetData(diceId, out dataGuardianInfo) == false)
                {
                    return;
                }
                
                ui_Popup_Dice_Info.Initialize(dataGuardianInfo);
            }
            else        // 주사위
            {
                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(diceId, out dataDiceInfo) == false)
                {
                    return;
                }

                ui_Popup_Dice_Info.Initialize(dataDiceInfo);
            }


            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }

        public void DeactivateSelectedObjectChild()
        {
            BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void RefreshEquipedMarkObject()
        {
            BroadcastMessage("SetEquipedMark", SendMessageOptions.DontRequireReceiver);
        }

        public void Click_Deck(int deckSlotNum)
        {
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var intDeck = UserInfoManager.Get().GetSelectDeck(active);
            
            if (_isSelectMode)
            {
                var isChanged = false;
                
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

                NetworkManager.session.DiceTemplate.DiceChangeDeckReq(NetworkManager.session.HttpClient, active, intDeck, OnReceiveDiceChangeDeckAck);
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);


                obj_Ciritical.SetActive(true);
                objSelectBlind.SetActive(false);
                _isSelectMode = false;
                
                Click_CancelSelectMode();
            }
            else
            {
                Click_Dice_Info(intDeck[deckSlotNum]);
            }
        }

        public void Click_EmotionUse(int emotionId)
        {
            _isSelectMode = true;
            _selectedEmotionId = emotionId;
            
            DeactivateSelectedObjectChild();
            obj_Ciritical.SetActive(false);
            objSelectBlind.SetActive(true);


            RandomWarsResource.Data.TDataItemList data;
            if (TableManager.Get().ItemList.GetData(emotionId, out data) == false)
            {
                return;
            }

            objSelectBlind.transform.GetChild(0).GetComponent<Image>().sprite =
                FileHelper.GetIcon(data.itemIcon);

            rts_Content.DOAnchorPosY(0, 0.1f);

            tsGettedEmotionParent.gameObject.SetActive(false);
            tsUngettedEmotionParent.gameObject.SetActive(false);
            text_GettedEmotion.gameObject.SetActive(false);
            text_UngettedEmotion.gameObject.SetActive(false);

            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }
        
        public void Click_EmotionDeck(int deckSlotNum)
        {
            if (_isSelectMode)
            {
                var isChanged = false;
                
                for (var i = 0; i < UserInfoManager.Get().GetUserInfo().emotionDeck.Count; i++)
                {
                    if (i == deckSlotNum) continue;
                    if (UserInfoManager.Get().GetUserInfo().emotionDeck[i] == _selectedEmotionId)
                    {
                        var temp = UserInfoManager.Get().GetUserInfo().emotionDeck[deckSlotNum];
                        UserInfoManager.Get().GetUserInfo().emotionDeck[deckSlotNum] = _selectedEmotionId;
                        UserInfoManager.Get().GetUserInfo().emotionDeck[i] = temp;
                        isChanged = true;
                        break;
                    }
                }
                
                if (!isChanged) UserInfoManager.Get().GetUserInfo().emotionDeck[deckSlotNum] = _selectedEmotionId;
                
                NetworkManager.session.ItemTemplate.EmotionEquipReq(NetworkManager.session.HttpClient,
                    UserInfoManager.Get().GetUserInfo().emotionDeck, OnRecieveEmotionChange);
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                
                objSelectBlind.SetActive(false);
                _isSelectMode = false;
                
                Click_CancelSelectMode();
            }
        }

        public bool OnReceiveDiceChangeDeckAck(ERandomwarsDiceErrorCode errorCode, int index, int[] arrayDiceId)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            //RefreshDeck();
            foreach (var deckInfo in listDeckInfo)
            {
                deckInfo.RefreshDiceIcon(true);
            }
            
            UI_Main.Get().panel_Dice.ui_MainStage.Set();
            RefreshEquipedMarkObject();
            return true;
        }

        public bool OnRecieveEmotionChange(ERandomwarsItemErrorCode errorCode, List<int> listItemId)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            if (errorCode == ERandomwarsItemErrorCode.Success)
            {
                UserInfoManager.Get().GetUserInfo().emotionDeck = listItemId;
                RefreshEmotionDeck();
                
                return true;
            }

            return false;
        }

        public void HideSelectPanel()
        {
            Click_CancelSelectMode();
            
            obj_Ciritical.SetActive(true);
            objSelectBlind.SetActive(false);
        }

        #region dice deck select

        public void SetActiveDeck()
        {
            foreach (var deckInfo in listDeckInfo)
            {
                deckInfo.SetActiveDeck();
            }
        }

        #endregion

        public void OnClickDeck(int index)
        {
            // UserInfoManager.Get().SetActiveDeckIndex(index);
            // SetActiveDeck();
        }

        public void Click_CancelSelectMode()
        {
            _isSelectMode = false;
            
            tsGettedDiceParent.gameObject.SetActive(true);
            tsGettedGuardianParent.gameObject.SetActive(true);
            tsGettedEmotionParent.gameObject.SetActive(true);
            text_GettedDice.gameObject.SetActive(true);
            text_GettedGuardian.gameObject.SetActive(true);
            text_GettedEmotion.gameObject.SetActive(true);
            
            bool isUngettedDiceEmpty = tsUngettedDiceParent.childCount == 0;
            text_UngettedDice.gameObject.SetActive(!isUngettedDiceEmpty);
            tsUngettedDiceParent.gameObject.SetActive(!isUngettedDiceEmpty);
            
            bool isUngettedGuardianEmpty = tsUngettedGuardianParent.childCount == 0;
            text_UngettedGuardian.gameObject.SetActive(!isUngettedGuardianEmpty);
            tsUngettedGuardianParent.gameObject.SetActive(!isUngettedGuardianEmpty);

            bool isUngettedEmotionEmpty = tsUngettedEmotionParent.childCount == 0;
            text_UngettedEmotion.gameObject.SetActive(!isUngettedEmotionEmpty);
            tsUngettedEmotionParent.gameObject.SetActive(!isUngettedEmotionEmpty);
        }
    }
}