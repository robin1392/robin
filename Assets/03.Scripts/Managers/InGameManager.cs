#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using Service.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;


using CodeStage.AntiCheat.ObscuredTypes;
using MirageTest.Scripts;
using Pathfinding;
using RandomWarsResource.Data;
using UnityEngine.U2D;
using ITEM_TYPE = RandomWarsProtocol.ITEM_TYPE;
using Random = UnityEngine.Random;

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
        
        public RandomWarsResource.TableData<int, RandomWarsResource.Data.TDataDiceInfo> data_DiceInfo;
        
        public GameObject pref_Player;
        public GameObject pref_AI;
        public Transform ts_Lights;
        public Transform ts_StadiumTop;
        public Transform ts_NexusHealthBar;
        
        public bool isAIMode;
        public bool isGamePlaying;

        public PlayerController playerController;

        private int _readyPlayerCount = 0;
        
        public UnityEvent<int> event_SP_Edit;

        #endregion

        #region wave variable

        public int wave
        {
            get;
            protected set;
        }

        public float startSpawnTime = 10f;
        public float spawnTime = 45f;

        protected float st => wave < 1 ? 10f : 5f;

        public float time { get; protected set; }
        protected DateTime pauseTime;
        #endregion

        #region etc variable
        
        private UI_CoopSpawnTurn _coopSpawnTurn;

        #endregion

        
        #region static
        public static bool IsNetwork
        {
            get
            {
                return true;
                if (NetworkManager.Get() == null)
                    return false;
                
                if (NetworkManager.Get() != null && NetworkManager.Get().IsConnect())
                    return true;

                return false;
            }
        }
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

            // 전체 주사위 정보
            data_DiceInfo = TableManager.Get().DiceInfo;

            // 위치를 옮김.. 차후 데이터 로딩후 풀링을 해야되서....
            PoolManager.Get().MakePool();


            StartManager();

            // not use
            /*             
            if (PhotonNetwork.IsConnected)
            {
                SendBattleManager(RpcTarget.Others, E_PTDefine.PT_NICKNAME, ObscuredPrefs.GetString("Nickname"));
            }
            else
            {
                UI_InGame.Get().SetNickname("AI");
            }
            */
            
            SoundManager.instance.PlayBGM(Global.E_SOUND.BGM_INGAME_BATTLE);
        }

        protected void Update()
        {
            RefreshTimeUI();
            if (UI_InGame.Get() != null)
                UI_InGame.Get().SetUnitCount(_client.ActorProxies.Count - _client.Towers.Count);
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
            if (IsNetwork == true)
            {
                UI_InGamePopup.Get().SetViewWaiting(true);

                // player controller create...my and other
                Vector3 myTowerPos = FieldManager.Get().GetPlayerPos(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);
                GameObject myTObj = UnityUtil.Instantiate("Tower/" + pref_Player.name);
                myTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);
                myTObj.transform.position = myTowerPos;
                playerController = myTObj.GetComponent<PlayerController>();
                playerController.isMine = true;
                
                playerController.ChangeLayer(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);


                Vector3 otherTowerPos = FieldManager.Get().GetPlayerPos(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
                GameObject otherTObj = UnityUtil.Instantiate("Tower/" + pref_Player.name);
                otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
                otherTObj.transform.position = otherTowerPos;
                //KZSee:
                // playerController.targetPlayer = otherTObj.GetComponent<PlayerController>();
                // playerController.targetPlayer.isMine = false;
                //
                // playerController.targetPlayer.targetPlayer = playerController;
                //
                // playerController.targetPlayer.ChangeLayer(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer, true);
                
                //
                UI_InGame.Get().SetNickName(NetworkManager.Get().GetNetInfo().playerInfo.Name , NetworkManager.Get().GetNetInfo().otherInfo.Name);

            }
            else
            {
                Vector3 startPos = FieldManager.Get().GetPlayerPos(true);
                var obj = Instantiate(pref_Player, startPos, Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(true);
                playerController = obj.GetComponent<PlayerController>();
                playerController.ChangeLayer(true);
                playerController.isMine = true;
                
                
                // Set MsgUserinfo
                var msgUserInfo = new MsgPlayerInfo();
                msgUserInfo.PlayerUId = 1;
                msgUserInfo.IsBottomPlayer = true;
                msgUserInfo.IsMaster = true;
                msgUserInfo.Name = UserInfoManager.Get().GetUserInfo().userNickName;
                msgUserInfo.CurrentSp = 200;
                int bonusHP = 0;


                RandomWarsResource.Data.TDataDiceInfo[] dataDiceInfoArray;
                if (TableManager.Get().DiceInfo.GetData( x => x.enableDice, out dataDiceInfoArray ) == false)
                {
                    return;
                }

                foreach (var info in dataDiceInfoArray)
                {
                    if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(info.id))
                    {
                        int level = UserInfoManager.Get().GetUserInfo().dicGettedDice[info.id][0];

                        RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                        if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level && x.diceGrade == info.grade, out dataDiceUpgrade) == false)
                        {
                            return;
                        }

                        bonusHP += dataDiceUpgrade.getTowerHp;
                    }
                }
                msgUserInfo.TowerHp = (30000 + bonusHP) * 100;
                msgUserInfo.SpGrade = 0;
                msgUserInfo.GetDiceCount = 0;
                msgUserInfo.DiceIdArray = UserInfoManager.Get().GetUserInfo().GetActiveDeck;
                msgUserInfo.DiceLevelArray = new short[5]
                {
                    ConvertNetMsg.MsgIntToShort(UserInfoManager.Get().GetUserInfo().dicGettedDice[msgUserInfo.DiceIdArray[0]][0]), 
                    ConvertNetMsg.MsgIntToShort(UserInfoManager.Get().GetUserInfo().dicGettedDice[msgUserInfo.DiceIdArray[1]][0]), 
                    ConvertNetMsg.MsgIntToShort(UserInfoManager.Get().GetUserInfo().dicGettedDice[msgUserInfo.DiceIdArray[2]][0]), 
                    ConvertNetMsg.MsgIntToShort(UserInfoManager.Get().GetUserInfo().dicGettedDice[msgUserInfo.DiceIdArray[3]][0]), 
                    ConvertNetMsg.MsgIntToShort(UserInfoManager.Get().GetUserInfo().dicGettedDice[msgUserInfo.DiceIdArray[4]][0])
                }; 
                NetworkManager.Get().GetNetInfo().SetPlayerInfo(msgUserInfo);
                
                // Set AI
                msgUserInfo = new MsgPlayerInfo();
                msgUserInfo.PlayerUId = 2;
                msgUserInfo.IsBottomPlayer = false;
                msgUserInfo.IsMaster = false;
                msgUserInfo.CurrentSp = 200;
                msgUserInfo.TowerHp = (30000 + Random.Range(0, 500)) * 100;
                msgUserInfo.Name = "AI";
                NetworkManager.Get().GetNetInfo().SetOtherInfo(msgUserInfo);

                GameObject otherTObj = Instantiate(pref_AI, FieldManager.Get().GetPlayerPos(false), Quaternion.identity);
                otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(false);

                // name
                UI_InGame.Get().SetNickName(UserInfoManager.Get().GetUserInfo().userNickName, "AI");
                otherTObj.SendMessage("ChangeLayer", false);
                otherTObj.SendMessage("AI_SetRandomDeck", false);
                isAIMode = true;
            }
            
            
            UI_InGame.Get().ViewTargetDice(!IsNetwork);

            event_SP_Edit.AddListener(RefreshSP);
            event_SP_Edit.AddListener(SetSPUpgradeButton);


            //
            if (NetworkManager.Get().isReconnect)
            {
                UI_InGamePopup.Get().ViewGameIndicator(true);
                
                SendInGameManager(GameProtocol.READY_SYNC_GAME_REQ);
                return;
            }
            

            // deck setting
            if (IsNetwork == true)
            {
                // my
                for(int i = 0 ; i < NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray.Length; i++)
                    print(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray[i]);
                
                //플레이어스테이트를 통한 동기화
                // playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                // //other
                // playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            else
            {
                // 네트워크 안쓰니까...개발용으로
                //var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
                if (UserInfoManager.Get() != null)
                {
                    //플레이어스테이트를 통한 동기화
                    // var deck = UserInfoManager.Get().GetActiveDeck();
                    // playerController.SetDeck(deck);
                }

            }

            //Mirage => PlayerState 클라이언트 동기화 로직으로 이동
            // UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, arrUpgradeLevel);
            // UI_InGame.Get().SetEnemyArrayDeck();
            // UI_InGame.Get().SetEnemyUpgrade();

            if (IsNetwork == true)
            {
                if (NetworkManager.Get().IsMaster == false)
                {
                    ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
                    ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
                    ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
                }
            }

            if (IsNetwork == true)
            {
                WorldUIManager.Get().SetWave(wave);
                SendInGameManager(GameProtocol.READY_GAME_REQ);
            }
            else
            {
                StartGame();
                RefreshTimeUI(true);
            }

            Invoke("MapScan", 1f);
        }
        
        protected void RefreshSP(int sp)
        {
            UI_InGame.Get().SetSP(sp);
        }

        protected void SetSPUpgradeButton(int sp)
        {
            var spGrade = FindObjectOfType<RWNetworkClient>().GetLocalPlayerState().spGrade;
            UI_InGame.Get().SetSPUpgrade(spGrade, sp);
        }


        private void MapScan()
        {
            AstarPath.active.Scan();
        }

        #endregion


        #region start game

        public void NetStartGame()
        {
            DeactivateWaitingObject();

            // start...
            StartGame();

            RefreshTimeUI(true);
        }


        protected void StartGame()
        {
            if (IsNetwork == true)
            {
                //KZSee:
                //StartCoroutine(SpawnLoop());
            }
            else if (IsNetwork == false)
            {

                Debug.Log("StartGame: OfflineMode");
                // 네트워크 연결 안됫으니 deactive...   
                DeactivateWaitingObject();
                //KZSee:
                // StartCoroutine(SpawnLoop());
            }
        }
        
        private void Ready()
        {
            _readyPlayerCount++;
        }

        #endregion
        

        #region sp & spawn

        private void DeactivateWaitingObject()
        {
            isGamePlaying = true;
            UI_InGamePopup.Get().SetViewWaiting(false);
        }
        
        protected void RefreshTimeUI(bool isImmediately = false)
        {
            if (isImmediately)
            {
                WorldUIManager.Get().SetSpawnTime(1f - time % st / st);
            }
            else
            {
                float ff = Mathf.Lerp(WorldUIManager.Get().GetSpawnAmount(), 1f - time % st / st, Time.deltaTime * 5.0f);

                if (ff > WorldUIManager.Get().GetSpawnAmount())
                    WorldUIManager.Get().SetSpawnTime(ff);
                else 
                    WorldUIManager.Get().SetSpawnTime(1f - time % st / st);
            }
            
       
            WorldUIManager.Get().SetTextSpawnTime(time);
            
            if (IsNetwork == false)
                WorldUIManager.Get().SetWave(wave);
        }

        #endregion

        #region leave & end game

        public void LeaveRoom()
        {
            if (IsNetwork == true)
            {
                // 자신이 포기한경우 게임중을 꺼주자
                if (isGamePlaying)
                {
                    isGamePlaying = false;
                }

                Time.timeScale = 1f;
                SendInGameManager(GameProtocol.LEAVE_GAME_REQ);
            }
            else
            {
                GameStateManager.Get().MoveMainScene();
            }
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
            if (isGamePlaying)
            {
                isGamePlaying = false;
                SendInGameManager(GameProtocol.LEAVE_GAME_REQ);
            }
        }


        public void EndGame(bool winLose, int winningStreak, ItemBaseInfo[] normalReward, ItemBaseInfo[] streakReward, ItemBaseInfo[] perfectReward)
        {
            // 게임이 끝낫으니까 그냥..
            if (NetworkManager.Get().isResume == true)
            {
                NetworkManager.Get().SetResume(false);
                //
                UI_InGamePopup.Get().SetViewWaiting(false);
            }
            // 인디케이터도 다시 안보이게..
            if (UI_InGamePopup.Get().IsIndicatorActive() == true)
            {
                UI_InGamePopup.Get().ViewGameIndicator(false);
            }

            isGamePlaying = false;
            StopAllCoroutines();
            SoundManager.instance?.StopBGM();
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);
            UI_InGame.Get().ClearUI();

            StartCoroutine(EndGameCoroutine(winLose, winningStreak, normalReward, streakReward, perfectReward));
        }

        //KZSee: 결과처리
        IEnumerator EndGameCoroutine(bool winLose, int winningStreak, ItemBaseInfo[] normalReward, ItemBaseInfo[] streakReward, ItemBaseInfo[] perfectReward)
        {
            yield return new WaitForSeconds(4f);

            playerController.SendEventLog_BatCheck();

            UI_InGamePopup.Get().SetPopupResult(true, winLose, winningStreak, normalReward, streakReward, perfectReward);

            SoundManager.instance.Play(winLose ? Global.E_SOUND.BGM_INGAME_WIN : Global.E_SOUND.BGM_INGAME_LOSE);
        }
        #endregion


        #region net etc system

        private RWNetworkClient _client;

        public void InitClient(RWNetworkClient _client)
        {
            _client = _client;
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
                if (isGamePlaying == false)
                    return;

                time -= (float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds;
                print("Application Resume : " + ((float)DateTime.UtcNow.Subtract(pauseTime).TotalSeconds));
                ResumeDelay();
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
                if (isGamePlaying == false)
                    return;
                
                ResumeDelay();
            }

        }
