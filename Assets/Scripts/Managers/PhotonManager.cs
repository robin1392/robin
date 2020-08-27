#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public enum PLAY_TYPE
{
    BATTLE,
    CO_OP,
}

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;
    public PLAY_TYPE playType;

    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    private const string GameVersion = "1";
    public bool isConnecting;
    public float aiModeWaitTime = 5f;
    private bool _isAiMode;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        isConnecting = false;
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
    }

    private void OnApplicationQuit()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }        
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Photon Connected !!");
            isConnecting = false;
        }
        else
        {
            isConnecting = true;
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Disconnect()
    {
        StopAllCoroutines();
        PhotonNetwork.Disconnect();
        isConnecting = false;
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to server");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN: OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            //PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public AsyncOperation async;
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);

        if (_isAiMode)
        {
            _isAiMode = false;
            async = SceneManager.LoadSceneAsync("InGame_Battle");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        var ro = new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[] {"PlayType"},
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"PlayType", playType}},
            MaxPlayers = maxPlayersPerRoom
        };
        PhotonNetwork.CreateRoom(null, ro);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN: OnJoinedRoom() called by PUN. Now this client is in a room.");

        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["PlayType"]);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            StartCoroutine(WaitUserCoroutine());
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        StopAllCoroutines();
    }

    private IEnumerator WaitUserCoroutine()
    {
        //var t = 0f;
        while (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            yield return null;

            // t += Time.deltaTime;
            // if (t > aiModeWaitTime)
            // {
            //     _isAiMode = true;
            //     PhotonNetwork.Disconnect();
            //     yield break;
            // }
        }

        if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient) yield break;
        Debug.Log("We load the 'InGame'");

        switch (playType)
        {
            case PLAY_TYPE.BATTLE:
                PhotonNetwork.LoadLevel("InGame_Battle");
                break;
            case PLAY_TYPE.CO_OP:
                PhotonNetwork.LoadLevel("InGame_Coop");
                break;
            default:
                break;
        }
    }

    public void JoinRoom(PLAY_TYPE type)
    {
        playType = type;
        var h = new ExitGames.Client.Photon.Hashtable() { { "PlayType", playType } };
        PhotonNetwork.JoinRandomRoom(h, maxPlayersPerRoom);
    }
}
