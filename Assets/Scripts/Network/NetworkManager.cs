﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsService.Network.Http;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEditor;

public class NetworkManager : Singleton<NetworkManager>
{
    #region net variable

    public Global.E_MATCHSTEP NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;

    SocketService _socketService;

    private HttpSender _httpSender;
    private HttpReceiver _httpReceiver;
    private HttpClient _httpClient;

    private int _matchTryCount;


    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    // socket
    private SocketManager _clientSocket = null;
    // sender 
    private SocketSender _packetSend;

    /*public GamePacketSender SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }*/

    // 외부에서 얘를 건들일은 없도록하지
    private SocketReceiver _packetRecv;

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

    private string _battlePath = "/BattleInfo.bytes";

    #endregion


    #region game process var

    private bool _recvJoinPlayerInfoCheck = false;

    public Global.PLAY_TYPE playType;

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


    #region net game pause , resume , reconnect

    // Pause
    private bool _isOtherPause;
    public bool IsOtherPause => _isOtherPause;
    public UnityEvent<bool> event_OtherPause = new UnityEvent<bool>();


    // Resume
    private bool _isResume;
    public bool isResume => _isResume;


    private bool _isReconnect;
    public bool isReconnect => _isReconnect;

    #endregion

    #region dev test var

    [Header("Dev Test Variable")]
    public bool UseLocalServer;
    public string LocalServerAddr;
    public int LocalServerPort;
    public string UserId;
    #endregion


    #region socket user info
    // 통신간 정보를 담아둘만한 휘발성 클래스

    private NetInfo _netInfo = null;

    private NetBattleInfo _battleInfo = null;

    private Action<MsgOpenBoxAck> _boxOpenCallback;
    private Action<MsgLevelUpDiceAck> _diceLevelUpCallback;
    private Action<MsgEditUserNameAck> _editUserNameCallback;
    private Action<MsgGetRankAck> _getRankCallback;
    #endregion

    #region unity base

