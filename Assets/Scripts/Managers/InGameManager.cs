#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using RWGameProtocol;
using RWGameProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;


using CodeStage.AntiCheat.ObscuredTypes;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


#region USING PHOTON
//using Photon.Pun;
//using Photon.Realtime;
#endregion

namespace ED
{
    public class InGameManager : SingletonDestroy<InGameManager> //, IPunObservable
    {
        //ßpublic static InGameManager Instance;

        #region game system variable

        [Header("SYSTEN INFO")] 
        public PLAY_TYPE playType;

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

        private float st => wave < 1 ? 10f : 20f;

        public float time { get; protected set; }

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
            //if(Instance == this)
            //{
            //Instance = null;
            //}
            
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

        

        public void StartManager()
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
                var deck = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
                if (UserInfoManager.Get() != null)
                {
                    deck = UserInfoManager.Get().GetActiveDeck();
                }

                playerController.SetDeck(deck);
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

            //obj_ViewTargetDiceField.SetActive(!PhotonNetwork.IsConnected);
            UI_InGame.Get().ViewTargetDice(!IsNetwork);

            event_SP_Edit.AddListener(RefreshSP);
            event_SP_Edit.AddListener(SetSPUpgradeButton);

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

        public void NetSpawnNotify(int wave)
        {
            this.wave = wave;
            WorldUIManager.Get().SetWave(wave);

            Debug.Log("spawn  : " + wave);

            playerController.Spawn();
            playerController.targetPlayer.Spawn();
        }

        #endregion


        #region update event

        private void RefreshSP(int sp)
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

        private void SetSPUpgradeButton(int sp)
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


        public void EndGame(bool winLose)
        {
            isGamePlaying = false;
            StopAllCoroutines();
            
            UI_InGamePopup.Get().SetPopupResult(true);
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);
            
            UI_InGamePopup.Get().SetResultText(winLose ? Global.g_inGameWin : Global.g_inGameLose);
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


        // 매니저 외부에서 패킷을 보낼때 쓰자..

        #region outter send

        public void SendDiceLevelUp(int resetFieldNum, int levelUpFieldNum)
        {
            SendInGameManager(GameProtocol.LEVEL_UP_DICE_REQ, (short) resetFieldNum, (short) levelUpFieldNum);
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
                case GameProtocol.LEVEL_UP_DICE_ACK:
                {
                    MsgLevelUpDiceAck lvupDiceack = (MsgLevelUpDiceAck) param[0];
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
                    OnOtherLeft((int) param[0]);
                    break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY:
                {
                    if (NetworkManager.Get().UserUID == (int) param[0]) // param 0 = useruid
                    {
                        NetSetSp((int) param[1]); // param1 wave
                    }

                    NetStartGame();
                    break;
                }
                case GameProtocol.ADD_SP_NOTIFY:
                {
                    if (NetworkManager.Get().UserUID == (int) param[0] ) // param 0 = useruid
                    {
                        NetSetSp((int) param[1]); // param1 wave
                    }

                    break;
                }
                case GameProtocol.SPAWN_NOTIFY:
                {
                    NetSpawnNotify((int) param[0]);
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
                case GameProtocol.LEVEL_UP_DICE_NOTIFY:
                {
                    MsgLevelUpDiceNotify lvupdiceNoti = (MsgLevelUpDiceNotify) param[0];
                    
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

                    // 이긴 사람 id 가 나면 내가 승
                    if (NetworkManager.Get().UserUID == endNoti.WinPlayerUId )
                    {
                        EndGame(true);
                    }
                    else
                    {
                        // 승리자 uid 내가 아닌 상대방
                        EndGame(false);
                    }
                    break;
                }
                case GameProtocol.PAUSE_GAME_NOTIFY:
                {
                    MsgPauseGameNotify pauseNoti = (MsgPauseGameNotify) param[0];

                    if (NetworkManager.Get().UserUID != pauseNoti.PlayerUId)
                    {
                        NetworkManager.Get().SetOtherPause(true);
                    }
                    
                    break;
                }
                case GameProtocol.RESUME_GAME_ACK:
                {
                    MsgResumeGameAck resumeack = (MsgResumeGameAck) param[0];
                    
                    break;
                }
                case GameProtocol.RESUME_GAME_NOTIFY:
                {
                    MsgResumeGameNotify resumeNoti = (MsgResumeGameNotify) param[0];
                    
                    break;
                }
                
                
                case GameProtocol.DISCONNECT_GAME_NOTIFY:
                {
                    MsgDisconnectGameNotify disNoti = (MsgDisconnectGameNotify) param[0];
                    
                    // 상대가 나갓다
                    if (NetworkManager.Get().UserUID != disNoti.PlayerUId)
                    {
                    }
                    
                    break;
                }
                case GameProtocol.RECONNECT_GAME_NOTIFY:
                {
                    MsgReconnectGameNotify reconnNoti = (MsgReconnectGameNotify) param[0];
                    
                    break;
                }
                case GameProtocol.RECONNECT_GAME_ACK:
                {
                    MsgReconnectGameAck reconnack = (MsgReconnectGameAck) param[0];
                    
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


        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                print("Application Pause");
                // 일시정지
                SendInGameManager(GameProtocol.PAUSE_GAME_REQ);
            }
            else
            {
                StartCoroutine(ResumeDelay());
            }
        }

        IEnumerator ResumeDelay()
        {
            // resume 신호 -- player controll 에서 혹시 모를 릴레이 패킷들 다 패스 시키기위해
            NetworkManager.Get().SetResume(true);
            
            // resume 을 하는 client 라면..
            // 인디케이터 -- 어차피 재동기화 위해 데이터 날려야됨
            UI_InGamePopup.Get().ViewGameIndicator(true);
                    
            
            yield return new WaitForSeconds(2.0f);
            
            print("Application Resume");
            if (NetworkManager.Get().IsConnect())
            {
                // resume
                SendInGameManager(GameProtocol.RESUME_GAME_REQ);
            }
            else
            {
                // 어차피 여기서 할필요가 없군..network 에서 씬을 보내버리니...
                // reconnect --> go
            }
        }
        

#if UNITY_EDITOR
        // 에디터에서 테스트용도로 사용하기 위해
        public void OnEditorAppPause(PauseState pause)
        {
            if (pause == PauseState.Paused)
            {
                print("Application Pause");
                // 일시정지
                SendInGameManager(GameProtocol.PAUSE_GAME_REQ);
            }
            else
            {
                print("Application Resume");
                StartCoroutine(ResumeDelay());
            }
        }
#endif

        

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
    }
}
