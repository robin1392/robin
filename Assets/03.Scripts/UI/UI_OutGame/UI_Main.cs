using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Percent.Platform.InAppPurchase;

using Service.Core;
//using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine.SceneManagement;

namespace ED
{
    public class UI_Main : SingletonDestroy<UI_Main>
    {
        [Header("Main UI")] 
        public Camera camera_Stage;
        public RectTransform rts_MainPages;
        public RectTransform rts_MainContents;
        public RectTransform rts_SafeArea;
        public RectTransform[] arrRts_MainButtons;
        private int currentPageNum = 2;
        private bool isDragging;

        public ScrollRect scrollRect;
        public ScrollRect scrollRectSeasonPass;
        public ScrollRect scrollRectTrophy;
        public UI_ScrollViewEvent ui_ScrollViewEvent;
        public UI_ScrollViewEvent ui_ScrollViewEventSeasonPass;
        public UI_ScrollViewEvent ui_ScrollViewEventTrophy;
        public Button btn_PlayBattle;
        public Button btn_PlayCoop;
        public Button btn_SearchCancel;
        public Button btn_AD;
        public CanvasGroup[] arrSearchingCanvasGroup;
        
        [Header("Popup")]
        public UI_SearchingPopup searchingPopup;
        public UI_BoxPopup boxPopup;
        public UI_BoxOpenPopup boxOpenPopup;
        public GameObject obj_IndicatorPopup;
        public UI_Popup_Userinfo userinfoPopup;
        public UI_Popup_Rank rankPopup;
        public UI_Popup_Quest questPopup;
        public UI_Get_Result gerResult;
        public UI_Popup_SeasonEnd seasonEndPopup;
        public UI_Popup_SeasonStart seasonStartPopup;
        public UI_CommonMessageBox commonMessageBoxPopup;
        public GameObject menuPopup;
        public UI_Popup_SeasonPassUnlock seasonPassUnlockPopup;
        public UI_PopupShopBuy shopBuyPopup;
        public UI_SettingPopup settingPopup;
        public UI_Popup_DailyShopReset dailyShopResetPopup;
        public UI_Popup_MoveShop moveShopPopup;
        
        [Header("User Info")] 
        public Text text_Nickname;
        public Text text_Class;
        public Text text_Trophy;
        public Text text_Diamond;
        public Text text_Gold;
        public Text text_Key;
        public CanvasGroup cg_Money;

        [Header("Badge")]
        public GameObject obj_MenuBadge;
        public GameObject obj_QuestBadge;

        [Space]
        public bool isAIMode;

        [Header("Panals")]
        public UI_Panel_Dice panel_Dice;
        public CanvasGroup[] arrPanels;
        
        private float[] mainPagePosX = {2484, 1242, 0, -1242, -2484};
        private float canvasWidth;

        public override void Awake()
        {
            base.Awake();
            
            Application.targetFrameRate = 60;
        }
        
        private void Start()
        {
            DOTween.Init();

            RefreshUserInfoUI();

            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_LOBBY);

            FirebaseManager.Get().LogEvent("Login");
            
            if (UserInfoManager.Get().GetUserInfo().needSeasonReset)
            {
                ShowMessageBox("시즌 종료", "시즌이 종료되었습니다.", seasonEndPopup.Initialize);
            }

            // TDataShopProductList data;
            // if (TableManager.Get().ShopProductList.GetData(pd => pd.isShow == true, out data))
            // {
            //     Debug.Log(data.googleProductId + ", " + data.appleProductId);
            // }
            
            HideAnotherPanel();

