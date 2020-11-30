﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Runtime.InteropServices;
using ED;
using RandomWarsService.Network.Socket.NetSession;
using RandomWarsService.Network.Socket.NetService;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsProtocol;

class NetLogger : RandomWarsService.Core.ILog
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
    private Action _reconnectCallBack;
    
    public SocketManager()
    {
        _netService = null;
        _serverPeer = null;
    }

    public void Init(IPacketReceiver recvProcessor)
    {
        NetLogger netLogger = new NetLogger();
        PacketHandler handler = new PacketHandler(recvProcessor, netLogger, 100, 10240);
        _netService = new NetClientService(handler, netLogger, 1, 10240, 5000, 1000, false, Application.persistentDataPath + "/NetState.bytes");
        _netService.ClientConnectedCallback += OnClientConnected;
        _netService.ClientDisconnectedCallback += OnClientDisconnected;
        _netService.ClientReconnectedCallback += OnClientReconnected;
        _netService.ClientReconnectingCallback += NetworkManager.Get().OnClientReconnecting;
    }


    public void Connect(string host, int port , string playerSessionId, Action connectCallback = null)
    {
        //_netService.ClientSession.NetState = ENetState.Connecting;
        //_netService.ClientSession.SessionId = clientSessionId;
        _netService.Connect(host, port, playerSessionId);
        _connectCallBack = connectCallback;
    }
    
    public void ReConnect(string host, int port , string playerSessionId, Action reconnectCallback = null)
    {
        //_netService.ClientSession.NetState = ENetState.Reconnecting;
        //_netService.ClientSession.SessionId = clientSessionId;
        _netService.Connect(host, port, playerSessionId);
        _reconnectCallBack = reconnectCallback;
    }

    public void Disconnect()
    {
        if (_serverPeer != null)
        {
            _netService.Disconnect(ESessionState.Leave);
            _serverPeer = null;    
        }
    }

    public void Pause()
    {
        _netService.PauseSession();
    }


    public void Resume()
    {
        _netService.ResumeSession();
    }


    public bool IsConnected()
    {
        return _serverPeer != null;
    }
    

    public void PrintNetworkStatus()
    {
        //UnityUtil.Print("NETWORK STATUS  ", 
        //    "Recv queue count: " + _netService.ReceiveQueueCount().ToString() 
        //    + ", Send queue count: " + _netService.ClientSession.SendQueueCount().ToString(), "magenta");
    }
    
    
    /// <summary>
    /// 서버 연결 성공 콜백
    /// </summary>
    /// <param name="session">세션</param>
    void OnClientConnected(ClientSession session, ESessionState sessionState)
    {
        UnityUtil.Print(" CONNECT   ", " CLINET CONNECT !!! state : " + sessionState, "blue");

        if (sessionState != ESessionState.None && sessionState != ESessionState.Wait)
        {
            //NetworkManager.Get().DeleteBattleInfo();
            NetworkManager.Get().SetReconnect(false);
            //
            GameStateManager.Get().MoveMainScene();
            return;
        }

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
    /// <param name="sessionState"></param>
    void OnClientReconnected(ClientSession session, ESessionState sessionState)
    {
        UnityUtil.Print(" RECONNECT   ", " CLINET RECONNECT !!! state : " + sessionState, "blue");

        //
        if (sessionState != ESessionState.None && sessionState != ESessionState.Wait)
        {
            //NetworkManager.Get().DeleteBattleInfo();
            NetworkManager.Get().SetReconnect(false);
            //
            GameStateManager.Get().MoveMainScene();
            return;
        }
        
        Peer peer = session.GetPeer();
        if (peer == null)
            _serverPeer = new Peer();    
        else
            _serverPeer = peer;
        
        _serverPeer.SetClientSession(session);


        NetworkManager.Get().Send(GameProtocol.RECONNECT_GAME_REQ);
        //
        //if (_reconnectCallBack != null)
        //    _reconnectCallBack();
    }


    void OnClientDisconnected(ClientSession session, ESessionState sessionState)
    {
        UnityUtil.Print(" DISCONNECT !!!!  ", " CLINET DISCONNECT !!! state : " + sessionState, "blue");

        if (sessionState != ESessionState.None && sessionState != ESessionState.Wait)
        {
            //NetworkManager.Get().DeleteBattleInfo();
            NetworkManager.Get().SetReconnect(false);
            //
            GameStateManager.Get().MoveMainScene();
            return;
        }
        
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


    public bool CheckReconnection()
    {
        return _netService.CheckReconnection();
    }
    
}