    public override void Awake()
    {
        if (NetworkManager.Get() != null && this != NetworkManager.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

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

        if (_socketService != null)
        {
            _socketService.Update();
        }

        if (_httpClient != null)
        {
            _httpClient.Update();
        }
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
        _socketService = new SocketService();

        _httpReceiver = new HttpReceiver();
        _httpClient = new HttpClient("https://vj7nnp92xd.execute-api.ap-northeast-2.amazonaws.com/prod", _httpReceiver);
        _httpSender = new HttpSender(_httpClient);

        _httpReceiver.AuthUserAck = OnAuthUserAck;
        _httpReceiver.UpdateDeckAck = OnUpdateDeckAck;
        _httpReceiver.StartMatchAck = OnStartMatchAck;
        _httpReceiver.StatusMatchAck = OnStatusMatchAck;
        _httpReceiver.StopMatchAck = OnStopMatchAck;
        _httpReceiver.OpenBoxAck = OnOpenBoxAck;
        _httpReceiver.LevelUpDiceAck = OnLevelUpDiceAck;
        _httpReceiver.EditUserNameAck = OnEditUserNameAck;
        _httpReceiver.GetRankAck = OnGetRankAck;





        //
        _netInfo = new NetInfo();
        _recvJoinPlayerInfoCheck = false;

        webNetCommon = this.gameObject.AddComponent<WebNetworkCommon>();
        webPacket = this.gameObject.AddComponent<WebPacket>();

        _clientSocket = new SocketManager();
        _packetSend = new SocketSender();

        // 
        _socketRecv = new SocketRecvEvent();
        _socketSend = new SocketSendEvent(_packetSend);

        SetReconnect(false);
        
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
        
        _battleInfo = null;

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
        if (_clientSocket != null)
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

    public void ConnectServer(Global.PLAY_TYPE type, Action callback = null)
    {
        // 배틀정보..저장
        SaveBattleInfo();

        // 시작하면서 상대 디스커넥트
        SetOtherDisconnect(false);    // disconnect
        SetResume(false);        // resume
        
        SetReconnect(false);        // reconnect

        playType = type;
        _clientSocket.Connect(_serverAddr, _port, _gameSession, callback);
    }

    public void ReConnectServer(Global.PLAY_TYPE type ,  string serverAddr , int port , string session , Action callback = null)
    {
        _serverAddr = serverAddr;
        _port = port;
        _gameSession = session;
        
        // 시작하면서 상대 멈춤 초기화
        SetOtherDisconnect(false);    // disconnect
        SetResume(false);        // resume

        print("ReConnecting....");

        playType = type;
        _clientSocket.ReConnect(_serverAddr, _port, _gameSession, callback);
    }


    public void DisconnectSocket()
    {
        if (_clientSocket != null && _clientSocket.IsConnected() == true)
            _clientSocket.Disconnect();
    }

    public bool IsConnect()
    {
        if (_clientSocket == null)
            return false;

        return _clientSocket.IsConnected();
    }

    public void Send(GameProtocol protocol, params object[] param)
    {
        if (protocol != GameProtocol.MINION_STATUS_RELAY)
            UnityUtil.Print("SEND =>  ", protocol.ToString(), "magenta");

        _socketSend.SendPacket(protocol, _clientSocket.Peer, param);
    }


    public void GameDisconnectSignal()
    {
        // disconnect 감지 했다는...
        //StopAllCoroutines();

        //
        GameStateManager.Get().ChangeScene(Global.E_GAMESTATE.STATE_START);
    }

    public void PrintNetworkStatus()
    {
        _clientSocket.PrintNetworkStatus();
    }


    public void PauseGame()
    {
        _clientSocket.Pause();
    }


    public void ResumeGame()
    {
        _clientSocket.Resume();
    }


    #endregion


    #region get net info

    public NetInfo GetNetInfo()
    {
        return _netInfo;
    }

    #endregion

    #region pause resume reconnect

    public void SetOtherDisconnect(bool disconnect)
    {
        event_OtherPause.Invoke(disconnect);
        _isOtherPause = disconnect;
    }

    public void SetResume(bool resume)
    {
        _isResume = resume;
    }

    public void SetReconnect(bool reconnect)
    {
        _isReconnect = reconnect;
    }
    #endregion


    #region socket delegate

    public void CombineRecvDelegate()
    {
        // TODO : 게임 서버 패킷 응답 처리 delegate를 설정해야합니다.
        _packetRecv = new SocketReceiver();

        _packetRecv.JoinGameAck = _socketRecv.OnJoinGameAck;
        _packetRecv.LeaveGameAck = _socketRecv.OnLeaveGameAck;
        _packetRecv.ReadyGameAck = _socketRecv.OnReadyGameAck;
        _packetRecv.GetDiceAck = _socketRecv.OnGetDiceAck;
        _packetRecv.MergeDiceAck = _socketRecv.OnMergeDiceAck;

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

        _packetRecv.MergeDiceNotify = _socketRecv.OnMergeDiceNotify;
        _packetRecv.UpgradeSpNotify = _socketRecv.OnUpgradeSpNotify;
        _packetRecv.InGameUpDiceNotify = _socketRecv.OnInGameUpDiceNotify;

        _packetRecv.HitDamageNotify = _socketRecv.HitDamageNotify;
        _packetRecv.EndGameNotify = _socketRecv.OnEndGameNotify;


        // relay
        _packetRecv.HitDamageMinionRelay = _socketRecv.OnHitDamageMinionRelay;
        _packetRecv.DestroyMinionRelay = _socketRecv.OnDestroyMinionRelay;
        _packetRecv.HealMinionRelay = _socketRecv.OnHealMinionRelay;
        _packetRecv.PushMinionRelay = _socketRecv.OnPushMinionRelay;
        _packetRecv.SetMinionAnimationTriggerRelay = _socketRecv.OnSetMinionAnimationTriggerRelay;
        _packetRecv.FireArrowRelay = _socketRecv.OnFireArrowRelay;
        _packetRecv.FireballBombRelay = _socketRecv.OnFireballBombRelay;
        _packetRecv.MineBombRelay = _socketRecv.OnMineBombRelay;
        _packetRecv.SetMagicTargetIdRelay = _socketRecv.OnSetMagicTargetIdRelay;
        _packetRecv.SetMagicTargetRelay = _socketRecv.OnSetMagicTargetRelay;

        //
        _packetRecv.SturnMinionRelay = _socketRecv.OnSturnMinionRelay;
        _packetRecv.RocketBombRelay = _socketRecv.OnRocketBombRelay;
        _packetRecv.IceBombRelay = _socketRecv.OnIceBombRelay;
        _packetRecv.DestroyMagicRelay = _socketRecv.OnDestroyMagicRelay;
        _packetRecv.FireCannonBallRelay = _socketRecv.OnFireCannonBallRelay;
        _packetRecv.FireSpearRelay = _socketRecv.OnFireSpearRelay;
        _packetRecv.FireManFireRelay = _socketRecv.OnFireManFireRelay;
        _packetRecv.ActivatePoolObjectRelay = _socketRecv.OnActivatePoolObjectRelay;
        _packetRecv.MinionCloackingRelay = _socketRecv.OnMinionCloackingRelay;
        _packetRecv.MinionFlagOfWarRelay = _socketRecv.OnMinionFogOfWarRelay;
        _packetRecv.SendMessageVoidRelay = _socketRecv.OnSendMessageVoidRelay;
        _packetRecv.SendMessageParam1Relay = _socketRecv.OnSendMessageParam1Relay;
        _packetRecv.NecromancerBulletRelay = _socketRecv.OnNecromancerBulletRelay;
        _packetRecv.SetMinionTargetRelay = _socketRecv.OnSetMinionTargetRelay;

        _packetRecv.ScarecrowRelay = _socketRecv.OnScarecrowRelay;
        _packetRecv.LayzerTargetRelay = _socketRecv.OnLazerTargetRelay;

        _packetRecv.MinionStatusRelay = _socketRecv.OnMinionStatusRelay;

        _packetRecv.FireBulletRelay = _socketRecv.OnFireBulletRelay;
        _packetRecv.MinionInvincibilityRelay = _socketRecv.OnMinionInvincibilityRelay;

        // reconnect , pause , resume
        _packetRecv.DisconnectGameNotify = _socketRecv.OnDisconnectGameNotify;
        _packetRecv.ReconnectGameNotify = _socketRecv.OnReconnectGameNotify;
        _packetRecv.ReconnectGameAck = _socketRecv.OnReconnectGameAck;
        
        _packetRecv.StartSyncGameAck = _socketRecv.OnStartSyncGameAck;
        _packetRecv.StartSyncGameNotify = _socketRecv.OnStartSyncGameNotify;
        _packetRecv.EndSyncGameAck = _socketRecv.OnEndSyncGameAck;
        _packetRecv.EndSyncGameNotify = _socketRecv.OnEndSyncGameNotify;

        _packetRecv.ReadySyncGameAck = _socketRecv.OnReadySyncGameAck;
        _packetRecv.ReadySyncGameNotify = _socketRecv.OnReadySyncGameNotify;
        
        
        // not use...
        _packetRecv.PauseGameNotify = _socketRecv.OnPauseGameNotify;
        _packetRecv.ResumeGameNotify = _socketRecv.OnResumeGameNotify;

        _clientSocket.Init((IPacketReceiver)_packetRecv);
    }

    #endregion


    #region read write file
    public void BinarySerialize(NetBattleInfo data, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public NetBattleInfo BinaryDeserialize(string filePath)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);
            NetBattleInfo patchinfo = (NetBattleInfo)formatter.Deserialize(stream);
            stream.Close();

            return patchinfo;
        }
        catch
        {
            // game frame -- init
            return null;
        }

    }

