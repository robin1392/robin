#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.Resourcing;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using MirageTest.Scripts;
using Photon;
using Quantum;
using Quantum.Commands;
using Quantum.Demo;
using RandomWarsResource;
using RandomWarsResource.Data;
using Service.Core;
using UnityEngine;
using UnityEngine.Networking;
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

        #endregion
        
        
        #region static

        public static bool IsNetwork;
        
        #endregion
        
        
        #region unity base


        public override void OnDestroy()
        {
            base.OnDestroy();
            global::Pool.Disable();
        }

        protected virtual void Start()
        {
            global::Pool.Enable();
            
            QuantumEvent.Subscribe<EventGameOver>(listener: this, handler: OnGameOver);
            // 개발 버전이라..중간에서 실행햇을시에..
            if (DataPatchManager.Get().isDataLoadComplete == false)
                DataPatchManager.Get().JsonDownLoad();

            Debug.Log(playType);

            StartManager();

            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_INGAME_BATTLE);
        }

        private void OnGameOver(EventGameOver callback)
        {
            var reports = MatchResult.GetResult(callback.Game.Frames.Verified, MatchData);
            var localNick = UserInfoManager.Get().GetUserInfo().userNickName;
            var report = reports.First(r => r.Nick == localNick );
            
            UI_InGamePopup.Get().SetViewWaiting(false);
            if (UI_InGamePopup.Get().IsIndicatorActive() == true)
            {
                UI_InGamePopup.Get().ViewGameIndicator(false);
            }
        
            UI_InGamePopup.Get().ShowLowHPEffect(false);
            SoundManager.instance.StopBGM();
            UI_InGame.Get().ClearUI();
            EndGame(Global.PLAY_TYPE.BATTLE, 
                MatchData.PlayerInfos.Find(p => p.NickName == localNick), 
                MatchData.PlayerInfos.Find(p => p.NickName != localNick), 
                report);
        }

        #endregion


        #region init destroy

        public virtual void StartManager()
        {
            if(PhotonNetwork.Instance.Online)
            {
                StartMatchGame().Forget();
            }
            else
            {
                StartFakeGame().Forget();
            }
            
            UI_InGame.Get().ViewTargetDice(playType == PLAY_TYPE.CO_OP);
        }

        public void RotateTopCampObject()
        {
            ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
            ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
        }
        
        async UniTask StartMatchGame()
        {
            if (TableManager.Get().Loaded == false)
            {
                TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
            }
            
            CameraController.Get().UpdateCameraRotation(true);
            
            var userInfo = UserInfoManager.Get().GetUserInfo();

            var userDeck = userInfo.GetActiveDeck;
            var diceDeck = userDeck.Take(5).ToArray();
            var guadianId = userDeck[5];

            if ((PhotonNetwork.Instance.Online && PhotonNetwork.Instance.LocalBalancingClient.InRoom) == false)
            {
                UnityEngine.Debug.LogError($"연결이 끊어졌습니다.");
                return;
            }

            var localMatchPlayer = new MatchPlayer()
            {
                UserId = userInfo.userID,
                NickName = userInfo.userNickName,
                Trophy = 0,
                WinStreak = 0,
                Deck = new DeckInfo(guadianId, diceDeck, userInfo.GetOutGameLevels(diceDeck)),
                EnableAI = false
            };

            var other = PhotonNetwork.Instance.LocalBalancingClient.CurrentRoom.Players.First(p => p.Value.IsLocal == false);
            var otherMatchPlayer = MatchPlayer.CreateFromPlayerCustomProperty(other.Value.CustomProperties);

            MatchData = new MatchData(localMatchPlayer, otherMatchPlayer);

            await UniTask.Yield();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.Realtime);
            
            UI_InGamePopup.Get().InitUIElement(localMatchPlayer, otherMatchPlayer);

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), DelayType.Realtime);
            
            var address = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ1YZA4IzGzT26a2Kk3NmCiUpcTJdb4ZBlH-t92rIT6McMFe5b7NnQBkv0aULsov_8XjNHG56aO_GrY/pub?gid=0&single=true&output=csv";
            var text = (await UnityWebRequest.Get(address).SendWebRequest()).downloadHandler.text;
            Debug.Log(text);
            Debug.Log($"DiceInfo 개발용 로드: {TableManager.Get().DiceInfo.Init(text)}");
            
            var preloadResources = GetPreoloadResources(MatchData);
            await PreloadedResourceManager.Preload(preloadResources);

            var quantumRunner = FindObjectOfType<RWQuantumRunnerDebug>();
            quantumRunner.Players = new[]
            {
                ToRuntimePlayer(localMatchPlayer),
            };

            PhotonNetwork.Instance.Ready();
            
            while (PhotonNetwork.Instance.State != PhotonNetwork.StateType.Started)
            {
                await UniTask.Yield();
            }
            
            var config = new RuntimeConfig();
            config.Map.Id = PhotonNetwork.Instance.GetMapGuid();

            var param = new QuantumRunner.StartParameters {
                RuntimeConfig       = config,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                GameMode            = Photon.Deterministic.DeterministicGameMode.Multiplayer,
                PlayerCount         = PhotonNetwork.Instance.LocalBalancingClient.CurrentRoom.MaxPlayers,
                LocalPlayerCount    = 1,
                NetworkClient       = PhotonNetwork.Instance.LocalBalancingClient,
            };

            var clientId = ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, PhotonNetwork.Instance.LocalBalancingClient);
            QuantumRunner.StartGame(clientId, param);
            
            while (QuantumRunner.Default.Game.Frames?.Verified?.IsGameStarted() == false)
            {
                await UniTask.Yield();
            }

            while (QuantumRunner.Default.Game.GetLocalPlayers().Length < 1)
            {
                await UniTask.Yield();
            }

            CameraController.Get().UpdateCameraRotation(QuantumRunner.Default.Game.GetLocalPlayers()[0] == 0);

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), DelayType.Realtime);

            UI_InGamePopup.Get().DisableStartPopup();
            
            Debug.LogFormat("### Starting game using map '{0}'", config.Map.Id);
        }

        async UniTask StartFakeGame()
        {
            PhotonNetwork.Instance.Online = false;
            
            if (TableManager.Get().Loaded == false)
            {
                TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
            }
            
            CameraController.Get().UpdateCameraRotation(true);
            
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
            
            var address = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ1YZA4IzGzT26a2Kk3NmCiUpcTJdb4ZBlH-t92rIT6McMFe5b7NnQBkv0aULsov_8XjNHG56aO_GrY/pub?gid=0&single=true&output=csv";
            var text = (await UnityWebRequest.Get(address).SendWebRequest()).downloadHandler.text;
            Debug.Log(text);
            Debug.Log($"DiceInfo 개발용 로드: {TableManager.Get().DiceInfo.Init(text)}");
            
            var preloadResources = GetPreoloadResources(MatchData);
            await PreloadedResourceManager.Preload(preloadResources);

            var quantumRunner = FindObjectOfType<RWQuantumRunnerDebug>();
            
            quantumRunner.Players = new[]
            {
                ToRuntimePlayer(localMatchPlayer),
                ToRuntimePlayer(aiMatchPlayer)
            };
            
            quantumRunner.StartWithFrame();
            
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), DelayType.Realtime);

            UI_InGamePopup.Get().DisableStartPopup();
        }

        private IEnumerable<string> GetPreoloadResources(MatchData matchData)
        {
            var preoloadResouces = new HashSet<string>()
            {
                AssetNames.EffectStun,
                AssetNames.EffectTaunted,
                AssetNames.EffectIceState,
                AssetNames.EffectHalfDamage,
                AssetNames.TowerBlue,
                AssetNames.TowerRed,
                AssetNames.EffectSpawnLine,
                AssetNames.EffectIceFreeze,
                AssetNames.IceBullet,
                AssetNames.ProjectileWind,
                AssetNames.DiceStateIceEffect,
            };

            var diceInfos = TableManager.Get().DiceInfo;
            foreach (var playerInfo in matchData.PlayerInfos)
            {
                foreach (var deckDice in playerInfo.Deck.DiceInfos)
                {
                    diceInfos.GetData(deckDice.DiceId, out var diceInfo);
                    preoloadResouces.Add(diceInfo.prefabName);
                }
            }

            return preoloadResouces;
        }

        RuntimePlayer ToRuntimePlayer(MatchPlayer matchPlayer)
        {
            return new RuntimePlayer()
            {
                Nickname = matchPlayer.NickName,
                DeckDiceIds = matchPlayer.Deck.DiceInfos.Select(i => i.DiceId).ToArray(),
                DeckDiceOutGameLevels = matchPlayer.Deck.DiceInfos.Select(i => i.OutGameLevel).ToArray(),
                GuardianId = matchPlayer.Deck.GuardianId,
                IsBot = matchPlayer.EnableAI,
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
                    EnableAI = true,
                };
            }

            return new MatchPlayer()
            {
                NickName = "AI",
                Deck = userMatchPlayer.Deck,
                EnableAI = true,
            };
        }
        #endregion
        

        #region leave & end game

        public void OnClickGiveUp()
        {
            QuantumRunner.Default.Shutdown();
            PhotonNetwork.Instance.LocalBalancingClient.Disconnect();
            MoveToMainScene();
        }

        // 내자신이 나간다고 눌럿을때 응답 받은것
        public void OnClickExit()
        {
            DisconnectGameServer();
            MoveToMainScene();
        }

        public void MoveToMainScene()
        {
            global::Pool.Disable();
            GameStateManager.Get().MoveMainScene();
            Time.timeScale = 1f;
        }

        public void DisconnectGameServer()
        {
            if (PhotonNetwork.Instance.LocalBalancingClient.IsConnected)
            {
                QuantumRunner.Default.Shutdown();
                PhotonNetwork.Instance.LocalBalancingClient.Disconnect();
            }
            
            MoveToMainScene();
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
            QuantumRunner.Default.Game.SendCommand(new CommingSpUpgradeCommand());
        }
        
        #endregion
        
        
        #region pause , resume , sync 
        
        private void RevmoeAllMinionAndMagic()
        {
            // playerController.RemoveAllMinionAndMagic();
            // playerController.targetPlayer.RemoveAllMinionAndMagic();
        }
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
