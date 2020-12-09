#if UNITY_EDITOR
#define ENABLE_LOG
#endif


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

using Service.Net;
using Template.Player.RandomWarsPlayer.Common;
using Template.Stage.RandomWarsMatch.Common;
using Template.Account.RandomWarsAccount.Common;
using Template.Item.RandomWarsDice.Common;


public class NetClientService : Singleton<NetClientService>
{
    NetServiceClient _netGameClient;
    HttpClient _httpClient;

    NetClientGameSession _gameSession;

    RandomWarsAccountProtocol _randomWarsAccountProtocol;
    RandomWarsMatchProtocol _randomWarsMatchProtocol;
    RandomWarsPlayerProtocol _randomWarsPlayerProtocol;
    RandomWarsDiceProtocol _randomWarsDiceProtocol;



    #region unity base
    public override void Awake()
    {
        if (NetworkManager.Get() != null && this != NetworkManager.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        base.Awake();
    }


    void Start()
    {
        InitController();
    }


    void Update()
    {

    }
    #endregion


    #region netClient
    void InitController()
    {
        _gameSession = null;

        _randomWarsAccountProtocol.HttpReceiveLoginAccountAckCallback = OnReceiveLoginAccountAck;

    }

    public void CreateGameSession(string serverAddr, int port, string gameSessionId, string playerSessionId)
    {
        MessageController messageController = new MessageController();
        messageController.AddControllers(_randomWarsAccountProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsMatchProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsPlayerProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsDiceProtocol.MessageControllers);


        _gameSession = new NetClientGameSession();
        _gameSession.Init(new GameSessionConfig
        {
            ServerAddr = serverAddr,
            Port = port,
            Id = gameSessionId,
            MsgController = messageController,
            MessageBufferSize = 10240,
            MessageQueueCapacity = 100,
        });
        _gameSession.GamePlayer.PlayerSessionId = playerSessionId;

        _netGameClient.AddGameSession(_gameSession);
    }

    public void CreateGameSession(string binarySerializePath)
    {
        MessageController messageController = new MessageController();
        messageController.AddControllers(_randomWarsAccountProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsMatchProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsPlayerProtocol.MessageControllers);
        messageController.AddControllers(_randomWarsDiceProtocol.MessageControllers);


        _gameSession = new NetClientGameSession();
        _gameSession.Init(new GameSessionConfig
        {
            ServerAddr = serverAddr,
            Port = port,
            Id = gameSessionId,
            MsgController = messageController,
            MessageBufferSize = 10240,
            MessageQueueCapacity = 100,
        });
        _gameSession.GamePlayer.PlayerSessionId = playerSessionId;

        _netGameClient.AddGameSession(_gameSession);
    }



    public void ConnectGameServer()
    {
        _netGameClient.Connect(_gameSession.ServerAddr, _gameSession.Port, _gameSession.Id, _gameSession.GamePlayer.PlayerSessionId, ENetState.Connecting);
    }

    public bool ReconnectGameServer()
    {
        if (_gameSession == null)
        {
            // 플레이 상태 파일 체크


            CreateGameSession();
            return false;
        }



        _netGameClient.Connect(_gameSession.ServerAddr, _gameSession.Port, _gameSession.Id, _gameSession.GamePlayer.PlayerSessionId, ENetState.Reconnecting);


        return true;
    }


    void DisconnectGameServer()
    {
        _netGameClient.Disconnect(_gameSession.GamePlayer, ESessionState.None);
    }


    #endregion


    #region send
    public bool Send(ERandomWarsAccountProtocol protocolId, params object[] param)
    {
        return true;
    }


    public bool Send(ERandomWarsMatchProtocol protocolId, params object[] param)
    {
        return true;
    }


    public bool Send(ERandomWarsPlayerProtocol protocolId, params object[] param)
    {
        return true;
    }


    public bool Send(ERandomWarsDiceProtocol protocolId, params object[] param)
    {
        return true;
    }
    #endregion


    #region receive
    bool OnReceiveLoginAccountAck(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo)
    {
        if (errorCode == ERandomWarsAccountErrorCode.NOT_FOUND_ACCOUNT)
        {
            ObscuredPrefs.SetString("UserKey", string.Empty);
            ObscuredPrefs.Save();

            UI_Start.Get().SetTextStatus(string.Empty);
            UI_Start.Get().btn_GuestAccount.gameObject.SetActive(true);
            UI_Start.Get().btn_GuestAccount.onClick.AddListener(() =>
            {
                UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);
                
                //AuthUserReq(string.Empty);
            });
            return true;
        }

        UserInfoManager.Get().SetAccountInfo(accountInfo);
        GameStateManager.Get().UserAuthOK();

        UnityUtil.Print("RECV AUTH => PlayerGuid", accountInfo.PlayerInfo.PlayerGuid, "green");
        return true;
    }
    #endregion

}
