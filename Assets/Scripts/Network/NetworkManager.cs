using RWGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RWCoreNetwork;
using RWGameProtocol.Msg;
using RWGameProtocol;

public class NetworkManager : Singleton<NetworkManager>
{
    
    
    #region net variable
    
    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    
    
    // socket
    private SocketManager _clientSocket = null;
    
    // sender 
    private GamePacketSender _packetSend;
    public GamePacketSender SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }

    // 외부에서 얘를 건들일은 없도록하지
    private GamePacketReceiver _packetRecv;

    private SocketRecvEvent _socketRecv;
    public SocketRecvEvent socketRecv
    {
        get => _socketRecv;
        private set => _socketRecv = value;
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

        // 
        _socketRecv = new SocketRecvEvent();
        // recv 셋팅
        CombineRecvDelegate();
    }

    
    public void DestroyNetwork()
    {
        GameObject.Destroy(webPacket);
        GameObject.Destroy(webNetCommon);

        _packetSend = null;
        _packetRecv = null;
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



    #region socket delegate

    public void CombineRecvDelegate()
    {
        // TODO : 게임 서버 패킷 응답 처리 delegate를 설정해야합니다.
        _packetRecv = new GamePacketReceiver();
        _packetRecv.JoinGameAck = _socketRecv.OnJoinGameAck;

        
        

        _clientSocket.Init(_packetRecv);
    }
    
    #endregion
    
}

