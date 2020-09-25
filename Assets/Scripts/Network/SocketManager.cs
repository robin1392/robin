﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using RWCoreNetwork;
using RWCoreNetwork.NetService;
using RWCoreNetwork.NetPacket;
using System;


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
        PacketHandler handler = new PacketHandler(recvProcessor, 30);

        _netService = new NetClientService(handler, 1, 4096, 1000, 1000);
        _netService.ClientConnectedCallback += OnClientConnected;
        _netService.ClientConnectedCallback += OnClientDisconnected;
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
