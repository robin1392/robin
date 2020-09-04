#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using RWGameProtocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

#region USING PHOTON
using Photon.Pun;
using Photon.Realtime;
using CodeStage.AntiCheat.ObscuredTypes;
#endregion

namespace ED
{
    public class InGameManager : SingletonPhoton<InGameManager>, IPunObservable
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
        [Header("WAVE INFO")]
        public int wave = 0;
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

        [SerializeField]
        protected List<BaseStat> listBottomPlayer = new List<BaseStat>();
        [SerializeField]
        protected List<BaseStat> listTopPlayer = new List<BaseStat>();

        private readonly string recvMessage = "RecvBattleManager";
        
        #endregion
        
        
        
        /// <summary>
        /// 삭제 -- Canvas 위치에 따라 world ui 와 in game ui , popup 으로 나눔 
        /// </summary>
        //[Header("UI Link")]
        //public Image image_SpawnTime;
        //public Text text_SpawnTime;
        //public TextMeshProUGUI tmp_Wave;
        //public Text text_SP;
        //public Text text_GetDiceButton;
        //public UI_GetDiceButton btn_GetDice;
        //public GameObject popup_Result;
        //public Text text_Result;
        //public UI_UpgradeButton[] arrUpgradeButtons;
        
        //public Image image_BottomHealthBar;
        //public Text text_BottomHealth;
        //public Image image_TopHealthBar;
        //public Text text_TopHealth;
        
        //public GameObject popup_Waiting;
        
        //public GameObject obj_ViewTargetDiceField;
        //public GameObject obj_Low_HP_Effect;
        //public Button button_SP_Upgrade;
        //public Text text_SP_Upgrade;
        //public Text text_SP_Upgrade_Price;

        //public Text text_UnitCount;

        #region unity base
        public override void Awake()
        {
            //if (Instance == null)
            //{
                //Instance = this;
            //}
            base.Awake();

            Application.targetFrameRate = 30;
            InitializeManager();
        }

        public override void OnDestroy()
        {
            //if(Instance == this)
            //{
                //Instance = null;
            //}
            
            DestroyManager();
            
            base.OnDestroy();
        }

        protected virtual void Start()
        {
            // 개발 버전이라..중간에서 실행햇을시에..
            if(DataPatchManager.Get().isDataLoadComplete == false )
                DataPatchManager.Get().JsonDownLoad();

            // 전체 주사위 정보
            data_DiceInfo = JsonDataManager.Get().dataDiceInfo;
            
            // 위치를 옮김.. 차후 데이터 로딩후 풀링을 해야되서....
            PoolManager.Get().MakePool();

            
            StartManager();
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
            //text_UnitCount.text = $"총 유닛수: {listBottomPlayer.Count + listTopPlayer.Count - 2}";
            if(UI_InGame.Get() != null)
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

        public bool IsNetwork()
        {
            if (NetworkManager.Get() != null && NetworkManager.Get().IsConnect())
                return true;

            return false;
        }

        public void StartManager()
        {

            if ( IsNetwork() == true)
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
                playerController.targetPlayer.ChangeLayer(NetworkManager.Get().GetNetInfo().otherInfo.IsBottomPlayer);

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
            
            // deck setting
            //NetworkManager.Get().GetNetInfo().playerInfo.DiceInfoArray
            //NetworkManager.Get().GetNetInfo().otherInfo.DiceInfoArray
            
            
            
            if ( IsNetwork() == true)
            {
                //playerController.photonView.RPC("SetDeck", RpcTarget.All, deck);
                //playerController.SendPlayer(RpcTarget.All , E_PTDefine.PT_SETDECK , deck);
                
                // my
                playerController.SetDeck(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray);
                //other
                playerController.targetPlayer.SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            }
            else
            {
                // 네트워크 안쓰니까...개발용으로
                var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
                if (UserInfoManager.Get() != null)
                {
                    deck = UserInfoManager.Get().GetActiveDeck();
                }
                playerController.SetDeck(deck);
            }
            
            // Upgrade buttons
            // ui 셋팅
            UI_InGame.Get().SetArrayDeck(playerController.arrDiceDeck , arrUpgradeLevel);

            
            if ( IsNetwork() == true)
            {
                if (NetworkManager.Get().IsMaster == false)
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

            
            if ( IsNetwork() == true)
            {
                SendInGameManager(GameProtocol.READY_GAME_REQ);
            }
            else
            {
                StartGame();
                RefreshTimeUI(true);
            }
            
            
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

            var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
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
            if ( IsNetwork() == true)
            {
                StartCoroutine(SpawnLoop());
            }
            else
            {
                Debug.Log("StartGame: OfflineMode");
                StartCoroutine(SpawnLoop());
            }
                
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
            
            
            wave = 0;
            //tmp_Wave.text = $"{wave}";
            
            // 개발용으로 쓰일때만..
            if (IsNetwork() == false)
                WorldUIManager.Get().SetWave(wave);
            
            time = startSpawnTime;
            int[] arrAddTime = { 20, 15, 10, 5, -1 };
            var addNum = 0;

            while (true)
            {
                yield return null;
                
                time -= Time.deltaTime;

                //
                if (wave > 0 && time <= arrAddTime[addNum])
                {
                    addNum++;
                    if (IsNetwork() == true)
                    {
                        //...sp 를 노티가 오니....안해도 되겟네..
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

                    if (IsNetwork() == true)
                    {
                        // spawn 도 노티로 온다...
                    }
                    else
                    {
                        playerController.Spawn();
                        playerController.targetPlayer.Spawn();
                        
                        wave++;
                    }
                }
            }
        }

        public void NetSpawnNotify(int wave)
        {
            WorldUIManager.Get().SetWave(wave);
            
            playerController.Spawn();
            playerController.targetPlayer.Spawn();
        }

        #endregion
        
        
        #region net sp

        //[PunRPC]
        private void AddSP(int wave = 0)
        {
            playerController.AddSpByWave(wave);

            //if (PhotonNetwork.IsConnected == false)
            if (IsNetwork() == false)
            {
                playerController.targetPlayer.AddSpByWave(wave);
            }
        }


        #endregion
        
        
        #region update event
        
        private void RefreshSP(int sp)
        {
            //text_SP.text = sp.ToString();
            UI_InGame.Get().SetSP(sp);
        }

        
        protected void RefreshTimeUI(bool isImmediately = false)
        {
            if (isImmediately)
            {
                //image_SpawnTime.fillAmount = time / st;
                WorldUIManager.Get().SetSpawnTime(time / st);
            }
            else
            {
                //var f = Mathf.Lerp(image_SpawnTime.fillAmount, time / st, Time.deltaTime * 5f);
                //if (f < image_SpawnTime.fillAmount) image_SpawnTime.fillAmount = f;
                //else image_SpawnTime.fillAmount = time / st;
                
                float ff = Mathf.Lerp(WorldUIManager.Get().GetSpawnAmount(), time / st, Time.deltaTime * 5.0f);

                if (ff < WorldUIManager.Get().GetSpawnAmount()) WorldUIManager.Get().SetSpawnTime(ff);
                else WorldUIManager.Get().SetSpawnTime(time / st);
                
            }
            
            
            //text_SpawnTime.text = $"{Mathf.CeilToInt(time):F0}";
            WorldUIManager.Get().SetTextSpawnTime(time);
            //tmp_Wave.text = $"{wave}";
            WorldUIManager.Get().SetWave(wave);
        }

        #endregion

        
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
            UI_InGame.Get().SetSPUpgrade(playerController.spUpgradeLevel , sp);
        }
        #endregion
        
        
        #region leave game
        public void LeaveRoom()
        {
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
            
            if (NetworkManager.Get().IsConnect() == true)
            {
                SendInGameManager(GameProtocol.LEAVE_GAME_REQ);
            }
            else
            {
                GameStateManager.Get().MoveMainScene();
            }
        }
        
        // 내자신이 나간다고 눌럿을때
        public void CallBackLeaveRoom()
        {
            NetworkManager.Get().DisconnectSocket();
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
                SendInGameManager(GameProtocol.LEAVE_GAME_REQ);
            }
        }

        //[PunRPC]
        public void EndGame(PhotonMessageInfo info)
        {
            isGamePlaying = false;
            StopAllCoroutines();
            //popup_Result.SetActive(true);
            UI_InGamePopup.Get().SetPopupResult(true);
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);

            //text_Result.text = playerController.isAlive ? "승리" : "패배";
            UI_InGamePopup.Get().SetResultText(playerController.isAlive ? Global.g_inGameWin : Global.g_inGameLose);
        }
        #endregion
        
        
        #region rpc etc
        
