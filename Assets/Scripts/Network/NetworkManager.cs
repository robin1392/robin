using RWGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RWCoreNetwork;
using RWGameProtocol.Msg;

public class NetworkManager : Singleton<NetworkManager>
{
    
    
    #region net variable
    
    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    
    
    // socket
    private SocketManager _clientSocket = null;
    
    // sender 
    private RWGameProtocol.GamePacketSender _packetSend;
    public RWGameProtocol.GamePacketSender SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }
    
    #endregion
    
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitNetwork();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSocket();
    }

    public override void OnDestroy()
    {
        DestroyNetwork();
        
        base.OnDestroy();
    }

    #endregion
    
    
    #region net add

    public void InitNetwork()
    {
        webNetCommon = this.gameObject.AddComponent<WebNetworkCommon>();
        webPacket = this.gameObject.AddComponent<WebPacket>();

        _clientSocket = new SocketManager();
        _packetSend = new GamePacketSender();


        // TODO : 게임 서버 패킷 응답 처리 delegate를 설정해야합니다.
        GamePacketReceiver gamePacketReceiver = new GamePacketReceiver();
        gamePacketReceiver.JoinGameAck = OnJoinGameAck;



        _clientSocket.Init(gamePacketReceiver);
    }

    
    public void DestroyNetwork()
    {
        GameObject.Destroy(webPacket);
        GameObject.Destroy(webNetCommon);

        _packetSend = null;
        _clientSocket = null;
    }
    #endregion
    
    
    #region socket

    public void UpdateSocket()
    {
        if(_clientSocket != null)
            _clientSocket.Update();
    }

    public void DisconnectSocket()
    {
        _clientSocket.Disconnect();
    }

    public bool IsConnect()
    {
        return _clientSocket.IsConnected();
    }
    #endregion


    /// <summary>
    /// 게임 참가 응답 처리부
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="msg"></param>
    void OnJoinGameAck(IPeer peer, MsgJoinGameAck msg)
    {
        // something to do...

        //NetworkManager.Get().SendSocket.ReadyGameReq(peer);
        //SendSocket.ReadyGameReq(peer);
    }
}

