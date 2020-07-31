#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;

namespace ED
{
    public class IntEvent : UnityEvent<int> { }

    public class InGameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static InGameManager Instance;

        public PLAY_TYPE playType;
        public Data_AllDice data_AllDice;
        public GameObject pref_Player;
        public GameObject pref_AI;
        public Transform ts_Lights;
        public Transform ts_StadiumTop;
        public Transform ts_NexusHealthBar;
        public Transform ts_TopPlayer;
        public Transform ts_BottomPlayer;
        public bool isAIMode;
        public bool isGamePlaying;

        [Header("UI Link")]
        public Image image_SpawnTime;
        public Text text_SpawnTime;
        public TextMeshProUGUI tmp_Wave;
        public Text text_SP;
        public Text text_GetDiceButton;
        public UI_GetDiceButton btn_GetDice;
        public GameObject popup_Result;
        public Text text_Result;
        public UI_UpgradeButton[] arrUpgradeButtons;
        public Image image_BottomHealthBar;
        public Text text_BottomHealth;
        public Image image_TopHealthBar;
        public Text text_TopHealth;
        public GameObject popup_Waiting;
        public GameObject obj_ViewTargetDiceField;
        public GameObject obj_Low_HP_Effect;
        public Button button_SP_Upgrade;
        public Text text_SP_Upgrade;
        public Text text_SP_Upgrade_Price;

        [HideInInspector]
        public PlayerController playerController;
        
        [Space]
        public int wave = 0;
        public float startSpawnTime = 10f;
        public float spawnTime = 45f;
        [SerializeField]
        protected int[] arrUpgradeLevel;
        private float st => wave < 1 ? 10f : 20f;

        public float time { get; protected set; }

        private int _readyPlayerCount = 0;
        public int getDiceCost => 10 + getDiceCount * 10;
        public int getDiceCount = 0;

        [SerializeField]
        protected List<BaseStat> listBottomPlayer = new List<BaseStat>();
        [SerializeField]
        protected List<BaseStat> listTopPlayer = new List<BaseStat>();

        public IntEvent event_SP_Edit;