    // 배틀 인포 파일이 잇는가??
    public NetBattleInfo ReadBattleInfo()
    {
        string battlePath = Application.persistentDataPath + _battlePath;
        if (File.Exists(battlePath) == false)
        {
            return null;
        }
        else
        {
            _battleInfo = BinaryDeserialize(battlePath);
            return _battleInfo;    
        }
    }
    

    public void SaveBattleInfo()
    {
        string battlePath = Application.persistentDataPath + _battlePath;

        if (_battleInfo == null)
            _battleInfo = new NetBattleInfo();

        _battleInfo.serverAddr = _serverAddr;
        _battleInfo.serverPort = _port;
        _battleInfo.serverSession = _gameSession;
        _battleInfo.battleStartTime = DateTime.UtcNow;

        _battleInfo.battleStart = true;
        
        print(_battleInfo.battleStartTime);

        BinarySerialize(_battleInfo, battlePath);
    }

    public void DeleteBattleInfo()
    {
        string battlePath = Application.persistentDataPath + _battlePath;
        _battleInfo = BinaryDeserialize(battlePath);

        if (_battleInfo == null)
            return;

        print(_battleInfo.battleStartTime);

        _battleInfo.ResetInfo();

        // 파일 삭제
        File.Delete(battlePath);

        _battleInfo = null;
    }

    #endregion
    
    
    #region reconnect to do

    public void ReconnectPacket(MsgReconnectGameAck msg)
    {
        // 에러코드가 0 이 아닐경우는 게임에 이상이 있다는 서버 메세지니...그냥 리턴 시켜서..메인으로
        if (msg.ErrorCode != 0)
        {
            DeleteBattleInfo();
            DisconnectSocket();
            
            SetReconnect(false);
            
            //
            GameStateManager.Get().MoveMainScene();
            return;
        }
        
        print("my info : " + msg.PlayerBase.PlayerUId +  "  " + msg.PlayerBase.IsBottomPlayer + "    " + msg.PlayerBase.Name );
        print("other info : " + msg.OtherPlayerBase.PlayerUId +  "  " + msg.OtherPlayerBase.IsBottomPlayer + "    " + msg.OtherPlayerBase.Name );
        _netInfo.SetPlayerBase(msg.PlayerBase);
        _netInfo.SetOtherBase(msg.OtherPlayerBase);
        
        //
        IsMaster = _netInfo.playerInfo.IsBottomPlayer;
        
        //
        GameStateManager.Get().MoveInGameBattle();
    }
    #endregion


    #region http

    public void AuthUserReq(string userId)
    {
        MsgUserAuthReq msg = new MsgUserAuthReq();
        msg.UserId = userId;
        _httpSender.AuthUserReq(msg);
        UnityUtil.Print("SEND AUTH => userid", userId, "green");
    }


