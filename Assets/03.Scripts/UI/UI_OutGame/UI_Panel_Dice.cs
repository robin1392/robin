#if UNITY_EDITOR
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
using Template.Character.RandomwarsDice.Common;
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
        public RectTransform tsUngettedDiceParent;
        public RectTransform tsUngettedGuardianParent;
        public List<UI_Getted_Dice> listGettedDice = new List<UI_Getted_Dice>();
        public List<UI_Getted_Dice> listUngettedDice = new List<UI_Getted_Dice>();
        public GameObject objSelectBlind;
        public RectTransform rts_ScrollView;
        public ScrollRect scrollView;
        public RectTransform rts_Content;
        public Text text_BonusHP;
        public GameObject obj_Ciritical;

        public List<UI_DeckInfo> listDeckInfo = new List<UI_DeckInfo>();

        public Text text_GettedDice;
        public Text text_UngettedDice;
        public Text text_GettedGuardian;
        public Text text_UngettedGuardian;
        
        [Header("Prefabs")]
        public GameObject prefGettedDice;

        private bool _isSelectMode;
        private int _selectedDiceId;

        
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
            
            if(UserInfoManager.Get() == null)
                print("user null");
            //scrollView.OnDrag(data => { GetComponentInParent<UI_Main>().OnDrag((PointerEventData)data);});
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
            int bonusHP = 0;


            RandomWarsResource.Data.TDataDiceInfo[] dataDiceInfoArray;
            RandomWarsResource.Data.TDataGuardianInfo[] dataGuardianInfoArray;
            if (TableManager.Get().DiceInfo.GetData( x => x.enableDice, out dataDiceInfoArray) == false)
            {
                return;
            }
            if (TableManager.Get().GuardianInfo.GetData( x => x.enableDice == false, out dataGuardianInfoArray) == false)
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
            
            // Grid 즉시 업데이트
            // LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedDiceParent);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(tsUngettedDiceParent);
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

            text_BonusHP.text = bonusHP.ToString();
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
                DeactivateSelectedObjectChild();
                obj_Ciritical.SetActive(false);
                objSelectBlind.SetActive(true);


                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(diceId, out dataDiceInfo) == false)
                {
                    return;
                }

                objSelectBlind.transform.GetChild(0).GetComponent<Image>().sprite =
                    FileHelper.GetIcon(dataDiceInfo.iconName);

                rts_Content.DOAnchorPosY(0, 0.1f);

                tsGettedDiceParent.gameObject.SetActive(false);
                tsGettedGuardianParent.gameObject.SetActive(false);
                tsUngettedDiceParent.gameObject.SetActive(false);
                tsUngettedGuardianParent.gameObject.SetActive(false);
                text_GettedDice.gameObject.SetActive(false);
                text_UngettedDice.gameObject.SetActive(false);
                text_GettedGuardian.gameObject.SetActive(false);
                text_UngettedGuardian.gameObject.SetActive(false);
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

                //if (WebPacket.Get() != null)
                //{
                //    NetworkManager.Get().UpdateDeckReq(UserInfoManager.Get().GetUserInfo().userID,(sbyte)active, intDeck);
                //    UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                //}
                //else
                //{
                //    UserInfoManager.Get().GetUserInfo().SetDeck(active, intDeck);
                //}
                NetworkManager.session.DiceTemplate.DiceChangeDeckReq(NetworkManager.session.HttpClient, active, intDeck, OnReceiveDiceChangeDeckAck);
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);


                obj_Ciritical.SetActive(true);
                objSelectBlind.SetActive(false);
                _isSelectMode = false;
                
                Click_CancelSelectMode();
                
                //CallBackDeckUpdate();
            }
            else
            {
                Click_Dice_Info(intDeck[deckSlotNum]);
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
            return true;
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
            // int active = UserInfoManager.Get().GetActiveDeckIndex();
            // switch (active)
            // {
            //     case 0:
            //         arrImageDeckButton[0].sprite = sprite_Use;
            //         arrImageDeckButton[1].sprite = sprite_UnUse;
            //         arrImageDeckButton[2].sprite = sprite_UnUse;
            //         arrTextDeckButton[0].color = Color.white;
            //         arrTextDeckButton[1].color = Color.gray;
            //         arrTextDeckButton[2].color = Color.gray;
            //         break;
            //     case 1:
            //         arrImageDeckButton[0].sprite = sprite_UnUse;
            //         arrImageDeckButton[1].sprite = sprite_Use;
            //         arrImageDeckButton[2].sprite = sprite_UnUse;
            //         arrTextDeckButton[0].color = Color.gray;
            //         arrTextDeckButton[1].color = Color.white;
            //         arrTextDeckButton[2].color = Color.gray;
            //         break;
            //     case 2:
            //         arrImageDeckButton[0].sprite = sprite_UnUse;
            //         arrImageDeckButton[1].sprite = sprite_UnUse;
            //         arrImageDeckButton[2].sprite = sprite_Use;
            //         arrTextDeckButton[0].color = Color.gray;
            //         arrTextDeckButton[1].color = Color.gray;
            //         arrTextDeckButton[2].color = Color.white;
            //         break;
            // }
            //
            // RefreshDeck();

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
            text_GettedDice.gameObject.SetActive(true);
            text_GettedGuardian.gameObject.SetActive(true);
            
            bool isUngettedDiceEmpty = tsUngettedDiceParent.childCount == 0;
            text_UngettedDice.gameObject.SetActive(!isUngettedDiceEmpty);
            tsUngettedDiceParent.gameObject.SetActive(!isUngettedDiceEmpty);
            
            bool isUngettedGuardianEmpty = tsUngettedGuardianParent.childCount == 0;
            text_UngettedGuardian.gameObject.SetActive(!isUngettedGuardianEmpty);
            tsUngettedGuardianParent.gameObject.SetActive(!isUngettedGuardianEmpty);
        }
    }
}