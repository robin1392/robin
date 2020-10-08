using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using RWCoreNetwork;
using RWCoreNetwork.NetService;
using RWCoreNetwork.NetPacket;
using System;
using ED;


class NetLogger : RWCoreLib.Log.ILog
{
    public void Info(string log)
    {
        UnityEngine.Debug.Log(log);
    }
    public void Fatal(string log)
    {
        UnityEngine.Debug.LogError(log);
    }
    public void Error(string log)
    {
        UnityEngine.Debug.LogError(log);
    }
    public void Warn(string log)
    {
        UnityEngine.Debug.LogWarning(log);
    }
    public void Debug(string log)
    {
        UnityEngine.Debug.Log(log);
    }
}

public class SocketManager
{
    private NetClientService _netService;
    
    private Peer _serverPeer;
    public Peer Peer
    {
        get => _serverPeer;
    }


    private Action _connectCallBack;
    
    public SocketManager()
    {
        _netService = null;
        _serverPeer = null;
    }

    public void Init(IPacketReceiver recvProcessor)
    {
        NetLogger netLogger = new NetLogger();
        PacketHandler handler = new PacketHandler(recvProcessor, netLogger, 30, 20480);
        _netService = new NetClientService(handler, netLogger, 1, 20480, 5000, 1000, false);
        _netService.ClientConnectedCallback += OnClientConnected;
        _netService.ClientDisconnectedCallback += OnClientDisconnected;
        _netService.ClientOnlineCallback += OnClientReconnected;
    }


    public void Connect(string host, int port , string clientSessionId, Action connectCallback = null)
    {
        _netService.ClientSession.ClientSessionId = clientSessionId;
        _netService.Connect(host, port);
        _connectCallBack = connectCallback;
    }

    public void Disconnect()
    {
        if (_serverPeer != null)
        {
            _netService.Disconnect();
            _serverPeer = null;    
        }
    }
    
    public bool IsConnected()
    {
        return _serverPeer != null;
    }
    
    
    
    /// <summary>
    /// 서버 연결 성공 콜백
    /// </summary>
    /// <param name="session">세션</param>
    void OnClientConnected(ClientSession session)
    {
        _serverPeer = new Peer();
        _serverPeer.SetClientSession(session);

        //
        if (_connectCallBack != null)
            _connectCallBack();
    }


    /// <summary>
    /// 재연결 성공 콜백
    /// </summary>
    /// <param name="session"></param>
    /// <param name="peer"></param>
    void OnClientReconnected(ClientSession session, Peer peer)
    {

    }


    void OnClientDisconnected(ClientSession session)
    {
        Disconnect();
        
        UnityUtil.Print(" DISCONNECT !!!!  ", " CLINET DISCONNECT !!! ", "blue");
        
        // 게임중이었다면....을 체크해야된다
        if (InGameManager.Get() != null && InGameManager.Get().isGamePlaying == true)
        {
            // 게임 시작으로 보낸다
            NetworkManager.Get().GameDisconnectSignal();
        }
    }



    public void Update()
    {
        _netService.Update();
    }


    public void Send(short protocolId, byte[] msg)
    {
        _serverPeer.SendPacket(protocolId, msg);
    }


    
}
