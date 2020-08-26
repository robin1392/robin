using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using RWCoreNetwork;
using RWCoreNetwork.NetPacket;
using System;


public class SocketManager
{
    private NetworkService _netService;
    private ServerPeer _serverPeer;


    //
    private Action _callBack;
    
    
    
    public SocketManager()
    {
        _netService = null;
        _serverPeer = null;
    }

    public void Init(IPacketProcessor recvProcessor)
    {
        PacketHandler handler = new PacketHandler();
        handler.Init(recvProcessor, 10);

        _netService = new NetworkService(handler);
    }


    public void Connect(string host, int port)
    {
        Connector connector = new Connector(_netService);
        connector.OnConnectedCallback += OnServerConnected;

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        connector.Connect(endpoint);
    }

    public void Disconnect()
    {
        if (_serverPeer == null)
        {
            return;
        }

        _serverPeer.Disconnect();
        
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
    void OnServerConnected(UserToken session)
    {
        _serverPeer = new ServerPeer();
        _serverPeer.SetUserToken(session);

        //
    }
    
    
    
    public void Update()
    {
        if (IsConnected() == false)
        {
            return;
        }

        _netService.PacketHandler.ProcessPacket();
    }


    public void Send(short protocolId, byte[] msg)
    {
        _serverPeer.SendPacket(protocolId, msg);
    }


    public void SetCallBack(Action callback = null)
    {
        _callBack = callback;
    }
    
}
