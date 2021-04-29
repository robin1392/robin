#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using MirageTest.Scripts;
using Quantum;
using Service.Core;
using UnityEngine;
using DeckInfo = _Scripts.DeckInfo;
using MatchData = _Scripts.MatchData;
using MatchPlayer = _Scripts.MatchPlayer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ED
{
    public class InGameManager : SingletonDestroy<InGameManager>
    {
        private MatchData MatchData;
        
        #region game system variable

        [Header("SYSTEN INFO")] 
        public PLAY_TYPE playType;

        public Transform ts_Lights;
        public Transform ts_StadiumTop;

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
            
            UI_InGame.Get().ViewTargetDice(playType == PLAY_TYPE.CO_OP);

            //KZSee:AStarPathFinding MapScan
            //Invoke("MapScan", 1f);
        }

        public void RotateTopCampObject()
        {
            ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
            ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
        }
        
        async UniTask StartMatchGame(NetworkManager.MatchInfo matchInfo)
        {
            if (TableManager.Get().Loaded == false)
            {
                string targetPath = Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "Dev");
                TableManager.Get().LoadFromFile(targetPath);
            }
            
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
                TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
            }
            
            var userInfo = UserInfoManager.Get().GetUserInfo();

            var userDeck = userInfo.GetActiveDeck;
            var diceDeck = userDeck.Take(5).ToArray();
            var guadianId = userDeck[5];
            
            if (TutorialManager.isTutorial)
            {
            }
            else
            {
                
            }

            var localMatchPlayer = new MatchPlayer()
            {
                NickName = userInfo.userNickName,
                Trophy = 0,
                WinStreak = 0,
                Deck = new DeckInfo(guadianId, diceDeck, userInfo.GetOutGameLevels(diceDeck)),
                EnableAI = false
            };
            
            var aiMatchPlayer = GetAIMatchPlayer(localMatchPlayer, TutorialManager.isTutorial);

            MatchData = new MatchData(localMatchPlayer, aiMatchPlayer);

            await UniTask.Yield();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.Realtime);
            
            UI_InGamePopup.Get().InitUIElement(localMatchPlayer, aiMatchPlayer);

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), DelayType.Realtime);
            
            var quantumRunner = FindObjectOfType<RWQuantumRunner>();
            
            quantumRunner.Players = new[]
            {
                ToRuntimePlayer(localMatchPlayer),
                ToRuntimePlayer(aiMatchPlayer)
            };

            quantumRunner.StartWithFrame();
            
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), DelayType.Realtime);

            UI_InGamePopup.Get().DisableStartPopup();
        }
        
        RuntimePlayer ToRuntimePlayer(MatchPlayer matchPlayer)
        {
            return new RuntimePlayer()
            {
                Nickname = matchPlayer.NickName,
                DeckDiceIds = matchPlayer.Deck.DiceInfos.Select(i => i.DiceId).ToArray(),
                DeckDiceOutGameLevels = matchPlayer.Deck.DiceInfos.Select(i => i.OutGameLevel).ToArray(),
                GuardianId = matchPlayer.Deck.GuardianId,
            };
        }
        
        public MatchPlayer GetAIMatchPlayer(MatchPlayer userMatchPlayer, bool isTutorial)
        {
            if (isTutorial)
            {
                return new MatchPlayer()
                {
                    NickName = "AI",
                    Deck = new DeckInfo(
                        userMatchPlayer.Deck.GuardianId, 
                        new int[] {31001, 31003, 31002, 32002, 32009}, 
                        new int[]{0,0,0,0,0}),
                };
            }

            return new MatchPlayer()
            {
                NickName = "AI",
                Deck = userMatchPlayer.Deck,
            };
        }
        #endregion
        

        #region leave & end game

        public void OnClickGiveUp()
        {
            if (IsNetwork)
            {
                var client = FindObjectOfType<RWNetworkClient>();
                client.GiveUp();
            }
            else
            {
                FindObjectOfType<RWNetworkServer>().FinalizeServer();
                FindObjectOfType<RWNetworkClient>().Disconnect();
            }
            
            GameStateManager.Get().MoveMainScene();
            
            Time.timeScale = 1f;
        }

        // 내자신이 나간다고 눌럿을때 응답 받은것
        public void OnClickExit()
        {
            DisconnectGameServer();
            MoveToMainScene();
        }

        public void MoveToMainScene()
        {
            GameStateManager.Get().MoveMainScene();
            Time.timeScale = 1f;
        }

        public void DisconnectGameServer()
        {
            if (IsNetwork)
            {
                var client = FindObjectOfType<RWNetworkClient>();
                client.Disconnect();
            }
            else
            {
                FindObjectOfType<RWNetworkServer>().FinalizeServer();
                FindObjectOfType<RWNetworkClient>().Disconnect();
            }
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


        public void EndGame(Global.PLAY_TYPE playType, MatchPlayer local, MatchPlayer other, MatchReport result)
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

            StartCoroutine(EndGameCoroutine(playType, local, other, result));
        }

        //KZSee: 결과처리
        IEnumerator EndGameCoroutine(Global.PLAY_TYPE playType, MatchPlayer localPlayer, MatchPlayer otherPlayer, MatchReport result)
        {
            yield return new WaitForSeconds(4f);

            //KZSee: 이벤트로그
            //playerController.SendEventLog_BatCheck();
            
            UI_InGamePopup.Get().SetPopupResult(playType, localPlayer, otherPlayer, true, result.WinLose, result.WinStreak, result.IsPerfect, result.NormalRewards, result.StreakRewards, result.PerfectRewards, result.LoseReward);

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
            if (_client == null || _client.IsConnected == false)
            {
                return;
            }
            
            var localPlayerProxy = _client.GetLocalPlayerProxy(); 
            localPlayerProxy.UpgradeSp();
            
            UI_InGame.Get().spUpgradeAnimator.SetTrigger("Fx_SP_Upgrade");

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
            
            //TODO: 네트워크 상태 확인 복구
            // networkManager.PrintNetworkStatus();

            if (pauseStatus)
            {
                pauseTime = DateTime.UtcNow;
                print("Application Pause");
                
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
            //TODO: 네트워크 상태 확인 복구
            // NetworkManager.Get()?.PrintNetworkStatus();

            if (pause == PauseState.Paused)
            {
                pauseTime = DateTime.UtcNow;
                print("Application Pause");
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
            if (isShow)
            {
                var enemyPlayerState = _client.GetEnemyPlayerState();
                var tableManager = TableManager.Get();
                var diceArr = enemyPlayerState.Field.Select(f =>
                {
                    tableManager.DiceInfo.GetData(f.diceId, out var diceInfo);
                    return new Dice()
                    {
                        diceFieldNum = f.index,
                        diceData = diceInfo
                    };
                }).ToArray();

                UI_DiceField.Get().SetField(diceArr);
                UI_DiceField.Get().RefreshField(0.5f);

                var deckArr = enemyPlayerState.Deck.Select(d =>
                {
                    TableManager.Get().DiceInfo.GetData(d.diceId, out var diceInfo);
                    return (diceInfo, d.inGameLevel);
                }).ToArray();
                
                UI_InGame.Get().SetArrayDeck(deckArr);
                int count = UI_InGame.Get().arrUpgradeButtons.Length;
                for (int i = 0; i < count; i++)
                {
                    UI_InGame.Get().arrUpgradeButtons[i].SetIconAlpha(0.5f);
                }
                
                _client.BindDeckUI(enemyPlayerState.userId);
            }
            else
            {
                var localPlayerState = _client.GetLocalPlayerState();
                var tableManager = TableManager.Get();
                var diceArr = localPlayerState.Field.Select(f =>
                {
                    tableManager.DiceInfo.GetData(f.diceId, out var diceInfo);
                    return new Dice()
                    {
                        diceFieldNum = f.index,
                        diceData = diceInfo
                    };
                }).ToArray();
                
                UI_DiceField.Get().SetField(diceArr);
                UI_DiceField.Get().RefreshField();

                var deckArr = localPlayerState.Deck.Select(d =>
                {
                    TableManager.Get().DiceInfo.GetData(d.diceId, out var diceInfo);
                    return (diceInfo, d.inGameLevel);
                }).ToArray();
                
                UI_InGame.Get().SetArrayDeck(deckArr);
                int count = UI_InGame.Get().arrUpgradeButtons.Length;
                for (int i = 0; i < count; i++)
                {
                    UI_InGame.Get().arrUpgradeButtons[i].SetIconAlpha(0.5f);
                }
                
                _client.BindDeckUI(localPlayerState.userId);
            }
        }
    }
}
