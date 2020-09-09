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
    
    /*public GamePacketSender SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }*/

    // 외부에서 얘를 건들일은 없도록하지
    private GamePacketReceiver _packetRecv;

    
    //
    // 패킷 리시브 함수들 모아놓는곳
    private SocketRecvEvent _socketRecv;
    // 패킷 send 함수 모아놓는곳
    private SocketSendEvent _socketSend;

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
    
    
    #region game process var

    private bool _recvJoinPlayerInfoCheck = false;
    
    public PLAY_TYPE playType;

    private bool _isMaster;
    public bool IsMaster
    {
        get => _isMaster;
        set => _isMaster = value;
    }

    public int UserUID
    {
        get => GetNetInfo().UserUID();
    }

    public int OtherUID
    {
        get => GetNetInfo().OtherUID();
    }
    #endregion
    
    
    
    #region socket user info
    // 통신간 정보를 담아둘만한 휘발성 클래스
    
    private NetInfo _netInfo = null;
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

    private void InitNetwork()
    {
        //
        _netInfo = new NetInfo();
        _recvJoinPlayerInfoCheck = false;
        
        webNetCommon = this.gameObject.AddComponent<WebNetworkCommon>();
        webPacket = this.gameObject.AddComponent<WebPacket>();

        _clientSocket = new SocketManager();
        _packetSend = new GamePacketSender();

        // 
        _socketRecv = new SocketRecvEvent();
        _socketSend = new SocketSendEvent(_packetSend);
        
        // recv 셋팅
        CombineRecvDelegate();
    }

    
    private void DestroyNetwork()
    {
        if (IsConnect())
        {
            DisconnectSocket();
        }
        
        GameObject.Destroy(webPacket);
        GameObject.Destroy(webNetCommon);

        _packetSend = null;
        _packetRecv = null;

        _socketRecv = null;
        _socketSend = null;
        
        _clientSocket = null;

        _netInfo = null;
    }
    #endregion
    
    #region update packet
    
    private void UpdateSocket()
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
        
        _recvJoinPlayerInfoCheck = true;
        _netInfo.Clear();
    }

    public void ConnectServer( PLAY_TYPE type , Action callback = null)
    {
        playType = type;
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

    public void Send(GameProtocol protocol , params object[] param)
    {
        UnityUtil.Print("SEND =>  " , protocol.ToString() , "magenta");
        _socketSend.SendPacket(protocol , _clientSocket.Peer , param);
    }
    #endregion

    
    #region get net info

    public NetInfo GetNetInfo()
    {
        return _netInfo;
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
        _packetRecv.GetDiceAck = _socketRecv.OnGetDiceAck;
        _packetRecv.LevelUpDiceAck = _socketRecv.OnLevelUpDiceAck;
        
        _packetRecv.UpgradeSpAck = _socketRecv.OnUpgradeSpAck;
        _packetRecv.InGameUpDiceAck = _socketRecv.OnInGameUpDiceAck;
        
        _packetRecv.HitDamageAck = _socketRecv.OnHitDamageAck;
        
        // notify
        _packetRecv.JoinGameNotify = _socketRecv.OnJoinGameNotify;
        _packetRecv.LeaveGameNotify = _socketRecv.OnLeaveGameNotify;
        _packetRecv.GetDiceNotify = _socketRecv.OnGetDiceNotify;
        _packetRecv.DeactiveWaitingObjectNotify = _socketRecv.OnDeactiveWaitingObjectNotify;
        _packetRecv.SpawnNotify = _socketRecv.OnSpawnNotify;
        _packetRecv.AddSpNotify = _socketRecv.OnAddSpNotify;

        _packetRecv.LevelUpDiceNotify = _socketRecv.OnLevelUpDiceNotify;
        _packetRecv.UpgradeSpNotify = _socketRecv.OnUpgradeSpNotify;
        _packetRecv.InGameUpDiceNotify = _socketRecv.OnInGameUpDiceNotify;
        
        _packetRecv.HitDamageNotify = _socketRecv.HitDamageNotify;
        _packetRecv.EndGameNotify = _socketRecv.OnEndGameNotify;
        
            
        // relay
        _packetRecv.RemoveMinionRelay = _socketRecv.OnRemoveMinionRelay;
        _packetRecv.HitDamageMinionRelay = _socketRecv.OnHitDamageMinionRelay;
        
        _packetRecv.HealMinionRelay = _socketRecv.OnHealMinionRelay;
        _packetRecv.PushMinionRelay = _socketRecv.OnPushMinionRelay;
        _packetRecv.SetMinionAnimationTriggerRelay = _socketRecv.OnSetMinionAnimationTriggerRelay;
        _packetRecv.RemoveMagicRelay = _socketRecv.OnRemoveMagicRelay;
        _packetRecv.FireArrowRelay = _socketRecv.OnFireArrowRelay;
        _packetRecv.FireballBombRelay = _socketRecv.OnFireballBombRelay;
        _packetRecv.MineBombRelay = _socketRecv.OnMineBombRelay;
        _packetRecv.SetMagicTargetIdRelay = _socketRecv.OnSetMagicTargetIdRelay;
        _packetRecv.SetMagicTargetRelay = _socketRecv.OnSetMagicTargetRelay;
        
        _packetRecv.DestroyMinionRelay = _socketRecv.OnDestroyMinionRelay;
        
        _clientSocket.Init(_packetRecv);
    }
    
    #endregion
    
}





#region net user info
/// <summary>
/// 소켓 통신에서 잠시 담아둘 정보 클래스
/// 대전 할때마다 정보 갱신..
/// </summary>
public class NetInfo
{
    
    //
    public MsgPlayerInfo playerInfo;
    public MsgPlayerInfo otherInfo;
    public bool myInfoGet = false;
    public bool otherInfoGet = false;
    
    public NetInfo()
    {
        
    }

    public void Clear()
    {
        myInfoGet = false;
        otherInfoGet = false;
    }

    public void SetPlayerInfo(MsgPlayerInfo info)
    {
        playerInfo = info;
        myInfoGet = true;
    }

    public void SetOtherInfo(MsgPlayerInfo info)
    {
        otherInfo = info;
        otherInfoGet = true;
    }

    public bool IsMyUID(int userUid)
    {
        if (playerInfo.PlayerUId == userUid)
            return true;

        return false;
    }

    public int UserUID()
    {
        return playerInfo.PlayerUId;
    }

    public int OtherUID()
    {
        return otherInfo.PlayerUId;
    }

    public bool IsOtherUID(int userUid)
    {
        if (otherInfo.PlayerUId == userUid)
            return true;

        return false;
    }
    
}
#endregion

