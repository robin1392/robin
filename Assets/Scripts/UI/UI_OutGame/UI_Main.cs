using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace ED
{
    public class UI_Main : SingletonDestroy<UI_Main>
    {
        [Header("Main UI")] 
        public RectTransform rts_MainPages;
        public RectTransform rts_MainContents;
        public RectTransform rts_SafeArea;
        public RectTransform[] arrRts_MainButtons;
        private int currentPageNum = 2;
        private bool isDragging;

        public ScrollRect scrollRect;
        public UI_ScrollViewEvent ui_ScrollViewEvent;
        public Button btn_PlayBattle;
        public Button btn_PlayCoop;
        public Button btn_SearchCancel;
        
        [Header("Popup")]
        public UI_SearchingPopup searchPopup;
        public UI_BoxPopup boxPopup;
        public UI_BoxOpenPopup boxOpenPopup;
        public GameObject obj_IndicatorPopup;
        public UI_Popup_Userinfo userinfoPopup;
        public UI_Popup_Rank rankPopup;
        
        [Header("User Info")] 
        public InputField inputfield_Nicnname;
        public Text text_Nickname;
        public Text text_Class;
        public Text text_Trophy;
        public Text text_Diamond;
        public Text text_Gold;
        public Text text_Key;

        [Space]
        public bool isAIMode;

        [Header("Panals")] 
        public UI_Panel_Dice panel_Dice;
        
        private readonly int[] mainPagePosX = {2484, 1242, 0, -1242, -2484};

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
        }

        public void RefreshUserInfoUI()
        {
            string nickname = UserInfoManager.Get().GetUserInfo().userNickName;
            inputfield_Nicnname.text = nickname;
            text_Nickname.text = nickname;
            text_Trophy.text = UserInfoManager.Get().GetUserInfo().trophy.ToString();
            text_Class.text = $"클래스 {UserInfoManager.Get().GetUserInfo().nClass}";
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

            if (isAIMode)
            {
                btn_PlayBattle.interactable = false;
                btn_PlayCoop.interactable = false;
                searchPopup.gameObject.SetActive(true);
                
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
                searchPopup.gameObject.SetActive(true);
                
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
            searchPopup.gameObject.SetActive(true);
                
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
            rankPopup.gameObject.SetActive(true);
            rankPopup.Initialize();
        }

        public void Click_MenuButton()
        {
            userinfoPopup.gameObject.SetActive(true);
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
            NetworkManager.Get().playType = Global.PLAY_TYPE.BATTLE;
            
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().ConnectServer(Global.PLAY_TYPE.BATTLE, NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, NetworkManager.Get().UserId);
                return;
            }

            NetworkManager.Get().StartMatchReq(UserInfoManager.Get().GetUserInfo().userID);
        }

        private void ConnectCoop()
        {
            NetworkManager.Get().playType = Global.PLAY_TYPE.COOP;
            
            ShowMainUI(false);
            CameraGyroController.Get().FocusIn();
            
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().ConnectServer(Global.PLAY_TYPE.COOP, NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, NetworkManager.Get().UserId);
                return;
            }

            NetworkManager.Get().StartMatchReq(UserInfoManager.Get().GetUserInfo().userID);
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

        public void Click_MainButton(int num)
        {
            currentPageNum = num;
            const float duration = 0.3f;

            rts_MainPages.DOAnchorPosX(mainPagePosX[num], duration).SetEase(ease);

            for (var i = 0; i < arrRts_MainButtons.Length; i++)
            {
                arrRts_MainButtons[i].DOSizeDelta(new Vector2(i == num ? 390f : 213f, 260f), duration).SetEase(ease);
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
            
            var images = rts_MainContents.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].CompareTag("UI_AlphaImage") == false)
                    images[i].DOFade(fade, 0.2f);
            }

            var texts = rts_MainContents.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].DOFade(fade, 0.2f);
            }

            images = rts_SafeArea.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].CompareTag("UI_AlphaImage") == false)
                    images[i].DOFade(fade, 0.2f);
            }

            texts = rts_SafeArea.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].DOFade(fade, 0.2f);
            }

            for (int j = 0; j < arrRts_MainButtons.Length; ++j)
            {
                images = arrRts_MainButtons[j].GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    if (images[i].CompareTag("UI_AlphaImage") == false)
                        images[i].DOFade(fade, 0.2f);
                }

                texts = arrRts_MainButtons[j].GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].DOFade(fade, 0.2f);
                }
            }
            
            
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
            ui_ScrollViewEvent.scrollType = UI_ScrollViewEvent.SCROLL.HORIZONTAL;
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
                ui_ScrollViewEvent.scrollType = UI_ScrollViewEvent.SCROLL.NONE;

                if (data.currentInputModule.input.mousePosition.x < _pointerDownPos.x - 100f)
                {
                    if (currentPageNum < 4) Click_MainButton(currentPageNum + 1);
                    else Click_MainButton(currentPageNum);
                }
                else if (data.currentInputModule.input.mousePosition.x > _pointerDownPos.x + 100f)
                {
                    if (currentPageNum > 0) Click_MainButton(currentPageNum - 1);
                    else Click_MainButton(currentPageNum);
                }
                else
                {
                    Click_MainButton(currentPageNum);
                }
            }
            
            isDragging = false;
        }
        #endregion
    }
}