#endif
        void ResumeDelay()
        {
            /*
            // resume 신호 -- player controll 에서 혹시 모를 릴레이 패킷들 다 패스 시키기위해
            NetworkManager.Get().SetResume(true);
            
            // resume 을 하는 client 라면..
            // 인디케이터 -- 어차피 재동기화 위해 데이터 날려야됨
            UI_InGamePopup.Get().ViewGameIndicator(true);

            //yield return new WaitForSeconds(2.0f);
            RevmoeAllMinionAndMagic();
            */

            if (NetworkManager.Get().IsConnect())
            {
                // resume
                NetworkManager.Get().ResumeGame();

                // 1초 동안 NavMeshAgent를 사용하지 않고 즉시 이동하도록
                // playerController.SyncMinionResume();
                // playerController.targetPlayer.SyncMinionResume();
            }
        }

        public void SyncInfo()
        {
            // deck setting
            if (IsNetwork == true)
            {
                // my
                for(int i = 0 ; i < NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray.Length; i++)
                    print(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray[i]);
                
                //KZSee:
                // playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                // //other
                // playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            
            // Upgrade buttons
            //Mirage : PlayerState로 이동
            // UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, arrUpgradeLevel);

            if (IsNetwork == true)
            {
                if (NetworkManager.Get().IsMaster == false)
                {
                    ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
                    ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
                    ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
                }
            }
            
        }
        #endregion
        


        // 매니저 외부에서 패킷을 보낼때 쓰자..

        #region outter send

        public void SendDiceLevelUp(int resetFieldNum, int levelUpFieldNum)
        {
            SendInGameManager(GameProtocol.MERGE_DICE_REQ, (short) resetFieldNum, (short) levelUpFieldNum);
        }

        public void SendInGameUpgrade(int diceId, int slotNum)
        {
            SendInGameManager(GameProtocol.INGAME_UP_DICE_REQ, diceId);
        }

        #endregion
        
        public void SendInGameManager(GameProtocol protocol, params object[] param)
        {
            
            if (IsNetwork == true)
            {
                NetworkManager.Get().Send(protocol, param);
            }
            // 패킷프로토콜이 틀리기때문에 쓸모없슴...
            /*else
            {
                RecvInGameManager(protocol, param);
            }*/
        }

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

        #region not use old code
        // photon remove
        /*
        #region rpc etc

        // not use
        public void SpawnPlayerMinions()
        {
            WorldUIManager.Get().SetSpawnTime(1.0f);
            playerController.SendPlayer(RpcTarget.All, E_PTDefine.PT_SPAWN);
        }

        // 내가 언제 이런 함수를 만들엇지....???!!???
        public void ShowAIField(bool isShow)
        {
            if (isShow)
            {
                playerController.uiDiceField.SetField(playerController.targetPlayer.arrDice);
                playerController.uiDiceField.RefreshField(0.5f);
                StartCoroutine(nameof(ShowAiFieldCoroutine));
            }
            else
            {
                StopCoroutine(nameof(ShowAiFieldCoroutine));
                playerController.uiDiceField.SetField(playerController.arrDice);
                playerController.uiDiceField.RefreshField();
            }
        }

        private IEnumerator ShowAiFieldCoroutine()
        {
            while (true)
            {
                playerController.uiDiceField.SetField(playerController.targetPlayer.arrDice);
                playerController.uiDiceField.RefreshField(0.5f);
                yield return null;
            }
        }


        #endregion

        
        #region photon override
        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(time);
                stream.SendNext(wave);
            }
            else
            {
                time = (float) stream.ReceiveNext();
                wave = (int) stream.ReceiveNext();
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);

            GameStateManager.Get().MoveMainScene();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (isGamePlaying)
            {
                PhotonNetwork.Disconnect();
            }
        }
        #endregion
        

        #region photon send recv
        public void SendBattleManager(RpcTarget target, E_PTDefine ptID, params object[] param)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(recvMessage, target, ptID, param);
            }
            else
            {
                RecvBattleManager(ptID, param);
            }
        }

        [PunRPC]
        public void RecvBattleManager(E_PTDefine ptID, params object[] param)
        {
            switch (ptID)
            {
                case E_PTDefine.PT_READY:
                    Ready();
                    break;
                case E_PTDefine.PT_DEACTIVEWAIT:
                    DeactivateWaitingObject();
                    break;
                case E_PTDefine.PT_ADDSP:
                    int addsp = (int) param[0];
                    AddSP(addsp);
                    break;
                case E_PTDefine.PT_SPAWNMINION:
                    SpawnPlayerMinions();
                    break;
                case E_PTDefine.PT_ENDGAME:
                    EndGame(new PhotonMessageInfo());
                    break;
                case E_PTDefine.PT_NICKNAME:
                    UI_InGame.Get().SetNickname((string) param[0]);
                    break;
            }
        }
        #endregion
        */
        #endregion
    }
}