        public Text text_UnitCount;

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        protected void OnDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
            }
        }

        protected virtual void Start()
        {
            arrUpgradeLevel = new int[6];
            event_SP_Edit = new IntEvent();

            if (PhotonNetwork.IsConnected)
            {
                popup_Waiting.SetActive(true);

                if (PlayerController.Instance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.identifier);

                    var startPos = PhotonNetwork.IsMasterClient ? ts_BottomPlayer.position : ts_TopPlayer.position;
                    var obj = PhotonNetwork.Instantiate("Tower/" + pref_Player.name, startPos, Quaternion.identity, 0);
                    obj.transform.parent = PhotonNetwork.IsMasterClient ? ts_BottomPlayer : ts_TopPlayer;
                    playerController = obj.GetComponent<PlayerController>();
                    playerController.photonView.RPC("ChangeLayer", RpcTarget.All, PhotonNetwork.IsMasterClient);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            else
            {
                var obj = Instantiate(pref_Player, ts_BottomPlayer.position, Quaternion.identity);
                obj.transform.parent = ts_BottomPlayer;
                playerController = obj.GetComponent<PlayerController>();
                playerController.ChangeLayer(true);
                playerController.isMine = true;
                playerController.isBottomPlayer = true;

                obj = Instantiate(pref_AI, ts_TopPlayer.position, Quaternion.identity);
                obj.transform.parent = ts_TopPlayer;
                obj.SendMessage("ChangeLayer", false);

                isAIMode = true;
            }

            var deck = ObscuredPrefs.GetString("Deck", "0/1/2/3/4");
            if (PhotonNetwork.IsConnected)
            {
                playerController.photonView.RPC("SetDeck", RpcTarget.All, deck);
            }
            else
            {
                playerController.SetDeck(deck);
            }

            // Upgrade buttons
            for (var i = 0; i < arrUpgradeButtons.Length; i++)
            {
                arrUpgradeButtons[i].Initialize(playerController.arrDeck[i], arrUpgradeLevel[i]);
            }

            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient == false)
                {
                    ts_StadiumTop.localRotation = Quaternion.Euler(180f, 0, 180f);
                    ts_NexusHealthBar.localRotation = Quaternion.Euler(0, 0, 180f);
                    ts_Lights.localRotation = Quaternion.Euler(0, 340f, 0);
                }
            }

            obj_ViewTargetDiceField.SetActive(!PhotonNetwork.IsConnected);
            event_SP_Edit.AddListener(RefreshSP);
            event_SP_Edit.AddListener(SetSPUpgradeButton);

            StartGame();
            RefreshTimeUI(true);
        }

        protected void Update()
        {
            RefreshTimeUI();
            text_UnitCount.text = $"총 유닛수: {listBottomPlayer.Count + listTopPlayer.Count - 2}";
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {
                //SceneManager.LoadScene("Main");
                GameStateManager.Get().MoveMainScene();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);

            //SceneManager.LoadSceneAsync("Main");
            GameStateManager.Get().MoveMainScene();
        }

        //public override void OnPlayerEnteredRoom(Player newPlayer)
        //{

        //}

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (isGamePlaying)
            {
                PhotonNetwork.Disconnect();
            }
        }

        protected void StartGame()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Ready();
                    StartCoroutine(SpawnLoop());
                }
                else
                {
                    photonView.RPC("Ready", RpcTarget.MasterClient);
                }
            }
            else
            {
                Debug.Log("StartGame: OfflineMode");
                StartCoroutine(SpawnLoop());
            }
        }

        [PunRPC]
        private void Ready()
        {
            _readyPlayerCount++;
        }

        [PunRPC]
        private void DeactivateWaitingObject()
        {
            isGamePlaying = true;
            popup_Waiting.SetActive(false);
        }

        private IEnumerator SpawnLoop()
        {
            if (PhotonNetwork.IsConnected)
            {
                while (PhotonNetwork.InRoom && _readyPlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers) { yield return null; }
                photonView.RPC("DeactivateWaitingObject", RpcTarget.All);
            }
            else
            {
                DeactivateWaitingObject();
            }

            wave = 0;
            tmp_Wave.text = $"{wave}";
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
                        photonView.RPC("AddSP", RpcTarget.All, wave);
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
                        photonView.RPC("SpawnPlayerMinions", RpcTarget.All);
                    }
                    else
                    {
                        playerController.Spawn();
                        playerController.targetPlayer.Spawn();
                    }

                    wave++;
                }
            }
        }

        protected void RefreshTimeUI(bool isImmediately = false)
        {
            if (isImmediately)
            {
                image_SpawnTime.fillAmount = time / st;
            }
            else
            {
                var f = Mathf.Lerp(image_SpawnTime.fillAmount, time / st, Time.deltaTime * 5f);
                
                if (f < image_SpawnTime.fillAmount) image_SpawnTime.fillAmount = f;
                else image_SpawnTime.fillAmount = time / st;
            }
            text_SpawnTime.text = $"{Mathf.CeilToInt(time):F0}";
            tmp_Wave.text = $"{wave}";
        }

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

        [PunRPC]
        public void SpawnPlayerMinions()
        {
            image_SpawnTime.fillAmount = 1f;
            playerController.photonView.RPC("Spawn", RpcTarget.All);
        }

        public void GetDice()
        {
            playerController.AddSp(-getDiceCost);
            getDiceCount++;
            playerController.AddSp(0);
            text_GetDiceButton.text = $"{getDiceCost}";
        }

        [PunRPC]
        private void AddSP(int wave = 0)
        {
            playerController.AddSpByWave(wave);

            if (PhotonNetwork.IsConnected == false)
            {
                playerController.targetPlayer.AddSpByWave(wave);
            }
        }

        private void RefreshSP(int sp)
        {
            text_SP.text = sp.ToString();
        }

        [PunRPC]
        public void EndGame(PhotonMessageInfo info)
        {
            isGamePlaying = false;
            StopAllCoroutines();
            popup_Result.SetActive(true);
            BroadcastMessage("EndGameUnit", SendMessageOptions.DontRequireReceiver);

            text_Result.text = playerController.isAlive ? "승리" : "패배";
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
        
        public BaseStat GetRandomPlayerUnit(bool isBottomPlayer)
        {
            return isBottomPlayer ? listBottomPlayer[Random.Range(0, listBottomPlayer.Count)] : listTopPlayer[Random.Range(0, listTopPlayer.Count)];
        }

        public Vector3 GetRandomPlayerFieldPosition(bool isBottomPlayer)
        {
            var x = Random.Range(-3f, 3f);
            var z = Random.Range(-2f, 2f);
            return new Vector3(x, 0, z);
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

        private void SetSPUpgradeButton(int sp)
        {
            button_SP_Upgrade.interactable = (playerController.spUpgradeLevel + 1) * 500 <= sp;
            text_SP_Upgrade.text = $"SP Lv.{playerController.spUpgradeLevel + 1}";
            text_SP_Upgrade_Price.text = $"{(playerController.spUpgradeLevel + 1) * 500}";
        }

        public void Click_SP_Upgrade_Button()
        {
            playerController.SP_Upgrade();
        }
    }
}