            canvasWidth = ((RectTransform) transform).sizeDelta.x;
            ((RectTransform) arrPanels[0].transform).offsetMin = new Vector2(-canvasWidth * 2,
                ((RectTransform) arrPanels[0].transform).offsetMin.y);
            ((RectTransform) arrPanels[0].transform).offsetMax = new Vector2(-canvasWidth * 2,
                ((RectTransform) arrPanels[0].transform).offsetMin.y);
            canvasWidth = ((RectTransform) transform).sizeDelta.x;
            ((RectTransform) arrPanels[1].transform).offsetMin = new Vector2(-canvasWidth,
                ((RectTransform) arrPanels[1].transform).offsetMin.y);
            ((RectTransform) arrPanels[1].transform).offsetMax = new Vector2(-canvasWidth,
                ((RectTransform) arrPanels[1].transform).offsetMin.y);
            canvasWidth = ((RectTransform) transform).sizeDelta.x;
            ((RectTransform) arrPanels[3].transform).offsetMin = new Vector2(canvasWidth,
                ((RectTransform) arrPanels[3].transform).offsetMin.y);
            ((RectTransform) arrPanels[3].transform).offsetMax = new Vector2(canvasWidth,
                ((RectTransform) arrPanels[3].transform).offsetMin.y);
            canvasWidth = ((RectTransform) transform).sizeDelta.x;
            ((RectTransform) arrPanels[4].transform).offsetMin = new Vector2(canvasWidth * 2,
                ((RectTransform) arrPanels[4].transform).offsetMin.y);
            ((RectTransform) arrPanels[4].transform).offsetMax = new Vector2(canvasWidth * 2,
                ((RectTransform) arrPanels[4].transform).offsetMin.y);
            mainPagePosX = new float[] {canvasWidth * 2, canvasWidth, 0, -canvasWidth, -canvasWidth * 2};
            Click_MainButton(2);
        }

        private void Update()
        {
            btn_AD.interactable = MopubCommunicator.Instance != null && MopubCommunicator.Instance.hasVideo();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (UI_Popup.stack.Count > 0)
                {
                    UI_Popup.ClosePop();
                }
                else
                {
                    commonMessageBoxPopup.Initialize("게임 종료", "게임을 종료 하시겠습니까?", "종료", null, () =>
                    {
                        Application.Quit();
                    });
                }
            }
            
            // popup debug
            // if (UI_Popup.stack.Count > 0)
            // {
            //     Debug.Log($"Popup {UI_Popup.stack.Count} Peek {UI_Popup.stack.Peek().name}");
            // }
            
            // 퀘스트 뱃지 체크
            obj_QuestBadge.SetActive(UI_Popup_Quest.IsCompletedQuest());
            
