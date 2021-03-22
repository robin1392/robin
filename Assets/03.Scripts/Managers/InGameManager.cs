﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MirageTest.Aws;
using MirageTest.Scripts;
using Service.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ED
{
    public class InGameManager : SingletonDestroy<InGameManager>
    {
        #region game system variable

        [Header("SYSTEN INFO")] 
        public Global.PLAY_TYPE playType;

        public Transform ts_Lights;
        public Transform ts_StadiumTop;
        public Transform ts_NexusHealthBar;

        public float time { get; protected set; }
        protected DateTime pauseTime;
        #endregion
        
        
        #region static

        public static bool IsNetwork;
        
        #endregion
        
        
        #region unity base

        public override void Awake()
        {
            base.Awake();

            // Application.targetFrameRate = 30;

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnEditorAppPause;
#endif
        }

        public override void OnDestroy()
        {
            
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= OnEditorAppPause;
#endif

            DestroyManager();

            base.OnDestroy();
        }

        protected virtual void Start()
        {
            // 개발 버전이라..중간에서 실행햇을시에..
            if (DataPatchManager.Get().isDataLoadComplete == false)
                DataPatchManager.Get().JsonDownLoad();

            // 위치를 옮김.. 차후 데이터 로딩후 풀링을 해야되서....
            PoolManager.Get().MakePool();
            
            Debug.Log(InGameManager.Get().playType);
            Debug.Log(NetworkManager.Get().UseLocalServer);
            
            StartManager();

            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_INGAME_BATTLE);
        }

        #endregion


        #region init destroy

        public void DestroyManager()
        {
            //KZSee:
            // arrUpgradeLevel = null;
            //
            // event_SP_Edit.RemoveAllListeners();
            // event_SP_Edit = null;

        }
        
        public virtual void StartManager()
        {
            var matchInfo = NetworkManager.Get().LastMatchInfo;
            NetworkManager.Get().LastMatchInfo = null;
            IsNetwork =  matchInfo != null;
            if (IsNetwork)
            {
                StartMatchGame(matchInfo).Forget();
            }
            else
            {
                StartFakeGame().Forget();
            }
            
            UI_InGame.Get().ViewTargetDice(false);

            //KZSee:AStarPathFinding MapScan
            //Invoke("MapScan", 1f);
        }

        public void RotateTopCampObject()
        {
            ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
            ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
            ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
        }
        
        async UniTask StartMatchGame(NetworkManager.MatchInfo matchInfo)
        {
            if (TableManager.Get().Loaded == false)
            {
                string targetPath = Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "Dev");
                TableManager.Get().LoadFromFile(targetPath);
            }
            
            var server = FindObjectOfType<RWNetworkServer>();
            server.enabled = false;
            
            var userInfo = UserInfoManager.Get().GetUserInfo();
            var client = FindObjectOfType<RWNetworkClient>();
            
            client.LocalUserId = userInfo.userID;
            client.LocalNickName = userInfo.userNickName;
            client.PlayerSessionId = matchInfo.PlayerGameSession;
            
            await client.RWConnectAsync(matchInfo.ServerAddress, (ushort)matchInfo.Port);
        }

        async UniTask StartFakeGame()
        {
            if (TableManager.Get().Loaded == false)
            {
                string targetPath = Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "DEV");
                TableManager.Get().LoadFromFile(targetPath);
            }
            
            var server = FindObjectOfType<RWNetworkServer>();
            var userInfo = UserInfoManager.Get().GetUserInfo();

            var userDeck = userInfo.GetActiveDeck;
            var diceDeck = userDeck.Take(5).ToArray();
            var guadianId = userDeck[5];
            server.serverGameLogic.isAIMode = true;
            server.MatchData.AddPlayerInfo(
                userInfo.userID, 
                userInfo.userNickName, 0, 0,
                new DeckInfo(guadianId, diceDeck));
            server.MatchData.AddPlayerInfo(
                "AI", 
                "AI", 0, 0,
                new DeckInfo(guadianId, GetAIDeck(TutorialManager.isTutorial)));

            server.authenticator = null;
            var client = FindObjectOfType<RWNetworkClient>();
            client.authenticator = null;
            client.LocalUserId = userInfo.userID;
            client.LocalNickName = userInfo.userNickName;

            server.Listening = false;
            await server.StartHost(client);
        }

        public int[] GetAIDeck(bool isTutorial)
        {
            if (isTutorial)
            {
                return new int[] {1000, 1002, 3011, 3003, 3005};
            }
            
            var diceInfos = TableManager.Get().DiceInfo.Values;
            var arr = diceInfos.Where(info => info.enableDice).ToArray();
            var shuffled = arr.OrderBy(x => Guid.NewGuid()).ToList();
            return shuffled.Take(5).Select(info => info.id).ToArray();
        }
        #endregion
        

        #region leave & end game

        public void LeaveRoom()
        {
            if (IsNetwork)
            {
                var client = FindObjectOfType<RWNetworkClient>();
                client.GetLocalPlayerProxy().GiveUp();
                GameStateManager.Get().MoveMainScene();
            }
            else
            {
                FindObjectOfType<RWNetworkServer>().Finalize();
                FindObjectOfType<RWNetworkClient>().Disconnect();
                GameStateManager.Get().MoveMainScene();
            }
            
            Time.timeScale = 1f;
        }

        // 내자신이 나간다고 눌럿을때 응답 받은것
        public void CallBackLeaveRoom()
        {
            // 잠시 테스트로 주석
            //NetworkManager.Get().DeleteBattleInfo();
            GameStateManager.Get().MoveMainScene();
        }


        /// <summary>
        /// 상대방이 나갓다고 noti를 받앗을 경우
        /// </summary>
        public void OnOtherLeft(int userUid)
        {
            // 플레이 도중 나갓을경우...
            // 나중에 플레이어가 여러명일 경우 해당 플레이어만 초기화 해주는 로직이 필요하긴하다
            // 나도 나가자
            //KZSee:
            // if (isGamePlaying)
            // {
            //     isGamePlaying = false;
            //     SendInGameManager(GameProtocol.LEAVE_GAME_REQ);
            // }
        }


        public void EndGame(MatchReport result)
        {
            UI_InGamePopup.Get().SetViewWaiting(false);
            
            // 인디케이터도 다시 안보이게..
            if (UI_InGamePopup.Get().IsIndicatorActive() == true)
            {
                UI_InGamePopup.Get().ViewGameIndicator(false);
            }
            
            StopAllCoroutines();
            SoundManager.instance?.StopBGM();
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);
            UI_InGame.Get().ClearUI();

            StartCoroutine(EndGameCoroutine(result));
        }

        //KZSee: 결과처리
        IEnumerator EndGameCoroutine(MatchReport result)
        {
            yield return new WaitForSeconds(4f);

            //KZSee: 이벤트로그
            //playerController.SendEventLog_BatCheck();

            UI_InGamePopup.Get().SetPopupResult(true, result.WinLose, result.IsPerfect, result.WinStreak, result.NormalRewards, result.StreakRewards, result.PerfectRewards);

            SoundManager.instance.Play(result.WinLose ? Global.E_SOUND.BGM_INGAME_WIN : Global.E_SOUND.BGM_INGAME_LOSE);
        }
        #endregion


        #region net etc system

        private RWNetworkClient _client;

        public void InitClient(RWNetworkClient client)
        {
            _client = client;
        }
        
        public void Click_SP_Upgrade_Button()
        {
            var localPlayerProxy = _client.GetLocalPlayerProxy(); 
            localPlayerProxy.UpgradeSp();

            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_SP_LEVEL_UP);
        }
        
        #endregion
        
        
        #region pause , resume , sync 
        
        private void RevmoeAllMinionAndMagic()
        {
            // playerController.RemoveAllMinionAndMagic();
            // playerController.targetPlayer.RemoveAllMinionAndMagic();
        }


        public void OnApplicationPause(bool pauseStatus)
        {
            var networkManager = NetworkManager.Get();
            if (networkManager == null)
            {
                return;
            }
            
            networkManager.PrintNetworkStatus();

            if (pauseStatus)
            {
                pauseTime = DateTime.UtcNow;
                print("Application Pause");
                NetworkManager.Get().PauseGame();
            }
            else
            {
                
                time -= (float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds;
                print("Application Resume : " + ((float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds));
            }
        }

#if UNITY_EDITOR
        // 에디터에서 테스트용도로 사용하기 위해
        public void OnEditorAppPause(PauseState pause)
        {
            NetworkManager.Get()?.PrintNetworkStatus();

            if (pause == PauseState.Paused)
            {
                pauseTime = DateTime.UtcNow;
                print("Application Pause");
                NetworkManager.Get().PauseGame();
            }
            else
            {
                time -= (float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds;
                print("Application Resume : " + ((float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds));
            }
        }
#endif
        #endregion

        public void ShowAIField(bool isShow)
        {
            //KZSee: 에이아이모드에서 사용중 확인 필요
            // if (isShow)
            // {
            //     playerController.uiDiceField.SetField(playerController.targetPlayer.arrDice);
            //     playerController.uiDiceField.RefreshField(0.5f);
            //     
            //     UI_InGame.Get().SetArrayDeck(playerController.targetPlayer.arrDiceDeck, playerController.targetPlayer.arrUpgradeLevel);
            //     int count = UI_InGame.Get().arrUpgradeButtons.Length;
            //     for (int i = 0; i < count; i++)
            //     {
            //         UI_InGame.Get().arrUpgradeButtons[i].SetIconAlpha(0.5f);
            //     }
            //     StartCoroutine(nameof(ShowAiFieldCoroutine));
            // }
            // else
            // {
            //     StopCoroutine(nameof(ShowAiFieldCoroutine));
            //     playerController.uiDiceField.SetField(playerController.arrDice);
            //     playerController.uiDiceField.RefreshField();
            //     
            //     UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, playerController.arrUpgradeLevel);
            //     int count = UI_InGame.Get().arrUpgradeButtons.Length;
            //     for (int i = 0; i < count; i++)
            //     {
            //         UI_InGame.Get().arrUpgradeButtons[i].SetIconAlpha(1f);
            //     }
            // }
        }

        private IEnumerator ShowAiFieldCoroutine()
        {
            //KZSee: 에이아이모드에서 사용중 확인 필요
            // while (true)
            // {
            //     playerController.uiDiceField.SetField(playerController.targetPlayer.arrDice);
            //     playerController.uiDiceField.RefreshField(0.5f);
            //     
            //     UI_InGame.Get().SetArrayDeck(playerController.targetPlayer.arrDiceDeck, playerController.targetPlayer.arrUpgradeLevel);
            //     int count = UI_InGame.Get().arrUpgradeButtons.Length;
            //     for (int i = 0; i < count; i++)
            //     {
            //         UI_InGame.Get().arrUpgradeButtons[i].SetIconAlpha(0.5f);
            //     }
            //     yield return null;
            // }
            yield return null;
        }
    }
}
