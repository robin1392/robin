﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

using Service.Net;
using Template.Account.RandomWarsAccount;
using Template.Account.RandomWarsAccount.Common;
using Template.Item.RandomWarsDice;
using Template.Item.RandomWarsDice.Common;
using Template.Player.RandomWarsPlayer;
using Template.Player.RandomWarsPlayer.Common;
using Template.Stage.RandomWarsMatch;
using Template.Stage.RandomWarsMatch.Common;



public class NetService : Singleton<NetService>
{
    NetServiceClient _netGameClient;
    HttpClient _httpClient;

    MessageController _messageController;

    RandomWarsAccountTemplate _randomWarsAccountTemplate;
    RandomWarsPlayerTemplate _randomWarsPlayerTemplate;
    RandomWarsDiceTemplate _randomWarsDiceTemplate;
    RandomWarsMatchTemplate _randomWarsMatchTemplate;

    NetClientGameSession _gameSession;
    NetClientPlayer _gamePlayer;

    string _tokenSerializePath;
    string _playerSessionId;
    string _gameSessionId;
    string _serverAddr;
    int _port;
    long _playerTimeStampTick;
    long _netClientTokenSerializeTick;

    public Global.E_MATCHSTEP NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
    public Global.PLAY_TYPE playType;


    #region unity base
    public override void Awake()
    {
        if (NetService.Get() != null && this != NetService.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        base.Awake();
    }


    void Start()
    {
        InitNetClient();
    }


    void Update()
    {
        UpdateNetClient();

        _gameSession.Update();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion


    #region netClient

    void InitNetClient()
    {
        _netGameClient = new NetServiceClient();
        _netGameClient.Init(new NetServiceConfig
        {
            MaxConnectionNum = 1,
            BufferSize = 10240,
            KeepAliveTime = 5000,
            KeepAliveInterval = 1000
        });

        _randomWarsAccountTemplate = new RandomWarsAccountTemplate();
        _randomWarsPlayerTemplate = new RandomWarsPlayerTemplate();
        _randomWarsDiceTemplate = new RandomWarsDiceTemplate();
        _randomWarsMatchTemplate = new RandomWarsMatchTemplate();


        _messageController = new MessageController();
        _messageController.AddControllers(_randomWarsAccountTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsPlayerTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsDiceTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsMatchTemplate.MessageControllers);


        _gameSession = new NetClientGameSession();
        _gameSession.Init(new GameSessionConfig
        {
            MessageBufferSize = 10240,
            MessageQueueCapacity = 100,
            MsgController = _messageController,
        });

        _netGameClient.SetGameSession(_gameSession);


        _httpClient = new HttpClient(
            //"https://vj7nnp92xd.execute-api.ap-northeast-2.amazonaws.com/prod", 
            "http://localhost:5001/api",
            _gameSession);



        _tokenSerializePath = Application.persistentDataPath + "/NetClientToken.bytes";
        _netClientTokenSerializeTick = 0;
    }


    void UpdateNetClient()
    {
        if (_netGameClient == null)
        {
            return;
        }

        _netGameClient.Update();

        DateTime nowTime = DateTime.UtcNow;
        if (IsConnected() == true 
            && _netClientTokenSerializeTick != 0
            && _netClientTokenSerializeTick < nowTime.Ticks)
        {
            _netClientTokenSerializeTick = nowTime.Ticks + (10 * TimeSpan.TicksPerSecond);
            SerializeNetClientToken();
        }
    }


    public void CreateGameSession(string serverAddr, int port, string gameSessionId, string playerSessionId)
    {
        _playerSessionId = playerSessionId;
        _gameSessionId = gameSessionId;
        _serverAddr = serverAddr;
        _port = port;

        NetClientGameSession gameSession = new NetClientGameSession();
        gameSession.Init(new GameSessionConfig
        {
            Id = gameSessionId,
            MessageBufferSize = 10240,
            MessageQueueCapacity = 100,
            MsgController = _messageController,
        });
        _netGameClient.AddGameSession(gameSession);
        _gamePlayer = gameSession.Player;
    }


    public bool IsConnected()
    {
        return true;
    }


    public void ConnectGameServer(Global.PLAY_TYPE playType, string serverAddr, int port, string playerSessionId)
    {
        NetMatchStep = Global.E_MATCHSTEP.MATCH_CONNECT;
        _serverAddr = serverAddr;
        _port = port;
        _playerSessionId = playerSessionId;

        _netGameClient.Connect(_serverAddr, _port, _gameSessionId, _playerSessionId, ENetState.Connecting);
    }


    public bool ReconnectGameServer()
    {
        _netGameClient.Connect(_serverAddr, _port, _gameSessionId, _playerSessionId, ENetState.Reconnecting);
        return true;
    }


    void DisconnectGameServer()
    {
        _netGameClient.Disconnect(_gamePlayer, ESessionState.None);
    }


    void SerializeNetClientToken()
    {
        Dictionary<string, string> values = new Dictionary<string, string>
        {
            { "serverAddr", _serverAddr },
            { "port", _port.ToString()},
            { "gameSessionId", _gameSessionId },
            { "playerSessionId", _playerSessionId },
            { "playerTimeStampTick", DateTime.UtcNow.Ticks.ToString() },
        };


        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_tokenSerializePath, FileMode.OpenOrCreate);
        formatter.Serialize(stream, values);
        stream.Close();
    }


    bool DeserializeNetClientToken()
    {
        if (File.Exists(_tokenSerializePath) == false)
        {
            return false;
        }


        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_tokenSerializePath, FileMode.Open);
        Dictionary<string, string> values = (Dictionary<string, string>)formatter.Deserialize(stream);
        _serverAddr = values["serverAddr"];
        _port = int.Parse(values["port"]);
        _gameSessionId = values["gameSessionId"];
        _playerSessionId = values["playerSessionId"];
        _playerTimeStampTick = long.Parse(values["playerTimeStampTick"]);

        stream.Close();
        return true;
    }



