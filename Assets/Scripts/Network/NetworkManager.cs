using System;
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
    
    #region socket addr

    private string _serverAddr;
    public string serverAddr
    {
        get => _serverAddr;
        private set => _serverAddr = value;
    }

    private int _port;

    public int port
    {
        get => _port;
        private set => _port = value;
    }

    private string _gameSession;

    public string gameSession
    {
        get => _gameSession;
        private set => _gameSession = value;
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
    
    #region update packet
    
    public void UpdateSocket()
    {
        if(_clientSocket != null)
            _clientSocket.Update();
    }

    #endregion
    
    
    #region connent

    public void SetAddr(string serveraddr, int port, string gamesession)
    {
        _serverAddr = serveraddr;
        _port = port;
        _gameSession = gamesession;
    }

    public void ConnectServer( Action callback = null)
    {
        _clientSocket.Connect( _serverAddr , _port , callback);
    }

    
    public void DisconnectSocket()
    {
        if(_clientSocket.IsConnected() == true)
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
        _packetRecv.LeaveGameAck = _socketRecv.OnLeaveGameAck;
        _packetRecv.ReadyGameAck = _socketRecv.OnReadyGameAck;
        _packetRecv.SetDeckAck = _socketRecv.OnSetDeckAck;
        _packetRecv.GetDiceAck = _socketRecv.OnGetDiceAck;
        _packetRecv.LevelUpDiceAck = _socketRecv.OnLevelUpDiceAck;
        _packetRecv.HitDamageAck = _socketRecv.OnHitDamageAck;
        
        // notify
        _packetRecv.JoinGameNotify = _socketRecv.OnJoinGameNotify;
        _packetRecv.LeaveGameNotify = _socketRecv.OnLeaveGameNotify;
        _packetRecv.GetDiceNotify = _socketRecv.OnGetDiceNotify;
        _packetRecv.DeactiveWaitingObjectNotify = _socketRecv.OnDeactiveWaitingObjectNotify;
            
        // relay
        _packetRecv.RemoveMinionRelay = _socketRecv.OnRemoveMinionRelay;
        _packetRecv.HitDamageMinionRelay = _socketRecv.OnHitDamageMinionRelay;
        _packetRecv.DestroyMinionRelay = _socketRecv.OnDestroyMinionRelay;
        _packetRecv.HealMinionRelay = _socketRecv.OnHealMinionRelay;
        _packetRecv.PushMinionRelay = _socketRecv.OnPushMinionRelay;
        _packetRecv.SetMinionAnimationTriggerRelay = _socketRecv.OnSetMinionAnimationTriggerRelay;
        _packetRecv.RemoveMagicRelay = _socketRecv.OnRemoveMagicRelay;
        _packetRecv.FireArrowRelay = _socketRecv.OnFireArrowRelay;
        _packetRecv.FireballBombRelay = _socketRecv.OnFireballBombRelay;
        _packetRecv.MineBombRelay = _socketRecv.OnMineBombRelay;
        _packetRecv.SetMagicTargetIdRelay = _socketRecv.OnSetMagicTargetIdRelay;
        _packetRecv.SetMagicTargetRelay = _socketRecv.OnSetMagicTargetRelay;
        
        _clientSocket.Init(_packetRecv);
    }
    
    #endregion
    
}



