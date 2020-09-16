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
        public UI_Getted_Dice[] arrGettedDice;
        public GameObject objSelectBlind;
        public RectTransform rts_ScrollView;
        public ScrollRect scrollView;
        public RectTransform rts_Content;
        public Text text_Getted;
        public GameObject obj_Ciritical;

        public Sprite sprite_Use;
        public Sprite sprite_UnUse;

        public Image[] arrImageDeckButton;
        
        [Header("Prefabs")]
        public GameObject prefGettedDice;

        private bool _isSelectMode;
        private int _selectedDiceId;

        
        private void Start()
        {
            RefreshDeck();
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
            
            //scrollView.OnDrag(data => { GetComponentInParent<UI_Main>().OnDrag((PointerEventData)data);});
        }

        private void RefreshDeck()
        {
            //var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var deck = UserInfoManager.Get().GetSelectDeck(active);
            
            var splitDeck = deck.Split('/');

            for (var i = 0; i < arrImageDeck.Length; i++)
            {
                var num = int.Parse(splitDeck[i]);
                var data = JsonDataManager.Get().dataDiceInfo.GetData(num);
                arrImageDeck[i].sprite = FileHelper.GetIcon(data.iconName);//dataAllDice.listDice.Find(data => data.id == num).icon;
                arrImageDeckEye[i].color = FileHelper.GetColor(data.color);
                arrImageDeck_Main[i].sprite = FileHelper.GetIcon(data.iconName);//dataAllDice.listDice.Find(data => data.id == num).icon;
                arrImageDeckEye_Main[i].color = FileHelper.GetColor(data.color);
            }
            
            ui_MainStage.Set();
        }

        private void RefreshGettedDice()
        {
            var isCreated = false;

            int enableCount = 0;
            foreach (KeyValuePair<int,DiceInfoData> info in JsonDataManager.Get().dataDiceInfo.dicData)
            {
                if (info.Value.enableDice == true)
                    enableCount++;
            }
            
            
            if (arrGettedDice == null)
            {
                isCreated = true;
                //arrGettedDice = new UI_Getted_Dice[JsonDataManager.Get().dataDiceInfo.dicData.Count];
                arrGettedDice = new UI_Getted_Dice[enableCount];
            }
            //else if (arrGettedDice.Length < JsonDataManager.Get().dataDiceInfo.dicData.Count)
            else if (arrGettedDice.Length < enableCount)
            {
                isCreated = true;
                arrGettedDice = new UI_Getted_Dice[enableCount];
            }

            if (isCreated)
            {
                //for (var i = 0; i < JsonDataManager.Get().dataDiceInfo.dicData.Count; i++)
                for (var i = 0; i < enableCount; i++)
                {
                    var obj = Instantiate(prefGettedDice, tsGettedDiceParent);
                    arrGettedDice[i] = obj.GetComponent<UI_Getted_Dice>();
                    arrGettedDice[i].slotNum = i;
                }
            }

            //for (var i = 0; i < dataAllDice.listDice.Count; i++)
            //{
                //arrGettedDice[i].Initialize(dataAllDice.listDice[i]);
            //}
            int countindex = 0;
            foreach (KeyValuePair<int, DiceInfoData> info in JsonDataManager.Get().dataDiceInfo.dicData)
            {
                if (info.Value.enableDice)
                {
                    arrGettedDice[countindex].Initialize(info.Value);
                    countindex++;
                }
            }
            
            // Grid 즉시 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate(tsGettedDiceParent);
            rts_Content.sizeDelta = new Vector2(0, tsGettedDiceParent.sizeDelta.y + 1460 + 300);
        }

        public void ResetYPos()
        {
            rts_Content.DOAnchorPosY(0f, 0.1f);
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
                = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.GetData(diceId).iconName);
                //= dataAllDice.listDice.Find(data => data.id == diceId).icon;
                

            rts_Content.DOAnchorPosY(0, 0.1f);
        }

        public void Click_Dice_Info(int diceId)
        {
            DeactivateSelectedObjectChild();
            ui_Popup_Dice_Info.gameObject.SetActive(true);
            //ui_Popup_Dice_Info.Initialize(dataAllDice.listDice.Find(data=>data.id == diceId));
            ui_Popup_Dice_Info.Initialize(JsonDataManager.Get().dataDiceInfo.GetData(diceId));
        }

        public void DeactivateSelectedObjectChild()
        {
            BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void Click_Deck(int deckNum)
        {
            if (_isSelectMode)
            {
                //var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
                int active = UserInfoManager.Get().GetActiveDeckIndex();
                var deck = UserInfoManager.Get().GetSelectDeck(active);
                
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
                
                //ObscuredPrefs.SetString("Deck", $"{intDeck[0]}/{intDeck[1]}/{intDeck[2]}/{intDeck[3]}/{intDeck[4]}");
                UserInfoManager.Get().GetUserInfo()
                    .SetDeck(active, $"{intDeck[0]}/{intDeck[1]}/{intDeck[2]}/{intDeck[3]}/{intDeck[4]}");

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
                    break;
                case 1:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_Use;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    break;
                case 2:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_Use;
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
                    break;
                case 1:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_Use;
                    arrImageDeckButton[2].sprite = sprite_UnUse;
                    break;
                case 2:
                    arrImageDeckButton[0].sprite = sprite_UnUse;
                    arrImageDeckButton[1].sprite = sprite_UnUse;
                    arrImageDeckButton[2].sprite = sprite_Use;
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