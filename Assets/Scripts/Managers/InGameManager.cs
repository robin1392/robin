#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;


using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ED
{
    public class InGameManager : SingletonDestroy<InGameManager> //, IPunObservable
    {
        //ßpublic static InGameManager Instance;

        #region game system variable

        [Header("SYSTEN INFO")] 
        public Global.PLAY_TYPE playType;

        //public Data_AllDice data_AllDice;
        // new all dice info
        public DiceInfo data_DiceInfo;


        public GameObject pref_Player;
        public GameObject pref_AI;
        public Transform ts_Lights;
        public Transform ts_StadiumTop;
        public Transform ts_NexusHealthBar;

        // --> field manager
        //public Transform ts_TopPlayer;
        //public Transform ts_BottomPlayer;

        public bool isAIMode;
        public bool isGamePlaying;

        public PlayerController playerController;

        private int _readyPlayerCount = 0;

        #endregion

        #region wave variable

        public int wave
        {
            get;
            protected set;
        }

        public float startSpawnTime = 10f;
        public float spawnTime = 45f;

        protected float st => wave < 1 ? 10f : 20f;

        public float time { get; protected set; }
        protected DateTime pauseTime;

        #endregion

        #region dice variable

        [Header("DICE INFO")] 
        [SerializeField] 
        protected int[] arrUpgradeLevel;

        //public int getDiceCost => 10 + getDiceCount * 10;
        public int getDiceCount = 0;

        #endregion


        #region etc variable

        // event
        public UnityEvent<int> event_SP_Edit;

        [SerializeField] protected List<BaseStat> listBottomPlayer = new List<BaseStat>();
        [SerializeField] protected List<BaseStat> listTopPlayer = new List<BaseStat>();

        private readonly string recvMessage = "RecvBattleManager";

        #endregion

        
        #region static
        public static bool IsNetwork
        {
            get
            {
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

            Application.targetFrameRate = 30;
            InitializeManager();
            
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
            data_DiceInfo = JsonDataManager.Get().dataDiceInfo;

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
                UI_InGame.Get().SetUnitCount(listBottomPlayer.Count + listTopPlayer.Count - 2);
        }

        #endregion


        #region init destroy

        public void InitializeManager()
        {
            arrUpgradeLevel = new int[6];
            event_SP_Edit = new UnityEvent<int>();
        }


        public void DestroyManager()
        {
            arrUpgradeLevel = null;

            event_SP_Edit.RemoveAllListeners();
            event_SP_Edit = null;

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
                playerController.isBottomPlayer = NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer;
                playerController.ChangeLayer(NetworkManager.Get().GetNetInfo().playerInfo.IsBottomPlayer);


                Vector3 otherTowerPos = FieldManager.Get().GetPlayerPos(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
                GameObject otherTObj = UnityUtil.Instantiate("Tower/" + pref_Player.name);
                otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
                otherTObj.transform.position = otherTowerPos;
                playerController.targetPlayer = otherTObj.GetComponent<PlayerController>();
                playerController.targetPlayer.isMine = false;
                playerController.targetPlayer.isBottomPlayer = NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer;
                playerController.targetPlayer.targetPlayer = playerController;
                
                playerController.targetPlayer.ChangeLayer(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);
                
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
                playerController.isBottomPlayer = true;

                GameObject otherTObj = Instantiate(pref_AI, FieldManager.Get().GetPlayerPos(false), Quaternion.identity);
                otherTObj.transform.parent = FieldManager.Get().GetPlayerTrs(false);
                playerController.targetPlayer = otherTObj.GetComponent<PlayerController>();
                
                // name
                UI_InGame.Get().SetNickName(ObscuredPrefs.GetString("Nickname") , "AI");
                otherTObj.SendMessage("ChangeLayer", false);
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
                
                playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                //other
                playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            else
            {
                // 네트워크 안쓰니까...개발용으로
                //var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
                if (UserInfoManager.Get() != null)
                {
                    var deck = UserInfoManager.Get().GetActiveDeck();
                    playerController.SetDeck(deck);
                }

            }

            // Upgrade buttons
            // ui 셋팅
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, arrUpgradeLevel);

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

            // not use
            /*
            if (PhotonNetwork.IsConnected)
            {
                UI_InGamePopup.Get().SetViewWaiting(true);

                if (PlayerController.Get() == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.identifier);

                    //var startPos = PhotonNetwork.IsMasterClient ? ts_BottomPlayer.position : ts_TopPlayer.position;
                    Vector3 startPos = FieldManager.Get().GetPlayerPos(PhotonNetwork.IsMasterClient);
                    GameObject obj = PhotonNetwork.Instantiate("Tower/" + pref_Player.name, startPos, Quaternion.identity, 0);
                    obj.transform.parent = FieldManager.Get().GetPlayerTrs(PhotonNetwork.IsMasterClient);
                    playerController = obj.GetComponent<PlayerController>();
                    
                    //playerController.photonView.RPC("ChangeLayer", RpcTarget.All, PhotonNetwork.IsMasterClient);
                    //Debug.Log("StartManager (IsMasterClient): " + PhotonNetwork.IsMasterClient);
                    playerController.SendPlayer(RpcTarget.All , E_PTDefine.PT_CHANGELAYER , PhotonNetwork.IsMasterClient);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            else
            {
                Vector3 startPos = FieldManager.Get().GetPlayerPos(true);
                var obj = Instantiate(pref_Player, startPos, Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(true);
                playerController = obj.GetComponent<PlayerController>();
                playerController.ChangeLayer(true);
                playerController.isMine = true;
                playerController.isBottomPlayer = true;

                obj = Instantiate(pref_AI, FieldManager.Get().GetPlayerPos(false), Quaternion.identity);
                obj.transform.parent = FieldManager.Get().GetPlayerTrs(false);
                obj.SendMessage("ChangeLayer", false);

                isAIMode = true;
            }

            var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
            if (UserInfoManager.Get() != null)
            {
                deck = UserInfoManager.Get().GetActiveDeck();
            }
            
            if (PhotonNetwork.IsConnected)
            {
                //playerController.photonView.RPC("SetDeck", RpcTarget.All, deck);
                playerController.SendPlayer(RpcTarget.All , E_PTDefine.PT_SETDECK , deck);
            }
            else
            {
                playerController.SetDeck(deck);
            }

            // Upgrade buttons
            // ui 셋팅
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck , arrUpgradeLevel);


            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient == false)
                {
                    ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
                    ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
                    ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
                }
            }

            //obj_ViewTargetDiceField.SetActive(!PhotonNetwork.IsConnected);
            UI_InGame.Get().ViewTargetDice(!PhotonNetwork.IsConnected);
            
            event_SP_Edit.AddListener(RefreshSP);
            event_SP_Edit.AddListener(SetSPUpgradeButton);

            StartGame();
            RefreshTimeUI(true);
            */
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
                StartCoroutine(SpawnLoop());
            }
            else if (IsNetwork == false)
            {

                Debug.Log("StartGame: OfflineMode");
                // 네트워크 연결 안됫으니 deactive...   
                DeactivateWaitingObject();
                StartCoroutine(SpawnLoop());
            }

            // not use
            /*
            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Ready();
                    StartCoroutine(SpawnLoop());
                }
                else
                {
                    //photonView.RPC("Ready", RpcTarget.MasterClient);
                    SendBattleManager(RpcTarget.MasterClient , E_PTDefine.PT_READY);
                }
            }
            else
            {
                Debug.Log("StartGame: OfflineMode");
                StartCoroutine(SpawnLoop());
            }
            */
        }

        // not use
        //[PunRPC]
        private void Ready()
        {
            _readyPlayerCount++;
        }

        #endregion

        #region spawn

        private IEnumerator SpawnLoop()
        {
            wave = 0;
            // 개발용으로 쓰일때만..
            if (IsNetwork == false)
                WorldUIManager.Get().SetWave(wave);

            time = startSpawnTime;
            int[] arrAddTime = {20, 15, 10, 5, -1};
            var addNum = 0;

            while (true)
            {
                yield return null;

                time -= Time.deltaTime;

                //
                if (wave > 0 && time <= arrAddTime[addNum])
                {
                    addNum++;
                    /*if (IsNetwork() == true)
                    {
                        //...sp 를 노티가 오니....안해도 되겟네..
                    }*/
                    if (IsNetwork == false)
                    {
                        AddSP(wave);
                    }
                }

                if (time <= 0)
                {
                    time = spawnTime;
                    addNum = 0;

                    if (IsNetwork == false)
                    {
                        playerController.Spawn();
                        playerController.targetPlayer.Spawn();

                        wave++;
                    }
                }
            }
            
            //not use
            /*
            if (PhotonNetwork.IsConnected)
            {
                while (PhotonNetwork.InRoom && _readyPlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    yield return null;
                }
                //photonView.RPC("DeactivateWaitingObject", RpcTarget.All);
                SendBattleManager(RpcTarget.All , E_PTDefine.PT_DEACTIVEWAIT);
            }
            else
            {
                DeactivateWaitingObject();
            }

            wave = 0;
            //tmp_Wave.text = $"{wave}";
            WorldUIManager.Get().SetWave(wave);
            time = startSpawnTime;
            int[] arrAddTime = { 20, 15, 10, 5, -1 };
            var addNum = 0;

            while (true)
            {
                yield return null;

                time -= Time.deltaTime;

                if (wave > 0 && time <= arrAddTime[addNum])
                {
                    addNum++;
                    if (PhotonNetwork.IsConnected)
                    {
                        //photonView.RPC("AddSP", RpcTarget.All, wave);
                        SendBattleManager(RpcTarget.All , E_PTDefine.PT_ADDSP , wave);
                        // clear cache
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                    else
                    {
                        AddSP(wave);
                    }
                }

                if (time <= 0)
                {
                    time = spawnTime;
                    addNum = 0;
                    if (PhotonNetwork.IsConnected)
                    {
                        //photonView.RPC("SpawnPlayerMinions", RpcTarget.All);
                        
                        SendBattleManager(RpcTarget.All , E_PTDefine.PT_SPAWNMINION );
                    }
                    else
                    {
                        playerController.Spawn();
                        playerController.targetPlayer.Spawn();
                    }

                    wave++;
                }
            }
            */
            
        }

        #endregion


        #region sp & spawn

        private void DeactivateWaitingObject()
        {
            isGamePlaying = true;
            UI_InGamePopup.Get().SetViewWaiting(false);
        }

        // not network --
        private void AddSP(int wave = 0)
        {
            playerController.AddSpByWave(wave);

            if (IsNetwork == false)
            {
                playerController.targetPlayer.AddSpByWave(wave);
            }
        }

        // network setting
        private void NetSetSp(int sp)
        {
            playerController.SetSp(sp);
        }

        public void NetSpawnNotify(int wave, int uid = 0)
        {
            this.wave = wave;
            WorldUIManager.Get().SetWave(wave);
            // 시간 리셋
            time = spawnTime;

            Debug.Log("spawn  : " + wave);
            
            switch (NetworkManager.Get().playType)
            {
                case Global.PLAY_TYPE.BATTLE:
                    playerController.Spawn();
                    playerController.targetPlayer.Spawn();
                    break;
                case Global.PLAY_TYPE.COOP:
                    if (uid > 0)
                    {
                        if (NetworkManager.Get().UserUID == uid)
                        {
                            playerController.Spawn();
                        }
                        else if (NetworkManager.Get().OtherUID == uid)
                        {
                            playerController.targetPlayer.Spawn();
                        }
                        ((Coop_AI) playerController.coopPlayer).Spawn();
                    }
                    break;
            }

            // 스폰이 불릴때마다 시간갱신을 위해 저장
            if (NetworkManager.Get() && IsNetwork)
                NetworkManager.Get().SaveBattleInfo();
        }

        public void NetMonsterSpawnNotify(int uid, MsgBossMonster msg)
        {
            if (playerController.UID == uid)
            {
                playerController.SpawnMonster(msg);
            }
            else if (playerController.targetPlayer.UID == uid)
            {
                playerController.targetPlayer.SpawnMonster(msg);
            }
            else if (playerController.coopPlayer.UID == uid)
            {
                playerController.coopPlayer.SpawnMonster(msg);
            }
        }

        #endregion


        #region update event

        protected void RefreshSP(int sp)
        {
            UI_InGame.Get().SetSP(sp);
        }


        protected void RefreshTimeUI(bool isImmediately = false)
        {
            if (isImmediately)
            {
                WorldUIManager.Get().SetSpawnTime(time / st);
            }
            else
            {
                float ff = Mathf.Lerp(WorldUIManager.Get().GetSpawnAmount(), time / st, Time.deltaTime * 5.0f);

                if (ff < WorldUIManager.Get().GetSpawnAmount())
                    WorldUIManager.Get().SetSpawnTime(ff);
                else 
                    WorldUIManager.Get().SetSpawnTime(time / st);
            }
            
       
            WorldUIManager.Get().SetTextSpawnTime(time);
            
            if (IsNetwork == false)
                WorldUIManager.Get().SetWave(wave);
        }

        #endregion


        #region net get dice
        public void NetGetDice()
        {
            SendInGameManager(GameProtocol.GET_DICE_REQ);
        }

        public void GetDiceCallBack(int diceId, int slotNum, int level, int curSp)
        {
            getDiceCount++;
            UI_InGame.Get().SetDiceButtonText(GetDiceCost());
            playerController.GetDice(diceId, slotNum, level);
            NetSetSp(curSp);
            
            UI_InGame.Get().ControlGetDiceButton(true);
        }

        public void GetDiceOther(int diecId, int slotNum, int level)
        {
            playerController.targetPlayer.OtherGetDice(diecId, slotNum);
        }

        #endregion


        // not network use
        #region get set
        public void GetDice()
        {
            playerController.AddSp(-GetDiceCost());
            getDiceCount++;
            playerController.AddSp(0);
            //text_GetDiceButton.text = $"{getDiceCost}";
            UI_InGame.Get().SetDiceButtonText(GetDiceCost());
        }
        public int GetDiceCost()
        {
            return 10 + getDiceCount * 10;
        }


        public BaseStat GetRandomPlayerUnit(bool isBottomPlayer)
        {
            int searchCount = 30;
            int rnd = 0;
            if (isBottomPlayer)
            {
                if (listBottomPlayer.Count > 0 && listBottomPlayer[0].isAlive)
                {
                    do
                    {
                        searchCount--;
                        rnd = Random.Range(0, listBottomPlayer.Count);
                    } while (listBottomPlayer[rnd].isAlive == false && searchCount > 0);

                    return listBottomPlayer[rnd];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (listTopPlayer.Count > 0 && listTopPlayer[0].isAlive)
                {
                    do
                    {
                        searchCount--;
                        rnd = Random.Range(0, listTopPlayer.Count);
                    } while (listTopPlayer[rnd].isAlive == false && searchCount > 0);

                    return listTopPlayer[rnd];
                }
                else
                {
                    return null;
                }
            }
        }

        public Vector3 GetRandomPlayerFieldPosition(bool isBottomPlayer)
        {
            var x = Random.Range(-3f, 3f);
            var z = Random.Range(-2f, 2f);
            return new Vector3(x, 0, z);
        }

        protected void SetSPUpgradeButton(int sp)
        {
            //button_SP_Upgrade.interactable = (playerController.spUpgradeLevel + 1) * 500 <= sp;
            //text_SP_Upgrade.text = $"SP Lv.{playerController.spUpgradeLevel + 1}";
            //text_SP_Upgrade_Price.text = $"{(playerController.spUpgradeLevel + 1) * 500}";
            UI_InGame.Get().SetSPUpgrade(playerController.spUpgradeLevel, sp);
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

            // not use
            /*
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {
                GameStateManager.Get().MoveMainScene();
            }
            */
        }

        // 내자신이 나간다고 눌럿을때 응답 받은것
        public void CallBackLeaveRoom()
        {
            if(IsNetwork)
                NetworkManager.Get().DisconnectSocket();

            // 잠시 테스트로 주석
            NetworkManager.Get().DeleteBattleInfo();
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


        public void EndGame(bool winLose, int winningStreak, MsgReward[] normalReward, MsgReward[] streakReward, MsgReward[] perfectReward)
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


            UI_InGamePopup.Get().SetPopupResult(true, winLose, winningStreak, normalReward, streakReward, perfectReward);
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);

            SoundManager.instance.StopBGM();
            SoundManager.instance.Play(winLose ? Global.E_SOUND.BGM_INGAME_WIN : Global.E_SOUND.BGM_INGAME_LOSE);
        }
        
        /*public void EndGame(PhotonMessageInfo info)
        {
            isGamePlaying = false;
            StopAllCoroutines();
            //popup_Result.SetActive(true);
            UI_InGamePopup.Get().SetPopupResult(true);
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);

            //text_Result.text = playerController.isAlive ? "승리" : "패배";
            UI_InGamePopup.Get().SetResultText(playerController.isAlive ? Global.g_inGameWin : Global.g_inGameLose);
        }*/

        #endregion


        #region net etc system

        public void Click_SP_Upgrade_Button()
        {
            if(IsNetwork)
                SendInGameManager(GameProtocol.UPGRADE_SP_REQ);
            else
            {
                playerController.SP_Upgrade();
            }
        }

        public void InGameUpgradeCallback(int diceId, int upgradeLv, int curSp)
        {
            int serverUpgradeLv = upgradeLv - 1;
            if (serverUpgradeLv < 0)
                serverUpgradeLv = 0;
            
            playerController.InGameDiceUpgrade(diceId, serverUpgradeLv);
            UI_InGame.Get().SetDeckRefresh(diceId, serverUpgradeLv);
            NetSetSp(curSp);
        }


        public BaseStat GetRandomPlayerUnitHighHealth(bool pIsBottomPlayer)
        {
            BaseStat rtn = null;
            float hp = 0;

            if (pIsBottomPlayer)
            {
                for (int i = 1; i < listBottomPlayer.Count; i++)
                {
                    if (listBottomPlayer[i].currentHealth > hp)
                    {
                        rtn = listBottomPlayer[i];
                        hp = rtn.currentHealth;
                    }
                }

                return rtn;
            }
            else
            {
                for (int i = 1; i < listTopPlayer.Count; i++)
                {
                    if (listTopPlayer[i].currentHealth > hp)
                    {
                        rtn = listTopPlayer[i];
                        hp = rtn.currentHealth;
                    }
                }

                return rtn;
            }
        }

        public void AddPlayerUnit(bool isBottomPlayer, BaseStat bs)
        {
            if (isBottomPlayer && listBottomPlayer.Contains(bs) == false)
            {
                listBottomPlayer.Add(bs);
            }
            else if (isBottomPlayer == false && listTopPlayer.Contains(bs) == false)
            {
                listTopPlayer.Add(bs);
            }
        }
        
        public void RemovePlayerUnit(bool isBottomPlayer, BaseStat bs)
        {
            if (isBottomPlayer && listBottomPlayer.Contains(bs))
            {
                listBottomPlayer.Remove(bs);
            }
            else if (isBottomPlayer == false && listTopPlayer.Contains(bs))
            {
                listTopPlayer.Remove(bs);
            }
        }
        
        #endregion
        
        
        #region pause , resume , sync 
        
        private void RevmoeAllMinionAndMagic()
        {
            playerController.RemoveAllMinionAndMagic();
            playerController.targetPlayer.RemoveAllMinionAndMagic();
        }


        public void OnApplicationPause(bool pauseStatus)
        {
            NetworkManager.Get().PrintNetworkStatus();

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
            NetworkManager.Get().PrintNetworkStatus();

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
                playerController.SyncMinionResume();
                playerController.targetPlayer.SyncMinionResume();
            }
        }

        public void SendSyncAllBattleInfo()
        {
            if (isGamePlaying == false)
                return;
            
            // 인디케이터 -- 어차피 재동기화 위해 데이터 날려야됨
            UI_InGamePopup.Get().ViewGameIndicator(true);
            
            // 미니언들 idle 강제 idle 상태로 만든다
            // foreach (var minion in playerController.listMinion)
            // {
            //     minion.StopAllCoroutines();
            //     minion.behaviourTreeOwner.behaviour.Pause();
            //     minion.animator.SetTrigger("Idle");
            // }
            // foreach (var minion in playerController.targetPlayer.listMinion)
            // {
            //     minion.StopAllCoroutines();
            //     minion.behaviourTreeOwner.behaviour.Pause();
            //     minion.animator.SetTrigger("Idle");
            // }
            //Time.timeScale = 0;
            
            
            // 현재 전장에 있는 미니언 정보들 모은다 
            NetSyncData myData = new NetSyncData();
            NetSyncData otherData = new NetSyncData();
            // 내 미니언
            myData.towerHp = playerController.currentHealth;
            myData.userId = NetworkManager.Get().UserUID;
            
            // 상대방 미니언
            otherData.towerHp = playerController.targetPlayer.currentHealth;
            otherData.userId = NetworkManager.Get().OtherUID;
            
            // NetSyncData my , other 
            if (playerController.isBottomPlayer)
            {
                foreach (var stat in listBottomPlayer)
                {
                    var m = stat as Minion;
                    if (m != null)
                    {
                        Debug.LogFormat("Send My SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
                        myData.netSyncMinionData.Add(m.GetNetSyncMinionData());
                    }
                }
                foreach (var stat in listTopPlayer)
                {
                    var m = stat as Minion;
                    if (m != null)
                    {
                        Debug.LogFormat("Send Other SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
                        otherData.netSyncMinionData.Add(m.GetNetSyncMinionData());
                    }
                }
            }
            else
            {
                foreach (var stat in listTopPlayer)
                {
                    var m = stat as Minion;
                    if (m != null)
                    {
                        Debug.LogFormat("Send My SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
                        myData.netSyncMinionData.Add(m.GetNetSyncMinionData());
                    }
                }
                foreach (var stat in listBottomPlayer)
                {
                    var m = stat as Minion;
                    if (m != null)
                    {
                        Debug.LogFormat("Send Other SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
                        otherData.netSyncMinionData.Add(m.GetNetSyncMinionData());
                    }
                }
            }

            MsgSyncMinionData[] syncMyMinionData = ConvertNetMsg.ConvertNetSyncToMsg(myData);
            MsgSyncMinionData[] syncOtherMinionData = ConvertNetMsg.ConvertNetSyncToMsg(otherData);
            
            // 돌리던 ai false
            NetworkManager.Get().SetOtherDisconnect(false);

            //
            //Peer peer, int playerId, MsgSyncMinionData[] syncMinionData, int otherPlayerId, MsgSyncMinionData[] otherSyncMinionData
            // 데이터 보냄
            SendInGameManager(GameProtocol.START_SYNC_GAME_REQ , myData.userId, playerController.spawnCount , syncMyMinionData , otherData.userId, playerController.targetPlayer.spawnCount , syncOtherMinionData);
        }

        public void SyncGameData(MsgStartSyncGameNotify gameData)
        {
            print("recv p info " + gameData.PlayerInfo.PlayerUId + "  " + gameData.PlayerInfo.Name);
            print("recv other info " + gameData.OtherPlayerInfo.PlayerUId + "  " + gameData.OtherPlayerInfo.Name);

            // 정보 셋팅
            NetworkManager.Get().GetNetInfo().SetPlayerInfo(gameData.PlayerInfo);
            playerController.currentHealth = ConvertNetMsg.MsgIntToFloat(gameData.PlayerInfo.TowerHp);
            if (playerController.currentHealth <= 20000) playerController.isHalfHealth = true; 
            playerController.RefreshHealthBar();
            playerController.transform.parent = FieldManager.Get().GetPlayerTrs(gameData.PlayerInfo.IsBottomPlayer);
            playerController.transform.position = FieldManager.Get().GetPlayerPos(gameData.PlayerInfo.IsBottomPlayer);
            playerController.isMine = true;
            playerController.ChangeLayer(gameData.PlayerInfo.IsBottomPlayer);
            getDiceCount = gameData.PlayerInfo.GetDiceCount;
            playerController.SetSp(gameData.PlayerInfo.CurrentSp);

            NetworkManager.Get().GetNetInfo().SetOtherInfo(gameData.OtherPlayerInfo);
            playerController.targetPlayer.currentHealth = ConvertNetMsg.MsgIntToFloat(gameData.OtherPlayerInfo.TowerHp);
            if (playerController.targetPlayer.currentHealth <= 20000) playerController.targetPlayer.isHalfHealth = true; 
            playerController.targetPlayer.RefreshHealthBar();
            playerController.targetPlayer.transform.parent = FieldManager.Get().GetPlayerTrs(gameData.OtherPlayerInfo.IsBottomPlayer);
            playerController.targetPlayer.transform.position = FieldManager.Get().GetPlayerPos(gameData.OtherPlayerInfo.IsBottomPlayer);
            playerController.targetPlayer.isMine = false;
            playerController.targetPlayer.ChangeLayer(gameData.OtherPlayerInfo.IsBottomPlayer);
            playerController.targetPlayer.SetSp(gameData.OtherPlayerInfo.CurrentSp);
            
            CameraController.Get().Start();

            //
            SyncInfo();
            
            // 주사위필드 데이터 셋팅
            playerController.targetPlayer.SetDiceField(gameData.OtherGameDiceData);
            playerController.SetDiceField(gameData.GameDiceData);
            
            // 미니언 셋팅
            List<NetSyncMinionData> myMinionData = ConvertNetMsg.ConvertMsgToSync(gameData.SyncMinionData);
            foreach (var data in myMinionData)
            {
                var diceData = data_DiceInfo.GetData(data.minionDataId);
                var m = playerController.CreateMinion(FileHelper.LoadPrefab(diceData.prefabName, Global.E_LOADTYPE.LOAD_MINION), data.minionPos, 1, 1, false);
                m.ChangeLayer(gameData.PlayerInfo.IsBottomPlayer);
                m.Initialize(playerController.MinionDestroyCallback);
                if (data.minionDataId == 4004)
                {
                    m.CancelInvoke("Fusion");
                    ((Minion_Robot)m).Transform();
                }
                m.SetNetSyncMinionData(data);
                Debug.LogFormat("Recv My SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
            }
            
            List<NetSyncMinionData> otherMinionData = ConvertNetMsg.ConvertMsgToSync(gameData.OtherSyncMinionData);
            foreach (var data in otherMinionData)
            {
                var diceData = data_DiceInfo.GetData(data.minionDataId);
                var m = playerController.targetPlayer.CreateMinion(FileHelper.LoadPrefab(diceData.prefabName, Global.E_LOADTYPE.LOAD_MINION), data.minionPos, 1, 1, false);
                m.ChangeLayer(gameData.OtherPlayerInfo.IsBottomPlayer);
                m.Initialize(playerController.targetPlayer.MinionDestroyCallback);
                if (data.minionDataId == 4004)
                {
                    m.CancelInvoke("Fusion");
                    ((Minion_Robot)m).Transform();
                }
                m.SetNetSyncMinionData(data);
                Debug.LogFormat("Recv Other SyncMinion ID:{0}, DataID:{1}, HP:{2}", m.id, m.diceId, m.currentHealth);
            }

            // Spawn Count
            playerController.spawnCount = gameData.PlayerSpawnCount;
            playerController.targetPlayer.spawnCount = gameData.OtherPlayerSpawnCount;
            
            NetworkManager.Get().SetReconnect(false);

            // timer 돌려야됨
            NetStartGame();
            
            //
            SendInGameManager(GameProtocol.END_SYNC_GAME_REQ);
        }


        public void SyncInfo()
        {
            // deck setting
            if (IsNetwork == true)
            {
                // my
                for(int i = 0 ; i < NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray.Length; i++)
                    print(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray[i]);
                
                playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                //other
                playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            
            // Upgrade buttons
            // ui 셋팅
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck, arrUpgradeLevel);

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


        #region network
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

        // net direct player
        public void RecvPlayerManager(GameProtocol protocol, params object[] param)
        {
            playerController.NetRecvPlayer(protocol, param);
        }

        // net ingame manager to do
        public void RecvInGameManager(GameProtocol protocol, params object[] param)
        {
            switch (protocol)
            {
                #region case ack

                case GameProtocol.LEAVE_GAME_ACK:
                    CallBackLeaveRoom();
                    break;
                case GameProtocol.GET_DICE_ACK:
                {
                    MsgGetDiceAck diceack = (MsgGetDiceAck) param[0];
                    GetDiceCallBack(diceack.DiceId, diceack.SlotNum, diceack.Level, diceack.CurrentSp);

                    Debug.Log(diceack.DiceId + "  " + diceack.SlotNum + "   " + diceack.Level);

                    break;
                }
                case GameProtocol.MERGE_DICE_ACK:
                {
                    MsgMergeDiceAck lvupDiceack = (MsgMergeDiceAck) param[0];
                    playerController.LevelUpDice(lvupDiceack.ResetFieldNum, lvupDiceack.LeveupFieldNum, lvupDiceack.LevelupDiceId, lvupDiceack.Level);

                    break;
                }
                case GameProtocol.UPGRADE_SP_ACK:
                {
                    MsgUpgradeSpAck spack = (MsgUpgradeSpAck) param[0];
                    playerController.SP_Upgrade(spack.Upgrade, spack.CurrentSp);
                    break;
                }
                case GameProtocol.INGAME_UP_DICE_ACK:
                {
                    MsgInGameUpDiceAck ingameup = (MsgInGameUpDiceAck) param[0];
                    InGameUpgradeCallback(ingameup.DiceId, ingameup.InGameUp, ingameup.CurrentSp);
                    break;
                }
                #endregion

                #region case notify
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    OnOtherLeft(Convert.ToUInt16(param[0]));
                    break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY:
                {
                    if (NetworkManager.Get().UserUID == Convert.ToUInt16(param[0])) // param 0 = useruid
                    {
                        NetSetSp((int) param[1]); // param1 wave
                    }

                    NetStartGame();
                    break;
                }
                case GameProtocol.ADD_SP_NOTIFY:
                {
                    if (NetworkManager.Get().UserUID == Convert.ToUInt16(param[0])) // param 0 = useruid
                    {
                        NetSetSp((int) param[1]); // param1 wave
                    }

                    break;
                }
                case GameProtocol.SPAWN_NOTIFY:
                {
                    // 시작이 되었으니...
                    if (NetworkManager.Get().isResume == true)
                    {
                        Time.timeScale = 1f;
                        
                        NetworkManager.Get().SetResume(false);
                        
                        // 아군유닛 비헤이비어트리 활성화
                        foreach (var minion in playerController.listMinion)
                        {
                            minion.behaviourTreeOwner.behaviour.Resume();
                        }
                    }
                    
                    // 인디케이터도 다시 안보이게..
                    if (UI_InGamePopup.Get().IsIndicatorActive() == true)
                    {
                        UI_InGamePopup.Get().ViewGameIndicator(false);
                    }
                    
                    NetSpawnNotify((int) param[0]);
                    break;
                }
                case GameProtocol.COOP_SPAWN_NOTIFY:
                {
                    // 시작이 되었으니...
                    if (NetworkManager.Get().isResume == true)
                    {
                        Time.timeScale = 1f;
                        
                        NetworkManager.Get().SetResume(false);
                        
                        // 아군유닛 비헤이비어트리 활성화
                        foreach (var minion in playerController.listMinion)
                        {
                            minion.behaviourTreeOwner.behaviour.Resume();
                        }
                    }
                    
                    // 인디케이터도 다시 안보이게..
                    if (UI_InGamePopup.Get().IsIndicatorActive() == true)
                    {
                        UI_InGamePopup.Get().ViewGameIndicator(false);
                    }

                    MsgCoopSpawnNotify msg = (MsgCoopSpawnNotify) param[0];
                    NetSpawnNotify(msg.Wave, msg.PlayerUId);

                    break;
                }
                case GameProtocol.MONSTER_SPAWN_NOTIFY:
                {
                    MsgMonsterSpawnNotify msg = (MsgMonsterSpawnNotify) param[0];
                    NetMonsterSpawnNotify(msg.PlayerUId, msg.SpawnBossMonster);

                    break;
                }
                case GameProtocol.GET_DICE_NOTIFY:
                {
                    // 상대방것만 온다...
                    MsgGetDiceNotify dicenoty = (MsgGetDiceNotify) param[0];
                    if (NetworkManager.Get().OtherUID == dicenoty.PlayerUId)
                    {
                        GetDiceOther(dicenoty.DiceId, dicenoty.SlotNum, dicenoty.Level);
                    }

                    break;
                }
                case GameProtocol.MERGE_DICE_NOTIFY:
                {
                    MsgMergeDiceNotify lvupdiceNoti = (MsgMergeDiceNotify) param[0];
                    
                    if (NetworkManager.Get().OtherUID == lvupdiceNoti.PlayerUId )
                        playerController.targetPlayer.LevelUpDice(lvupdiceNoti.ResetFieldNum,lvupdiceNoti.LeveupFieldNum, lvupdiceNoti.LevelupDiceId, lvupdiceNoti.Level);
                    
                    break;
                }
                case GameProtocol.UPGRADE_SP_NOTIFY:
                {
                    MsgUpgradeSpNotify notisp = (MsgUpgradeSpNotify) param[0];
                    // 상대방이 SP 업그레이드한건데...알필요가 잇을까 싶다..일단은 공간만 만들어놓자
                    if (NetworkManager.Get().OtherUID == notisp.PlayerUId )
                        playerController.targetPlayer.SP_Upgrade(notisp.Upgrade);
                    break;
                }
                case GameProtocol.INGAME_UP_DICE_NOTIFY:
                {
                    // 상대방이...주사위 업그레이 햇다..고 노티가 날라옴
                    MsgInGameUpDiceNotify notiIngame = (MsgInGameUpDiceNotify) param[0];

                    // 상대방이 햇다는것을..알필요가 잇나..잇겟지...
                    if (NetworkManager.Get().OtherUID == notiIngame.PlayerUId )
                        playerController.targetPlayer.InGameDiceUpgrade(notiIngame.DiceId, notiIngame.InGameUp);
                    break;
                }
                case GameProtocol.END_GAME_NOTIFY:
                {
                    MsgEndGameNotify endNoti = (MsgEndGameNotify) param[0];
                    
                    EndGame((GAME_RESULT.VICTORY == endNoti.GameResult || GAME_RESULT.VICTORY_BY_DEFAULT == endNoti.GameResult), Convert.ToInt32(endNoti.WinningStreak), endNoti.NormalReward, endNoti.StreakReward, endNoti.PerfectReward);
                    
                    break;
                }
                
                
                
                case GameProtocol.START_SYNC_GAME_ACK:
                {
                    MsgStartSyncGameAck startsyncack = (MsgStartSyncGameAck) param[0];
                    // 할일없다
                    break;
                }
                case GameProtocol.START_SYNC_GAME_NOTIFY:
                {
                    MsgStartSyncGameNotify syncNotify = (MsgStartSyncGameNotify) param[0];
                    
                    // 받은 데이터로 동기화 시킨다
                    SyncGameData(syncNotify);
                    
                    break;
                }
                
                case GameProtocol.END_SYNC_GAME_ACK:
                {
                    MsgEndSyncGameAck endsynack = (MsgEndSyncGameAck) param[0];
                    
                    // 시작이 되었으니...
                    if (NetworkManager.Get().isResume == true)
                    {
                        NetworkManager.Get().SetResume(false);
                    }
                    // 인디케이터도 다시 안보이게..
                    if (UI_InGamePopup.Get().IsIndicatorActive() == true)
                    {
                        UI_InGamePopup.Get().ViewGameIndicator(false);
                    }

                    Time.timeScale = 1f;

                    break;
                }
                case GameProtocol.END_SYNC_GAME_NOTIFY:
                {
                    MsgEndSyncGameNotify endSyncNotify = (MsgEndSyncGameNotify) param[0];
                    
                    // 시작이 되었으니...
                    if (NetworkManager.Get().isResume == true)
                    {
                        NetworkManager.Get().SetResume(false);
                    }
                    // 인디케이터도 다시 안보이게..
                    if (UI_InGamePopup.Get().IsIndicatorActive() == true)
                    {
                        UI_InGamePopup.Get().ViewGameIndicator(false);
                    }

                    Time.timeScale = 1f;
                    
                    break;
                }
                
                
                // 상대방의 접속이 끊겼다
                case GameProtocol.DISCONNECT_GAME_NOTIFY:
                {
                    MsgDisconnectGameNotify disNoti = (MsgDisconnectGameNotify) param[0];
                    
                    if (NetworkManager.Get().UserUID != disNoti.PlayerUId)
                    {
                        NetworkManager.Get().SetOtherDisconnect(true);
                    }
                    
                    break;
                }



                case GameProtocol.READY_SYNC_GAME_ACK:
                {
                    MsgReadySyncGameAck readyack = (MsgReadySyncGameAck) param[0];
                    
                    Time.timeScale = 0;

                    break;
                }
                case GameProtocol.READY_SYNC_GAME_NOTIFY:
                {
                    MsgReadySyncGameNotify readynoti = (MsgReadySyncGameNotify) param[0];
                    
                    if (NetworkManager.Get().UserUID != readynoti.PlayerUId)
                    {
                        Time.timeScale = 0;

                        NetworkManager.Get().SetResume(true);
                        // 미니언 정보 취합 해서 보내준다..
                        SendSyncAllBattleInfo();
                    }
                    
                    break;
                }
                
                
                
                
                case GameProtocol.PAUSE_GAME_NOTIFY:
                {
                    MsgPauseGameNotify pauseNoti = (MsgPauseGameNotify) param[0];

                    if (NetworkManager.Get().UserUID != pauseNoti.PlayerUId)
                    {
                        NetworkManager.Get().SetOtherDisconnect(true);
                    }
                    
                    break;
                }
                case GameProtocol.RESUME_GAME_NOTIFY:
                {
                    MsgResumeGameNotify resumeNoti = (MsgResumeGameNotify) param[0];

                    if (NetworkManager.Get().UserUID != resumeNoti.PlayerUId)
                    {
                        NetworkManager.Get().SetResume(true);
                        NetworkManager.Get().SetOtherDisconnect(false);
                        // 미니언 정보 취합 해서 보내준다..
                        //SendSyncAllBattleInfo();
                    }
                    break;
                }

                #endregion
                
                
            }
        }

        #endregion


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
        
        public BaseStat GetBaseStatFromId(int baseStatId)
        {
            int uid = 0;
            if (baseStatId >= 10000) uid = baseStatId / 10000;
            else uid = baseStatId / 1000;
            //int bsID = baseStatId % 10000;
            Debug.Log($"GetBaseStatFromID = UID:{uid}, ID:{baseStatId}");

            PlayerController pc = null;
            if (NetworkManager.Get().UserUID == uid)
            {
                pc = playerController;
            }
            else if (NetworkManager.Get().OtherUID == uid)
            {
                pc = playerController.targetPlayer;
            }
            else if (NetworkManager.Get().CoopUID == uid)
            {
                pc = playerController.coopPlayer;
            }

            if (baseStatId < 0) return null;
            if (pc.id == baseStatId || (baseStatId >= 10000 && baseStatId % 10000 == 0)) return pc;

            var minion = pc.listMinion.Find(m => m.id == baseStatId);
            if (minion != null)
            {
                return minion;
            }
            else
            {
                var magic = pc.listMagic.Find(m => m.id == baseStatId);
                if (magic != null)
                {
                    return magic;
                }
                else
                {
                    return null;
                }
            }
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
