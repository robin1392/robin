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
        public RectTransform[] arrRts_MainButtons;
        private int currentPageNum = 2;
        private bool isDragging;

        public ScrollRect scrollRect;
        public UI_ScrollViewEvent ui_ScrollViewEvent;
        public Button btn_PlayBattle;
        public Button btn_PlayCoop;
        public Button btn_SearchCancel;
        public Image image_Progress;
        public Text text_Progress;
        public Text text_Nickname;
        
        [Header("Popup")]
        public UI_SearchingPopup searchPopup;
        public GameObject obj_IndicatorPopup;
        
        [Header("Nicnname")] 
        public InputField inputfield_Nicnname;

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

            string nickname = UserInfoManager.Get().GetUserInfo().userNickName;
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = string.Format("RW{0}", Random.Range(1000, 9999));
                UserInfoManager.Get().SetUserNickName(nickname);
            }
            
            /*string nickname = ObscuredPrefs.GetString("Nickname");
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = string.Format("RW{0}", Random.Range(1000, 9999));
                ObscuredPrefs.SetString("Nickname", nickname);
            }*/

            inputfield_Nicnname.text = nickname;
            text_Nickname.text = nickname;
        }

        public void Toggle(bool isOn)
        {
            isAIMode = isOn;

        }

        public void Click_PlayBattle()
        {
            StopAllCoroutines();
            if (isAIMode)
            {
                btn_PlayBattle.interactable = false;
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
                searchPopup.gameObject.SetActive(true);
                
                ConnectBattle();
            }
        }

        public void EditNickname(string str)
        {
            UserInfoManager.Get().SetUserNickName(str);
            //ObscuredPrefs.SetString("Nickname", str);
            
            text_Nickname.text = str;
        }
        
        private IEnumerator AIMode()
        {
            yield return new WaitForSeconds(1f);

            //SceneManager.LoadScene("InGame_Battle");
            GameStateManager.Get().MoveInGameBattle();
        }


        // send battle network
        private void ConnectBattle()
        {
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().SetAddr(NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, NetworkManager.Get().UserId);
                NetworkManager.Get().ConnectServer(Global.PLAY_TYPE.BATTLE, GameStateManager.Get().ServerConnectCallBack);
                return;
            }

            //WebPacket.Get().SendMatchRequest(UserInfoManager.Get().GetUserInfo().userID , null);
            NetworkManager.Get().StartMatchReq(UserInfoManager.Get().GetUserInfo().userID);
        }
        
        /*
        private IEnumerator ConnectBattle()
        {
            if (PhotonNetwork.IsConnected == false)
                PhotonManager.Instance.Connect();

            while (PhotonManager.Instance.isConnecting == true)
            {
                yield return null;
            }

            PhotonManager.Instance.JoinRoom(PLAY_TYPE.BATTLE);

            while (PhotonNetwork.IsConnected && PhotonNetwork.LevelLoadingProgress <= 0 || PhotonNetwork.LevelLoadingProgress > 0.9f)
            {
                yield return null;
            }

            if (PhotonNetwork.IsConnected)
            {
                btn_Cancel.interactable = false;
                while (true)
                {
                    image_Progress.fillAmount = PhotonNetwork.LevelLoadingProgress;
                    text_Progress.text = $"{(int) (PhotonNetwork.LevelLoadingProgress * 100)}%";
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                btn_Cancel.interactable = false;
                while (PhotonManager.Instance.async != null && PhotonManager.Instance.async.isDone == false)
                {
                    image_Progress.fillAmount = PhotonManager.Instance.async.progress / 0.9f;
                    text_Progress.text = $"{(int) (PhotonManager.Instance.async.progress / 0.9f * 100)}%";
                    yield return null;
                }
            }
        }
        */

        public void Click_PlayCoop()
        {
            //StartCoroutine(ConnectCoop());
        }

        /*
        private IEnumerator ConnectCoop()
        {
            if (PhotonNetwork.IsConnected == false)
                PhotonManager.Instance.Connect();

            while (PhotonNetwork.IsConnected == false || PhotonNetwork.IsConnectedAndReady == false ||
                   PhotonManager.Instance.isConnecting == true)
            {
                yield return null;
            }

            PhotonManager.Instance.JoinRoom(PLAY_TYPE.CO_OP);

            while (PhotonNetwork.LevelLoadingProgress <= 0 || PhotonNetwork.LevelLoadingProgress > 0.9f)
            {
                yield return null;
            }

            btn_Cancel.interactable = false;
            while (true)
            {
                image_Progress.fillAmount = PhotonNetwork.LevelLoadingProgress;
                text_Progress.text = $"{(int) (PhotonNetwork.LevelLoadingProgress * 100)}%";
                yield return new WaitForEndOfFrame();
            }
        }
        */

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
            //PhotonManager.Instance.Disconnect();
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
        
        /*
        #region test
        public void OnClickBtn1()
        {
            if (NetworkManager.Get().UseLocalServer == true)
            {
                NetworkManager.Get().SetAddr(NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, NetworkManager.Get().PlayerSessionId);
                NetworkManager.Get().ConnectServer(PLAY_TYPE.BATTLE, GameStateManager.Get().ServerConnectCallBack);
                return;
            }

            //WebPacket.Get().SendUserAuth( string.Empty , null );
            WebPacket.Get().SendMatchRequest(UserInfoManager.Get().GetUserInfo().userID , null);
        }        
        #endregion
        */
    }
}