        //[PunRPC]
        private void DeactivateWaitingObject()
        {
            isGamePlaying = true;
            //popup_Waiting.SetActive(false);
            UI_InGamePopup.Get().SetViewWaiting(false);
        }

        //[PunRPC]
        public void SpawnPlayerMinions()
        {
            //image_SpawnTime.fillAmount = 1f;
            WorldUIManager.Get().SetSpawnTime(1.0f);
            //playerController.photonView.RPC("Spawn", RpcTarget.All);
            playerController.SendPlayer(RpcTarget.All , E_PTDefine.PT_SPAWN);
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


        public void Click_SP_Upgrade_Button()
        {
            playerController.SP_Upgrade();
        }
        
        #endregion
        
        
        
        #region network

        public void SendInGameManager(GameProtocol protocol , params object[] param)
        {
            if ( NetworkManager.Get() != null && NetworkManager.Get().IsConnect())
            {
                NetworkManager.Get().Send(protocol , param);
            }
            else
            {
                RecvInGameManager(protocol, param);
            }
            
        }

        public void RecvInGameManager(GameProtocol protocol, params object[] param)
        {
            switch (protocol)
            {
                case GameProtocol.LEAVE_GAME_ACK:
                    CallBackLeaveRoom();
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    OnOtherLeft((int) param[0]);
                    break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY:
                    NetStartGame();
                    break;
                case GameProtocol.ADD_SP_NOTIFY:
                {
                    if (NetworkManager.Get().GetNetInfo().IsMyUID((int) param[0]) == true) // param 0 = useruid
                    {
                        AddSP((int)param[1]);    // param1 wave
                    }
                    break;
                }
                case GameProtocol.SPAWN_NOTIFY:
                {
                    NetSpawnNotify((int)param[0]);
                    break;
                }
                /*
                
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    break;*/
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
                time = (float)stream.ReceiveNext();
                wave = (int)stream.ReceiveNext();
            }
        }
        
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);

            //SceneManager.LoadSceneAsync("Main");
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
        public void SendBattleManager(RpcTarget target , E_PTDefine ptID , params object[] param)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(recvMessage, target , ptID , param);
            }
            else
            {
                RecvBattleManager(ptID, param);
            }
        }

        [PunRPC]
        public void RecvBattleManager(E_PTDefine ptID , params object[] param)
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
                    int addsp = (int)param[0];
                    AddSP(addsp);
                    break;
                case E_PTDefine.PT_SPAWNMINION:
                    SpawnPlayerMinions();
                    break;
                case E_PTDefine.PT_ENDGAME:
                    EndGame(new PhotonMessageInfo());
                    break;
                case E_PTDefine.PT_NICKNAME:
                    UI_InGame.Get().SetNickname((string)param[0]);
                    break;
            }
        }
        #endregion

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
    }
}