    void OnAuthUserAck(MsgUserAuthAck msg)
    {
        if (msg.ErrorCode == GameErrorCode.ERROR_USER_NOT_FOUND)
        {
            ObscuredPrefs.SetString("UserKey", string.Empty);
            ObscuredPrefs.Save();
            UI_Start.Get().SetTextStatus(string.Empty);
            UI_Start.Get().btn_GuestAccount.gameObject.SetActive(true);
            UI_Start.Get().btn_GuestAccount.onClick.AddListener(() =>
            {
                UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);
                AuthUserReq(string.Empty);
            });
            return;
        }
        
        UserInfoManager.Get().SetUserInfo(msg.UserInfo);
        UserInfoManager.Get().SetDeck(msg.UserDeck);
        UserInfoManager.Get().SetDice(msg.UserDice);
        UserInfoManager.Get().SetBox(msg.UserBox);

        GameStateManager.Get().UserAuthOK();
        UnityUtil.Print("RECV AUTH => userid", msg.UserInfo.UserId, "green");
    }


    public void EditUserNameReq(string userId, string userName, Action<MsgEditUserNameAck> callback)
    {
        MsgEditUserNameReq msg = new MsgEditUserNameReq();
        msg.UserId = userId;
        msg.UserName = userName;
        _editUserNameCallback = callback;
        _httpSender.EditUserNameReq(msg);
        UnityUtil.Print("SEND EDIT USER NAME => name", userName, "green");
    }


    void OnEditUserNameAck(MsgEditUserNameAck msg)
    {
        if (_editUserNameCallback != null)
        {
            _editUserNameCallback(msg);
        }
        
        UnityUtil.Print("RECV EDIT USER NAME => name", msg.UserName, "green");
    }


    IEnumerator WaitForMatch()
    {
        yield return new WaitForSeconds(1.0f);
        StatusMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
    }

    
    public void StartMatchReq(string userId)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_START 
            || NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중이면 요청할 수 없다.
            return;
        }

        NetMatchStep = Global.E_MATCHSTEP.MATCH_START;

        MsgStartMatchReq msg = new MsgStartMatchReq();
        msg.UserId = userId;
        _httpSender.StartMatchReq(msg);
        UnityUtil.Print("SEND MATCH START => userid", userId, "green");
    }


    void OnStartMatchAck(MsgStartMatchAck msg)
    {
        if (string.IsNullOrEmpty(msg.TicketId))
        {
            UnityUtil.Print("ticket id null");
            return;
        }

        _matchTryCount = 0;
        UserInfoManager.Get().SetTicketId(msg.TicketId);
        UnityUtil.Print("RECV MATCH START => ticketId", msg.TicketId, "green");

        StartCoroutine(WaitForMatch());
    }


    public void StatusMatchReq(string ticketId)
    {
        if (NetMatchStep != Global.E_MATCHSTEP.MATCH_START)
        {
            // 매칭 중에서 상태 요청을 할 수 있다ㅏ.
            return;
        }


        _matchTryCount++;
        MsgStatusMatchReq msg = new MsgStatusMatchReq();
        msg.UserId = UserInfoManager.Get().GetUserInfo().userID;
        msg.TicketId = ticketId;

        _httpSender.StatusMatchReq(msg);
        UnityUtil.Print("SEND MATCH STATUS => ticketid", ticketId, "green");
    }


    void OnStatusMatchAck(MsgStatusMatchAck msg)
    {
        if (string.IsNullOrEmpty(msg.PlayerSessionId))
        {
            if (_matchTryCount > 10)
            {
                StopMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
            }
            else
            {
                StartCoroutine(WaitForMatch());
            }
        }
        else
        {
            NetMatchStep = Global.E_MATCHSTEP.MATCH_CONNECT;

            // go match -> socket
            SetAddr(msg.ServerAddr, msg.Port, msg.PlayerSessionId);

            // 우선 그냥 배틀로 지정하자
            ConnectServer(Global.PLAY_TYPE.BATTLE, GameStateManager.Get().ServerConnectCallBack);
        }
        UnityUtil.Print("RECV MATCH STATUS => ", string.Format("server:{0}, player-session-id:{1}", msg.ServerAddr + ":" + msg.Port, msg.PlayerSessionId), "green");
    }


    public void StopMatchReq(string ticketId)
    {
        NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;

        MsgStopMatchReq msg = new MsgStopMatchReq();
        msg.TicketId = ticketId;
        _httpSender.StopMatchReq(msg);
        UnityUtil.Print("SEND MATCH STOP => ticketid", ticketId, "green");
    }


    void OnStopMatchAck(MsgStopMatchAck msg)
    {
        UnityUtil.Print("RECV MATCH STOP => userid", UserInfoManager.Get().GetUserInfo().userID, "green");
    }


    public void UpdateDeckReq(string userId, sbyte deckIndex, int[] deckIds)
    {
        MsgUpdateDeckReq msg = new MsgUpdateDeckReq();
        msg.UserId = userId;
        msg.DeckIndex = deckIndex;
        msg.DiceIds = deckIds;
        _httpSender.UpdateDeckReq(msg);
        UnityUtil.Print("SEND DECK UPDATE => index", string.Format("index:{0}, deck:[{1}]", deckIndex, string.Join(",", deckIds)), "green");
    }


    void OnUpdateDeckAck(MsgUpdateDeckAck msg)
    {
        UserInfoManager.Get().GetUserInfo().SetDeck(msg.DeckIndex, msg.DiceIds);

        ED.UI_Panel_Dice panelDice = FindObjectOfType<ED.UI_Panel_Dice>();
        panelDice.CallBackDeckUpdate();
        UnityUtil.Print("RECV DECK UPDATE => userid", string.Format("index:{0}, deck:[{1}]", msg.DeckIndex, string.Join(",", msg.DiceIds)), "green");
    }



    public void OpenBoxReq(string userId, int boxId, Action<MsgOpenBoxAck> callback)
    {
        MsgOpenBoxReq msg = new MsgOpenBoxReq();
        msg.UserId = userId;
        msg.BoxId = boxId;
        _boxOpenCallback = callback;
        _httpSender.OpenBoxReq(msg);
        UnityUtil.Print("SEND OPEN BOX => index", string.Format("boxId:{0}", boxId), "green");
    }

    void OnOpenBoxAck(MsgOpenBoxAck msg)
    {
        if (_boxOpenCallback != null)
        {
            _boxOpenCallback(msg);
        }
        UnityUtil.Print("RECV OPEN BOX => userid", UserInfoManager.Get().GetUserInfo().userID, "green");
    }




    public void LevelUpDiceReq(string userId, int diceId, Action<MsgLevelUpDiceAck> callback)
    {
        MsgLevelUpDiceReq msg = new MsgLevelUpDiceReq();
        msg.UserId = userId;
        msg.DiceId = diceId;
        _diceLevelUpCallback = callback;
        _httpSender.LevelUpDiceReq(msg);
        UnityUtil.Print("SEND LEVELUP DICE => index", string.Format("diceId:{0}", diceId), "green");
    }

    void OnLevelUpDiceAck(MsgLevelUpDiceAck msg)
    {
        if (_diceLevelUpCallback != null)
        {
            _diceLevelUpCallback(msg);
        }
        UnityUtil.Print("RECV LEVELUP DICE => userid", UserInfoManager.Get().GetUserInfo().userID, "green");
    }


    public void GetRankReq(string userId, Action<MsgGetRankAck> callback)
    {
        MsgGetRankReq msg = new MsgGetRankReq();
        msg.UserId = userId;
        _getRankCallback = callback;
        _httpSender.GetRankReq(msg);
        UnityUtil.Print("SEND GET RANK => index", string.Format("userId:{0}", userId), "green");
    }

    void OnGetRankAck(MsgGetRankAck msg)
    {
        if (_getRankCallback != null)
        {
            _getRankCallback(msg);
        }
    }

    #endregion
}

