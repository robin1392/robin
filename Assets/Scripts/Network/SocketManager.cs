using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using RWCoreNetwork;
using RWCoreNetwork.NetService;
using RWCoreNetwork.NetPacket;
using System;


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
        PacketHandler handler = new PacketHandler(recvProcessor, 30, 4096);
        _netService = new NetClientService(handler, new NetLogger(), 1, 4096, 5000, 1000, false);
        _netService.ClientConnectedCallback += OnClientConnected;
        _netService.ClientDisconnectedCallback += OnClientDisconnected;
    }


    public void Connect(string host, int port , Action connectCallback = null)
    {
        _netService.Connect(host, port, 1);
        _connectCallBack = connectCallback;
    }

    public void Disconnect()
    {
        _netService.Disconnect();
        _serverPeer = null;
    }
    
    public bool IsConnected()
    {
        return _serverPeer != null;
    }
    
    
    
    /// <summary>
    /// 서버 연결 성공 콜백
    /// </summary>
    /// <param name="session">세션</param>
    void OnClientConnected(UserToken session)
    {
        _serverPeer = new Peer();
        _serverPeer.SetUserToken(session);

        //
        if (_connectCallBack != null)
            _connectCallBack();
    }


    void OnClientDisconnected(UserToken session)
    {
        Disconnect();
        
        UnityUtil.Print(" DISCONNECT !!!!  ", " CLINET DISCONNECT !!! ", "blue");
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