            // 메뉴버튼 뱃지 체크
            obj_MenuBadge.SetActive(obj_QuestBadge.activeSelf);
        }

        public void RefreshUserInfoUI()
        {
            string nickname = UserInfoManager.Get().GetUserInfo().userNickName;
            text_Nickname.text = nickname;
            text_Trophy.text = UserInfoManager.Get().GetUserInfo().trophy.ToString();
            text_Class.text = $"{Global.g_class} {UserInfoManager.Get().GetUserInfo().nClass}";
            text_Diamond.text = UserInfoManager.Get().GetUserInfo().diamond.ToString();
            text_Gold.text = UserInfoManager.Get().GetUserInfo().gold.ToString();
            text_Key.text = UserInfoManager.Get().GetUserInfo().key.ToString();
        }

        public void Toggle(bool isOn)
        {
            isAIMode = isOn;
        }

        public void Click_PlayBattle()
        {
            FirebaseManager.Get().LogEvent("PlayBattle");

            StopAllCoroutines();
            
            ShowMainUI(false);
            CameraGyroController.Get().FocusIn();

            if (isAIMode || TutorialManager.isTutorial)
            {
                btn_PlayBattle.interactable = false;
                btn_PlayCoop.interactable = false;
                searchingPopup.gameObject.SetActive(true);
                
                StartCoroutine(AIMode());
            }
            else
            {
                // 취소중이라면 none 상태가 될때까지 기다리자...
                //if (WebPacket.Get() != null && WebPacket.Get().netMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
                //{
                    //return;
                //}

                btn_PlayBattle.interactable = false;
                btn_PlayCoop.interactable = false;
                searchingPopup.gameObject.SetActive(true);
                
                ConnectBattle();
            }
        }

        public void Click_PlayCoop()
        {
            //StartCoroutine(ConnectCoop());
            StopAllCoroutines();
            
            ShowMainUI(false);
            CameraGyroController.Get().FocusIn();

            btn_PlayBattle.interactable = false;
            btn_PlayCoop.interactable = false;
            searchingPopup.gameObject.SetActive(true);
                
            ConnectCoop();

        }
        public void Click_BoxButton()
        {
            boxPopup.gameObject.SetActive(true);
            boxPopup.Initialize();
        }

        public void Click_SessonPassButton()
        {
            
        }

        public void Click_RankButton()
        {
            UI_Popup.AllClose();
            rankPopup.Initialize();
        }

        public void Click_MenuButton()
        {
            menuPopup.SetActive(true);
        }

        public void EditNickname(string str)
        {
            UserInfoManager.Get().SetUserNickName(str);
            
            text_Nickname.text = str;
        }
        
        private IEnumerator AIMode()
        {
            yield return new WaitForSeconds(1f);

            GameStateManager.Get().MoveInGameBattle();
        }


        // send battle network
        private void ConnectBattle()
        {
            InGameManager.Get().playType = Global.PLAY_TYPE.BATTLE;
            
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().ConnectServer(Global.PLAY_TYPE.BATTLE, NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, UserInfoManager.Get().GetUserInfo().userID);
                return;
            }

            NetworkManager.Get().StartMatchReq(UserInfoManager.Get().GetUserInfo().userID, (int)Global.PLAY_TYPE.BATTLE);
        }

        private void ConnectCoop()
        {
            InGameManager.Get().playType = Global.PLAY_TYPE.COOP;

            ShowMainUI(false);
            CameraGyroController.Get().FocusIn();
            
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().ConnectServer(Global.PLAY_TYPE.COOP, NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, UserInfoManager.Get().GetUserInfo().userID);
                return;
            }

            NetworkManager.Get().StartMatchReq(UserInfoManager.Get().GetUserInfo().userID, (int)Global.PLAY_TYPE.COOP);
        }
        
        public void Click_DisconnectButton()
        {
            StopAllCoroutines();
            
            //if(WebPacket.Get() != null)
            //    WebPacket.Get().netMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
            if (NetworkManager.Get() != null)
            {
                NetworkManager.Get().NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
            }
            
            btn_PlayBattle.interactable = true;
            btn_PlayCoop.interactable = true;
        }

        public void Click_BoxOpen(int id, UI_BoxOpenPopup.COST_TYPE costType, int cost)
        {
            boxOpenPopup.gameObject.SetActive(true);
            boxOpenPopup.Initialize(id, costType, cost);
        }

        #region Main UI

        private readonly Ease ease = Ease.OutQuint;

        private void HideAnotherPanel()
        {
            for (int i = 0; i < arrPanels.Length; i++)
            {
                arrPanels[i].alpha = i == currentPageNum ? 1f : 0f;
            }

            camera_Stage.enabled = currentPageNum == 2;
        }

        private void ShowAllPanels()
        {
            for (int i = 0; i < arrPanels.Length; i++)
            {
                arrPanels[i].alpha = 1f;
            }

            camera_Stage.enabled = true;
        }

        public void Click_MainButton(int num)
        {
            // cg_Money.gameObject.SetActive(true);
            // cg_Money.DOFade(num == 3 ? 0f : 1f, 0f).OnComplete(() =>
            // {
            //     cg_Money.gameObject.SetActive(num != 3);
            // });

            var rt = (RectTransform) cg_Money.transform;
            rt.DOAnchorPosY(num == 3 ? 500 : -79, 0.1f).SetEase(num == 3 ? Ease.InBack : Ease.OutBack);
            
            ShowAllPanels();
            currentPageNum = num;
            const float duration = 0.3f;
            
            if (currentPageNum == 0) ShopManager.Get().InitShop();

            rts_MainPages.DOAnchorPosX(mainPagePosX[num], duration).SetEase(ease).OnComplete(() =>
            {
                HideAnotherPanel();
            });

            for (var i = 0; i < arrRts_MainButtons.Length; i++)
            {
                //arrRts_MainButtons[i].DOSizeDelta(new Vector2(i == num ? 390f : 213f, 260f), duration).SetEase(ease);
                arrRts_MainButtons[i].DOSizeDelta(new Vector2(i == num ? canvasWidth * 0.36f : canvasWidth * 0.16f, 260f), duration).SetEase(ease);
                //arrRts_MainButtons[i].GetComponent<Image>().DOColor(i == num ? Color.white : Color.gray, duration);
                arrRts_MainButtons[i]
                    .BroadcastMessage(i == num ? "Up" : "Down", SendMessageOptions.DontRequireReceiver);
            }

            panel_Dice.ResetYPos();
            panel_Dice.HideSelectPanel();
            panel_Dice.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void ShowMainUI(bool isShow)
        {
            var fade = 0f;

            if (isShow)
            {
                fade = 1f;
            }
            //
            // var group = rts_MainContents.GetComponent<CanvasGroup>();
            // group.DOFade(fade, 0.2f);
            //
            // group = rts_SafeArea.GetComponent<CanvasGroup>();
            // group.DOFade(fade, 0.2f);

            //var groups = GetComponentsInChildren<CanvasGroup>();
            foreach (var canvasGroup in arrSearchingCanvasGroup)
            {
                canvasGroup.DOFade(fade, 0.2f);
            }
            
            // group = arrRts_MainButtons[0].GetComponentInParent<CanvasGroup>();
            // group.DOFade(fade, 0.2f);

            // var images = rts_MainContents.GetComponentsInChildren<Image>();
            // for (int i = 0; i < images.Length; i++)
            // {
            //     if (images[i].CompareTag("UI_AlphaImage") == false)
            //         images[i].DOFade(fade, 0.2f);
            // }
            //
            // var texts = rts_MainContents.GetComponentsInChildren<Text>();
            // for (int i = 0; i < texts.Length; i++)
            // {
            //     texts[i].DOFade(fade, 0.2f);
            // }
            //
            // images = rts_SafeArea.GetComponentsInChildren<Image>();
            // for (int i = 0; i < images.Length; i++)
            // {
            //     if (images[i].CompareTag("UI_AlphaImage") == false)
            //         images[i].DOFade(fade, 0.2f);
            // }
            //
            // texts = rts_SafeArea.GetComponentsInChildren<Text>();
            // for (int i = 0; i < texts.Length; i++)
            // {
            //     texts[i].DOFade(fade, 0.2f);
            // }

            // for (int j = 0; j < arrRts_MainButtons.Length; ++j)
            // {
            //     images = arrRts_MainButtons[j].GetComponentsInChildren<Image>();
            //     for (int i = 0; i < images.Length; i++)
            //     {
            //         if (images[i].CompareTag("UI_AlphaImage") == false)
            //             images[i].DOFade(fade, 0.2f);
            //     }
            //
            //     texts = arrRts_MainButtons[j].GetComponentsInChildren<Text>();
            //     for (int i = 0; i < texts.Length; i++)
            //     {
            //         texts[i].DOFade(fade, 0.2f);
            //     }
            // }


        }

        private Vector2 _pointerDownPos;
        private Vector2 _mainpageAnchoredPos;

        public void PointerDown(BaseEventData data)
        {
            isDragging = true;
            _pointerDownPos = data.currentInputModule.input.mousePosition;
            _mainpageAnchoredPos = rts_MainPages.anchoredPosition;
            panel_Dice.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void OnBeginDrag(BaseEventData data)
        {
            ShowAllPanels();
            ui_ScrollViewEvent.scrollType = UI_ScrollViewEvent.SCROLL.HORIZONTAL;
            ui_ScrollViewEventSeasonPass.scrollType = UI_ScrollViewEvent.SCROLL.HORIZONTAL;
            ui_ScrollViewEventTrophy.scrollType = UI_ScrollViewEvent.SCROLL.HORIZONTAL;
            isDragging = true;
            _pointerDownPos = data.currentInputModule.input.mousePosition;
            _mainpageAnchoredPos = rts_MainPages.anchoredPosition;
            panel_Dice.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }

        public void OnDrag(BaseEventData data)
        {
            var x = _mainpageAnchoredPos.x +
                    (data.currentInputModule.input.mousePosition.x - _pointerDownPos.x);
            rts_MainPages.anchoredPosition = new Vector2(x, _mainpageAnchoredPos.y);
        }

        public void OnEndDrag(BaseEventData data)
        {
            if (isDragging && ui_ScrollViewEvent.scrollType == UI_ScrollViewEvent.SCROLL.HORIZONTAL)
            {
                scrollRect.enabled = true;
                scrollRectSeasonPass.enabled = true;
                scrollRectTrophy.enabled = true;
                ui_ScrollViewEvent.scrollType = UI_ScrollViewEvent.SCROLL.NONE;
                ui_ScrollViewEventSeasonPass.scrollType = UI_ScrollViewEvent.SCROLL.NONE;
                ui_ScrollViewEventTrophy.scrollType = UI_ScrollViewEvent.SCROLL.NONE;

                if (data.currentInputModule.input.mousePosition.x < _pointerDownPos.x - 100f)
                {
                    if (currentPageNum < 4) Click_MainButton(currentPageNum + 1);
                    else Click_MainButton(currentPageNum);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_UI_SCREEN_SWIPE);
                }
                else if (data.currentInputModule.input.mousePosition.x > _pointerDownPos.x + 100f)
                {
                    if (currentPageNum > 0) Click_MainButton(currentPageNum - 1);
                    else Click_MainButton(currentPageNum);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_UI_SCREEN_SWIPE);
                }
                else
                {
                    Click_MainButton(currentPageNum);
                }
            }
            
            isDragging = false;
        }
        #endregion

        public void Click_AD_Button()
        {
            MopubCommunicator.Instance.showVideo(AD_Callback);
        }

        public void AD_Callback(bool isComplete)
        {
            Debug.Log("AD Finished !!" + isComplete);
        }

        public void Click_Helpshift_Button()
        {
            HelpshiftManager.Get().ShowHelpshift();
        }

        public void Click_Quest_Button()
        {
            UI_Popup.AllClose();
            questPopup.Initialize();
        }

        public void Click_Setting_Button()
        {
            UI_Popup.AllClose();
            settingPopup.gameObject.SetActive(true);
        }

        public void ShowMessageBox(string title, string message, System.Action callback = null)
        {
            commonMessageBoxPopup.Initialize(title, message, callback);
        }

        public void AddReward(ItemBaseInfo[] rewards, Vector3 startPos)
        {
            if (rewards != null)
            {
                List<ItemBaseInfo> list = new List<ItemBaseInfo>();

                foreach (var reward in rewards)
                {
                    var data = new TDataItemList();
                    if (TableManager.Get().ItemList.GetData(reward.ItemId, out data))
                    {
                        switch (data.id)
                        {
                            case 1: // 골드
                                UserInfoManager.Get().GetUserInfo().gold += reward.Value;
                                UI_GetProduction.Get().Initialize(ITEM_TYPE.GOLD, startPos,
                                    Mathf.Clamp(reward.Value, 5, 20));
                                break;
                            case 2: // 다이아
                                UserInfoManager.Get().GetUserInfo().diamond += reward.Value;
                                UI_GetProduction.Get().Initialize(ITEM_TYPE.DIAMOND, startPos,
                                    Mathf.Clamp(reward.Value, 5, 20));
                                break;
                            default: // 주사위
                            {
                                ItemBaseInfo rw = new ItemBaseInfo();
                                rw.ItemId = reward.ItemId;
                                rw.Value = reward.Value;
                                list.Add(rw);
                            }
                                break;
                        }
                    }
                }

                if (list.Count > 0)
                {
                    gerResult.Initialize(list.ToArray(), false, false);
                }
            }
        }
    }
}