#region net battle save info

[Serializable]
public class NetBattleInfo
{
    public string serverAddr;
    public int serverPort;
    public string serverSession;
    public bool battleStart;

    public DateTime battleStartTime;

    public void ResetInfo()
    {
        serverAddr = "";
        serverPort = 0;
        serverSession = "";
        battleStart = false;

        battleStartTime = DateTime.UtcNow;
    }
}
#endregion



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
        /*for(int i = 0 ; i < playerInfo.DiceIdArray.Length ; i++ )
            UnityEngine.Debug.Log(playerInfo.DiceIdArray[i]);*/
        myInfoGet = true;
    }

    public void SetOtherInfo(MsgPlayerInfo info)
    {
        otherInfo = info;
        /*for(int i = 0 ; i < otherInfo.DiceIdArray.Length ; i++ )
            UnityEngine.Debug.Log(otherInfo.DiceIdArray[i]);*/
        otherInfoGet = true;
    }

    public void SetPlayerBase(MsgPlayerBase baseinfo)
    {
        MsgPlayerInfo pinfo = new MsgPlayerInfo();

        pinfo.PlayerUId = baseinfo.PlayerUId;
        pinfo.IsBottomPlayer = baseinfo.IsBottomPlayer;
        pinfo.Name = baseinfo.Name;
        
        playerInfo = pinfo;
    }

    public void SetOtherBase(MsgPlayerBase baseinfo)
    {
        MsgPlayerInfo pinfo = new MsgPlayerInfo();

        pinfo.PlayerUId = baseinfo.PlayerUId;
        pinfo.IsBottomPlayer = baseinfo.IsBottomPlayer;
        pinfo.Name = baseinfo.Name;
        
        otherInfo = pinfo;
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


#region net convert class

public class ConvertNetMsg
{

    #region convert minion data
    public static MsgSyncMinionData[] ConvertNetSyncToMsg(NetSyncData syncData)
    {
        //syncData.netSyncMinionData.Count
        MsgSyncMinionData[] convData = new MsgSyncMinionData[syncData.netSyncMinionData.Count];
        // 설마 100개를 넘진...
        for (int i = 0; i < syncData.netSyncMinionData.Count; i++)
        {
            convData[i] = new MsgSyncMinionData();
            convData[i].minionId = MsgIntToUshort(syncData.netSyncMinionData[i].minionId);
            convData[i].minionDataId = syncData.netSyncMinionData[i].minionDataId;
            convData[i].minionHp = MsgFloatToInt(syncData.netSyncMinionData[i].minionHp);
            convData[i].minionMaxHp = MsgFloatToInt(syncData.netSyncMinionData[i].minionMaxHp);
            convData[i].minionPower = MsgFloatToInt(syncData.netSyncMinionData[i].minionPower);
            convData[i].minionEffect = MsgFloatToInt(syncData.netSyncMinionData[i].minionEffect);
            convData[i].minionEffectUpgrade = MsgFloatToShort(syncData.netSyncMinionData[i].minionEffectUpgrade);
            convData[i].minionEffectIngameUpgrade =
                MsgFloatToShort(syncData.netSyncMinionData[i].minionEffectIngameUpgrade);
            convData[i].minionDuration = MsgFloatToShort(syncData.netSyncMinionData[i].minionDuration);
            convData[i].minionCooltime = MsgFloatToShort(syncData.netSyncMinionData[i].minionCooltime);

            convData[i].minionPos = Vector3ToMsg(syncData.netSyncMinionData[i].minionPos);
        }

        return convData;
    }

    public static List<NetSyncMinionData> ConvertMsgToSync(MsgSyncMinionData[] minionData)
    {
        List<NetSyncMinionData> syncData = new List<NetSyncMinionData>();

        for (int i = 0; i < minionData.Length; i++)
        {
            if (minionData[i].minionHp <= 0) continue;

            NetSyncMinionData miniondata = new NetSyncMinionData();

            miniondata.minionId = minionData[i].minionId;
            miniondata.minionDataId = minionData[i].minionDataId;
            miniondata.minionHp = MsgIntToFloat(minionData[i].minionHp);
            miniondata.minionMaxHp = MsgIntToFloat(minionData[i].minionMaxHp);
            miniondata.minionPower = MsgIntToFloat(minionData[i].minionPower);
            miniondata.minionEffect = MsgIntToFloat(minionData[i].minionEffect);
            miniondata.minionEffectUpgrade = MsgIntToFloat(minionData[i].minionEffectUpgrade);
            miniondata.minionEffectIngameUpgrade = MsgIntToFloat(minionData[i].minionEffectIngameUpgrade);
            miniondata.minionDuration = MsgIntToFloat(minionData[i].minionDuration);
            miniondata.minionCooltime = MsgIntToFloat(minionData[i].minionCooltime);

            miniondata.minionPos = MsgToVector3(minionData[i].minionPos);

            syncData.Add(miniondata);
        }

        return syncData;
    }
    #endregion

    #region server msg convert

    public static ushort[] MsgIntArrToUshortArr(int[] value)
    {
        ushort[] rtn = new ushort[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            rtn[i] = MsgIntToUshort(value[i]);
        }

        return rtn;
    }

    public static int[] MsgUshortArrToIntArr(ushort[] value)
    {
        int[] rtn = new int[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            rtn[i] = MsgUshortToInt(value[i]);
        }

        return rtn;
    }
    
    public static int MsgByteToInt(byte value)
    {
        return Convert.ToInt32(value);
    }

    public static byte MsgIntToByte(int value)
    {
        return Convert.ToByte(value);
    }

    public static short MsgIntToShort(int value)
    {
        return Convert.ToInt16(value);
    }

    public static int MsgShortToInt(short value)
    {
        return Convert.ToInt32(value);
    }

    public static ushort MsgIntToUshort(int value)
    {
        return Convert.ToUInt16(value);
    }

    public static int MsgUshortToInt(ushort value)
    {
        return Convert.ToInt32(value);
    }

    public static short MsgFloatToShort(float value)
    {
        return Convert.ToInt16(value * 100);
    }

    public static float MsgShortToFloat(short value)
    {
        return Convert.ToInt32(value) * 0.01f;
    }

    public static ushort MsgFloatToUshort(float value)
    {
        return Convert.ToUInt16(value * 100);
    }

    public static float MsgUshortToFloat(ushort value)
    {
        return Convert.ToInt32(value) * 0.01f;
    }

    public static byte MsgFloatToByte(float value)
    {
        return Convert.ToByte(value * 100);
    }

    public static float MsgByteToFloat(byte value)
    {
        return Convert.ToInt32(value) * 0.01f;
    }
    
    public static int MsgFloatToInt(float value)
    {
        return Convert.ToInt32(value * 100);
    }

    public static float MsgIntToFloat(int value)
    {
        return value * 0.01f;
    }

    public static MsgVector2 Vector3ToMsg(Vector2 val)
    {
        MsgVector2 chVec = new MsgVector2();

        chVec.X = MsgFloatToShort(val.x);
        chVec.Y = MsgFloatToShort(val.y);

        return chVec;
    }

    public static MsgVector3 Vector3ToMsg(Vector3 val)
    {
        MsgVector3 chVec = new MsgVector3();

        chVec.X = MsgFloatToShort(val.x);
        chVec.Y = MsgFloatToShort(val.y);
        chVec.Z = MsgFloatToShort(val.z);

        return chVec;
    }

    public static MsgQuaternion QuaternionToMsg(Quaternion quat)
    {
        MsgQuaternion chMsgQuat = new MsgQuaternion();

        chMsgQuat.X = MsgFloatToShort(quat.x);
        chMsgQuat.Y = MsgFloatToShort(quat.y);
        chMsgQuat.Z = MsgFloatToShort(quat.z);
        chMsgQuat.W = MsgFloatToShort(quat.w);

        return chMsgQuat;
    }

    public static Vector3 MsgToVector3(MsgVector2 msgVec)
    {
        Vector3 vecVal = new Vector3();

        vecVal.x = MsgIntToFloat(msgVec.X);
        vecVal.y = 0;
        vecVal.z = MsgIntToFloat(msgVec.Y);

        return vecVal;
    }

    public static Vector3 MsgToVector3(MsgVector3 msgVec)
    {
        Vector3 vecVal = new Vector3();

        vecVal.x = MsgIntToFloat(msgVec.X);
        vecVal.y = MsgIntToFloat(msgVec.Y);
        vecVal.z = MsgIntToFloat(msgVec.Z);

        return vecVal;
    }

    public static Vector3 MsgToVector3(int[] msgVec)
    {
        Vector3 vecVal = new Vector3();

        vecVal.x = MsgIntToFloat(msgVec[0]);
        vecVal.y = MsgIntToFloat(msgVec[1]);
        vecVal.z = MsgIntToFloat(msgVec[2]);

        return vecVal;
    }

    public static Quaternion MsgToQuaternion(MsgQuaternion quat)
    {
        Quaternion quatVal = new Quaternion();

        quatVal.x = MsgIntToFloat(quat.X);
        quatVal.y = MsgIntToFloat(quat.Y);
        quatVal.z = MsgIntToFloat(quat.Z);
        quatVal.w = MsgIntToFloat(quat.W);

        return quatVal;
    }

    public static MsgHitDamageMinionRelay GetHitDamageMinionRelayMsg(int uid, int id, float damage)
    {
        MsgHitDamageMinionRelay msg = new MsgHitDamageMinionRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Damage = MsgFloatToInt(damage);

        return msg;
    }


    public static MsgDestroyMinionRelay GetDestroyMinionRelayMsg(int uid, int id)
    {
        MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }


    public static MsgDestroyMagicRelay GetDestroyMagicRelayMsg(int uid, int id)
    {
        MsgDestroyMagicRelay msg = new MsgDestroyMagicRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.BaseStatId = MsgIntToUshort(id);

        return msg;
    }

    public static MsgFireballBombRelay GetFireballBombRelayMsg(int uid, int id)
    {
        MsgFireballBombRelay msg = new MsgFireballBombRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgHealMinionRelay GetHealMinionRelayMsg(int uid, int id, float heal)
    {
        MsgHealMinionRelay msg = new MsgHealMinionRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Heal = MsgFloatToInt(heal);

        return msg;
    }

    public static MsgMineBombRelay GetMineBombRelayMsg(int uid, int id)
    {
        MsgMineBombRelay msg = new MsgMineBombRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgSturnMinionRelay GetSturnMinionRelayMsg(int uid, int id, int sturnTime)
    {
        MsgSturnMinionRelay msg = new MsgSturnMinionRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.SturnTime = MsgIntToShort(sturnTime);

        return msg;
    }

    public static MsgRocketBombRelay GetRocketBombRelayMsg(int uid, int id)
    {
        MsgRocketBombRelay msg = new MsgRocketBombRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgIceBombRelay GetIceBombRelayMsg(int uid, int id)
    {
        MsgIceBombRelay msg = new MsgIceBombRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgFireManFireRelay GetFireManFireRelayMsg(int uid, int id)
    {
        MsgFireManFireRelay msg = new MsgFireManFireRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgMinionCloackingRelay GetMinionCloackingRelayMsg(int uid, int id, bool isCloacking)
    {
        MsgMinionCloackingRelay msg = new MsgMinionCloackingRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.IsCloacking = isCloacking;

        return msg;
    }

    public static MsgMinionFlagOfWarRelay GetMinionFlagOfWarRelayMsg(int uid, int id, int effect, bool isFlagOfWar)
    {
        MsgMinionFlagOfWarRelay msg = new MsgMinionFlagOfWarRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.BaseStatId = MsgIntToUshort(id);
        msg.Effect = MsgIntToShort(effect);
        msg.IsFogOfWar = isFlagOfWar;

        return msg;
    }

    public static MsgScarecrowRelay GetScarecrowRelayMsg(int uid, int id, int eyeLevel)
    {
        MsgScarecrowRelay msg = new MsgScarecrowRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.BaseStatId = MsgIntToUshort(id);
        msg.EyeLevel = MsgIntToByte(eyeLevel);

        return msg;
    }

    public static MsgLayzerTargetRelay GetLayzerTargetRelayMsg(int uid, int id, int[] target)
    {
        MsgLayzerTargetRelay msg = new MsgLayzerTargetRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.TargetIdArray = MsgIntArrToUshortArr(target);

        return msg;
    }

    public static MsgMinionInvincibilityRelay GetMinionInvincibilityRelayMsg(int uid, int id, int time)
    {
        MsgMinionInvincibilityRelay msg = new MsgMinionInvincibilityRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Time = MsgIntToShort(time);

        return msg;
    }

    public static MsgFireBulletRelay GetFireBulletRelayMsg(int uid, int id, int x, int y, int z, int damage, int speed, int type)
    {
        MsgFireBulletRelay msg = new MsgFireBulletRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Dir = new MsgVector3 { X = MsgIntToShort(x), Y = MsgIntToShort(y), Z = MsgIntToShort(z) };
        msg.Damage = damage;
        msg.MoveSpeed = MsgIntToShort(speed);
        msg.Type = MsgIntToByte(type);

        return msg;
    }

    public static MsgFireCannonBallRelay GetFireCannonBallRelayMsg(int uid, MsgVector3 shootPos,
        MsgVector3 targetPos, int damage, int range, int type)
    {
        MsgFireCannonBallRelay msg = new MsgFireCannonBallRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.ShootPos = shootPos;
        msg.TargetPos = targetPos;
        msg.Power = damage;
        msg.Range = MsgIntToShort(range);
        msg.Type = MsgIntToByte(type);

        return msg;
    }

    public static MsgSetMinionAnimationTriggerRelay GetMinionAnimationTriggerRelayMsg(int uid, int id, int trigger,
        int target)
    {
        MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Trigger = MsgIntToByte(trigger);
        msg.TargetId = MsgIntToUshort(target);

        return msg;
    }

    public static MsgSetMagicTargetIdRelay GetMagicTargetIDRelayMsg(int uid, int id, int targetID)
    {
        MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.TargetId = MsgIntToUshort(targetID);
        
        return msg;
    }

    public static MsgSetMagicTargetRelay GetMagicTargetPosRelayMsg(int uid, int id, int x, int z)
    {
        MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.X = MsgIntToShort(x);
        msg.Z = MsgIntToShort(z);
        
        return msg;
    }

    public static MsgActivatePoolObjectRelay GetActivatePoolObjectRelayMsg(int poolName, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        MsgActivatePoolObjectRelay msg = new MsgActivatePoolObjectRelay();

        msg.PoolName = poolName;
        msg.HitPos = Vector3ToMsg(pos);
        msg.Rotation = QuaternionToMsg(rot);
        msg.LocalScale = Vector3ToMsg(scale);
        
        return msg;
    }

    public static MsgSendMessageVoidRelay GetSendMessageVoidRelayMsg(int uid, int id, int message)
    {
        MsgSendMessageVoidRelay msg = new MsgSendMessageVoidRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Message = MsgIntToByte(message);
        
        return msg;
    }

    public static MsgSendMessageParam1Relay GetSendMessageParam1RelayMsg(int uid, int id, int message, int targetID)
    {
        MsgSendMessageParam1Relay msg = new MsgSendMessageParam1Relay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Message = MsgIntToByte(message);
        msg.TargetId = MsgIntToUshort(targetID);
        
        return msg;
    }

    public static MsgSetMinionTargetRelay GetMinionTargetRelayMsg(int uid, int id, int targetID)
    {
        MsgSetMinionTargetRelay msg = new MsgSetMinionTargetRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.TargetId = MsgIntToUshort(targetID);
        
        return msg;
    }

    public static MsgPushMinionRelay GetPushMinionRelayMsg(int uid, int id, int x, int y, int z, int power)
    {
        MsgPushMinionRelay msg = new MsgPushMinionRelay();

        msg.PlayerUId = MsgIntToUshort(uid);
        msg.Id = MsgIntToUshort(id);
        msg.Dir = new MsgVector3 { X = MsgIntToShort(x), Y = MsgIntToShort(y), Z = MsgIntToShort(z) };
        msg.PushPower = MsgIntToShort(power);
        
        return msg;
    }

    #endregion

}
#endregion


