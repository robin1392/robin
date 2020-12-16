#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

using Service.Net;



public class NetworkService : Singleton<NetworkService>
{
    public NetServiceClient NetGameClient { get; set; }
    public HttpClient HttpClient { get; set; }
    public NetClientGameSession GameSession { get; set; }

    string _tokenSerializePath;
    string _playerSessionId;
    string _serverAddr;
    int _port;
    long _playerTimeStampTick;
    long _netClientTokenSerializeTick;

    public Global.E_MATCHSTEP NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
    public Global.PLAY_TYPE playType;


    #region unity base
    public override void Awake()
    {
        if (NetworkService.Get() != null && this != NetworkService.Get())
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

        if (GameSession != null)
        {
            GameSession.Update();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion


    #region netClient

    void InitNetClient()
    {
        NetGameClient = new NetServiceClient();
        NetGameClient.Init(new NetServiceConfig
        {
            MaxConnectionNum = 1,
            BufferSize = 10240,
            KeepAliveTime = 5000,
            KeepAliveInterval = 1000
        });


        GameSession = new NetClientGameSession();
        GameSession.Init(new GameSessionConfig
        {
            MessageBufferSize = 10240,
            MessageQueueCapacity = 100,
        });

        NetGameClient.SetGameSession(GameSession);


        HttpClient = new HttpClient(
            "https://vj7nnp92xd.execute-api.ap-northeast-2.amazonaws.com/prod", 
            //"https://localhost:5001/api",
            GameSession);



        _tokenSerializePath = Application.persistentDataPath + "/NetClientToken.bytes";
        _netClientTokenSerializeTick = 0;
    }


    void UpdateNetClient()
    {
        if (NetGameClient == null)
        {
            return;
        }

        NetGameClient.Update();

        DateTime nowTime = DateTime.UtcNow;
        if (IsConnected() == true 
            && _netClientTokenSerializeTick != 0
            && _netClientTokenSerializeTick < nowTime.Ticks)
        {
            _netClientTokenSerializeTick = nowTime.Ticks + (10 * TimeSpan.TicksPerSecond);
            SerializeNetClientToken();
        }
    }


    public void CreateGameSession(string serverAddr, int port, string playerSessionId)
    {
        //_playerSessionId = playerSessionId;
        //_serverAddr = serverAddr;
        //_port = port;

        //NetClientGameSession gameSession = new NetClientGameSession();
        //gameSession.Init(new GameSessionConfig
        //{
        //    Id = gameSessionId,
        //    MessageBufferSize = 10240,
        //    MessageQueueCapacity = 100,
        //});

        //_netGameClient.SetGameSession(gameSession);
        //_gamePlayer = gameSession.Player;
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
        NetGameClient.Connect(_serverAddr, _port, _playerSessionId, ENetState.Connecting);
    }


    public bool ReconnectGameServer()
    {
        NetGameClient.Connect(_serverAddr, _port, _playerSessionId, ENetState.Reconnecting);
        return true;
    }


    void DisconnectGameServer()
    {
        NetGameClient.Disconnect(GameSession.Player, ESessionState.None);
    }


    void SerializeNetClientToken()
    {
        Dictionary<string, string> values = new Dictionary<string, string>
        {
            { "serverAddr", _serverAddr },
            { "port", _port.ToString()},
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
        _playerSessionId = values["playerSessionId"];
        _playerTimeStampTick = long.Parse(values["playerTimeStampTick"]);

        stream.Close();
        return true;
    }



    #endregion

}