    #endregion


    #region send
    public bool Send(ERandomWarsAccountProtocol protocolId, params object[] param)
    {
        switch(protocolId)
        {
            case ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ:
                {
                    _randomWarsAccountTemplate.SendLoginAccountReq(_httpClient, param[0].ToString(), (EPlatformType)param[1]);
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsPlayerProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsPlayerProtocol.EDIT_NAME_REQ:
                {
                    _randomWarsPlayerTemplate.SendEditNameReq(_httpClient, param[0].ToString(), param[1].ToString());
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsDiceProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsDiceProtocol.UPDATE_DECK_REQ:
                {
                    _randomWarsDiceTemplate.SendUpdateDeckReq(_httpClient, param[0].ToString(), (int)param[1], (int[])param[2]);
                }
                break;
            case ERandomWarsDiceProtocol.LEVELUP_DICE_REQ:
                {
                    _randomWarsDiceTemplate.SendLevelupDiceReq(_httpClient, param[0].ToString(), (int)param[1]);
                }
                break;
            case ERandomWarsDiceProtocol.OPEN_BOX_REQ:
                {
                    _randomWarsDiceTemplate.SendOpenBoxReq(_httpClient, param[0].ToString(), (int)param[1]);
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsMatchProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsMatchProtocol.REQUEST_MATCH_REQ:
                {
                    if (NetMatchStep == Global.E_MATCHSTEP.MATCH_START
                        || NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
                    {
                        // 매칭 중이면 요청할 수 없다.
                        break;
                    }

                    NetMatchStep = Global.E_MATCHSTEP.MATCH_START;
                    _randomWarsMatchTemplate.SendRequestMatchReq(_httpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.STATUS_MATCH_REQ:
                {
                    if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
                    {
                        // 매칭 중에서 상태 요청을 할 수 있다ㅏ.
                        break;
                    }

                    _randomWarsMatchTemplate.SendStatusMatchReq(_httpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.CANCEL_MATCH_REQ:
                {
                    if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
                    {
                        break;
                    }

                    NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
                    _randomWarsMatchTemplate.SendCancelMatchReq(_httpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.JOIN_STAGE_REQ:
                {
                    _randomWarsMatchTemplate.SendJoinStageReq(_gamePlayer, param[0].ToString(), (int)param[1]);
                }
                break;
        }

        UnityUtil.Print("SEND MATCH STATUS => ", string.Format("protocolId:{0}, params:{1}", protocolId.ToString(), string.Join(", ", param)), "green");

        return true;
    }


    #endregion

}