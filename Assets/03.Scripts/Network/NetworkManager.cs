#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CodeStage.AntiCheat.ObscuredTypes;
using ED;
using UnityEngine;
using UnityEngine.Events;
using Service.Core;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsService.Network.Http;
using RandomWarsService.Network.Socket.NetSession;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using Percent.GameBaseClient;
using UnityEditor;

public class NetLogger2 : ILog
{
    public void Info(string text)
    {
        UnityEngine.Debug.Log(text);
    }

    public void Fatal(string text)
    {
        UnityEngine.Debug.LogError(text);
    }

    public void Error(string text)
    {
        UnityEngine.Debug.LogError(text);
    }

    public void Warn(string text)
    {
        UnityEngine.Debug.Log(text);
    }

    public void Debug(string text)
    {
        UnityEngine.Debug.Log(text);
    }
}

public class NetworkManager : Singleton<NetworkManager>
{
    #region net variable

    public Global.E_MATCHSTEP NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;

    SocketService _socketService;

    private HttpSender _httpSender;
    private HttpReceiver _httpReceiver;
    private HttpClient _httpClient;



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
    
    public static GameBaseClientSession session;
    
    public Action inappViewLockCallback = null;
    public Action inappViewUnLockCallback = null;
    public void SetViewLockEvents(System.Action lockEvent, System.Action unlockEvent)
    {
        inappViewLockCallback = lockEvent;
        inappViewUnLockCallback = unlockEvent;
    }

    #endregion

    //#region socket addr

    //private string _serverAddr;
    //public string serverAddr
    //{
    //    get => _serverAddr;
    //    private set => _serverAddr = value;
    //}

    //private int _port;

    //public int port
    //{
    //    get => _port;
    //    private set => _port = value;
    //}

    //private string _gameSession;

    //public string gameSession
    //{
    //    get => _gameSession;
    //    private set => _gameSession = value;
    //}

    //private string _battlePath = "/BattleInfo.bytes";

    //#endregion


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

    public int CoopUID
    {
        get => GetNetInfo().CoopUID();
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

    public string UserIdFromInspector
    {
        get
        {
            if (UserId.Length > 0)
            {
                return UserId;
            }

            var fallback = UserInfoManager.Get().GetUserInfo().userID; 
            return fallback;
        }
    }
    public int matchSendCount;
    #endregion


    #region socket user info
    // 통신간 정보를 담아둘만한 휘발성 클래스

    private NetInfo _netInfo = null;

    //private NetBattleInfo _battleInfo = null;

    private Action<MsgOpenBoxAck> _boxOpenCallback;
    private Action<MsgLevelUpDiceAck> _diceLevelUpCallback;
    private Action<MsgEditUserNameAck> _editUserNameCallback;
    private Action<MsgEndTutorialAck> _endTutorialCallback;
    private Action<UserSeasonInfoAck> _seasonInfoCallback;
    private Action<MsgGetRankAck> _getRankCallback;
    private Action<MsgGetSeasonPassRewardAck> _getSeasonPassRewardCallback;
    private Action<MsgGetClassRewardAck> _getClassRewardCallback;
    private Action<QuestInfoAck> _questInfoCallback;
    private Action<MsgQuestRewardAck> _questRewardCallback;
    private Action<QuestDayRewardAck> _questDayRewardCallback;
    private Action<MsgSeasonResetAck> _seasonResetCallback;
    private Action<MsgSeasonPassRewardStepAck> _seasonPassRewardStep;
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
        
        session = new GameBaseClientSession();
        session.Init(new GameBaseClientConfig
        {
            Logger = new NetLogger2(),
        });
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

        if (session != null)
        {
            session.Update();
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

        _httpReceiver.StartMatchAck = OnStartMatchAck;
        _httpReceiver.StatusMatchAck = OnStatusMatchAck;
        _httpReceiver.StopMatchAck = OnStopMatchAck;


        //
        _netInfo = new NetInfo();
        _recvJoinPlayerInfoCheck = false;

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
            DisconnectSocket(true);
        }

        //_battleInfo = null;

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

    //public void SetAddr(string serveraddr, int port, string gamesession)
    //{
    //    _serverAddr = serveraddr;
    //    _port = port;
    //    _gameSession = gamesession;

    //    _recvJoinPlayerInfoCheck = true;
    //    _netInfo.Clear();
    //}

    public void ConnectServer(Global.PLAY_TYPE type, string serverAddr, int port, string playerSessionId)
    {
        // 시작하면서 상대 디스커넥트
        SetOtherDisconnect(false);    // disconnect
        SetResume(false);        // resume
        SetReconnect(false);        // reconnect
        //playType = type;
        _recvJoinPlayerInfoCheck = true;
        _netInfo.Clear();

        _clientSocket.Connect(serverAddr, port, playerSessionId);
    }


    public void OnClientReconnecting()
    {
        SetReconnect(true);        // reconnect
        // 시작하면서 상대 멈춤 초기화
        SetOtherDisconnect(false);    // disconnect
        SetResume(false);        // resume
    }


    public void DisconnectSocket(bool unexpected)
    {
        if (_clientSocket != null && _clientSocket.IsConnected() == true)
            _clientSocket.Disconnect(unexpected == true ? ESessionState.None : ESessionState.Leave);
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
        _packetRecv.JoinCoopGameAck = _socketRecv.OnJoinCoopGameAck;
        _packetRecv.LeaveGameAck = _socketRecv.OnLeaveGameAck;
        _packetRecv.ReadyGameAck = _socketRecv.OnReadyGameAck;
        _packetRecv.GetDiceAck = _socketRecv.OnGetDiceAck;
        _packetRecv.MergeDiceAck = _socketRecv.OnMergeDiceAck;

        _packetRecv.UpgradeSpAck = _socketRecv.OnUpgradeSpAck;
        _packetRecv.InGameUpDiceAck = _socketRecv.OnInGameUpDiceAck;

        _packetRecv.HitDamageAck = _socketRecv.OnHitDamageAck;

        // notify
        _packetRecv.JoinGameNotify = _socketRecv.OnJoinGameNotify;
        _packetRecv.JoinCoopGameNotify = _socketRecv.OnJoinCoopGameNotify;
        _packetRecv.LeaveGameNotify = _socketRecv.OnLeaveGameNotify;
        _packetRecv.GetDiceNotify = _socketRecv.OnGetDiceNotify;
        _packetRecv.DeactiveWaitingObjectNotify = _socketRecv.OnDeactiveWaitingObjectNotify;
        _packetRecv.SpawnNotify = _socketRecv.OnSpawnNotify;
        _packetRecv.CoopSpawnNotify = _socketRecv.OnCoopSpawnNotify;
        _packetRecv.AddSpNotify = _socketRecv.OnAddSpNotify;
        _packetRecv.MonsterSpawnNotify = _socketRecv.OnMonsterSpawnNotify;

        _packetRecv.MergeDiceNotify = _socketRecv.OnMergeDiceNotify;
        _packetRecv.UpgradeSpNotify = _socketRecv.OnUpgradeSpNotify;
        _packetRecv.InGameUpDiceNotify = _socketRecv.OnInGameUpDiceNotify;

        _packetRecv.HitDamageNotify = _socketRecv.HitDamageNotify;
        _packetRecv.EndGameNotify = _socketRecv.OnEndGameNotify;
        _packetRecv.EndCoopGameNotify = _socketRecv.OnEndCoopGameNotify;


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


    #region reconnect to do
    public bool CheckReconnection()
    {
        return _clientSocket.CheckReconnection();
    }

    public void ReconnectPacket(MsgReconnectGameAck msg)
    {
        // 에러코드가 0 이 아닐경우는 게임에 이상이 있다는 서버 메세지니...그냥 리턴 시켜서..메인으로
        if (msg.ErrorCode != 0)
        {
            //DeleteBattleInfo();
            DisconnectSocket(false);

            SetReconnect(false);

            //
            GameStateManager.Get().MoveMainScene();
            return;
        }

        print("my info : " + msg.PlayerBase.PlayerUId + "  " + msg.PlayerBase.IsBottomPlayer + "    " + msg.PlayerBase.Name);
        print("other info : " + msg.OtherPlayerBase.PlayerUId + "  " + msg.OtherPlayerBase.IsBottomPlayer + "    " + msg.OtherPlayerBase.Name);
        _netInfo.SetPlayerBase(msg.PlayerBase);
        _netInfo.SetOtherBase(msg.OtherPlayerBase);

        //
        IsMaster = _netInfo.playerInfo.IsBottomPlayer;

        //
        GameStateManager.Get().MoveInGameBattle();
    }
    #endregion


    #region http


    IEnumerator WaitForMatch()
    {
        yield return new WaitForSeconds(1.0f);
        matchSendCount++;
        StatusMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
    }


    public void StartMatchReq(string userId, int gameMode)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_START
            || NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중이면 요청할 수 없다.
            return;
        }

        matchSendCount = 0;
        NetMatchStep = Global.E_MATCHSTEP.MATCH_START;

        MsgStartMatchReq msg = new MsgStartMatchReq();
        msg.UserId = userId;
        msg.GameMode = gameMode;
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

        UserInfoManager.Get().SetTicketId(msg.TicketId);
        UnityUtil.Print("RECV MATCH START => ticketId", msg.TicketId, "green");

        StartCoroutine(WaitForMatch());
    }


    public void StatusMatchReq(string ticketId)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중에서 상태 요청을 할 수 있다ㅏ.
            return;
        }


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
            if (NetMatchStep != Global.E_MATCHSTEP.MATCH_CANCEL)
            {
                if (matchSendCount < 30)
                {
                    StartCoroutine(WaitForMatch());
                }
                else
                {
                    UI_SearchingPopup searchingPopup = FindObjectOfType<UI_SearchingPopup>();
                    searchingPopup.ClickSearchingCancelResult();
                    UI_Main.Get().ShowMessageBox("매칭 실패", "매칭에 실패했습니다. 다시 시도해주세요.");
                }
            }
        }
        else
        {
            NetMatchStep = Global.E_MATCHSTEP.MATCH_CONNECT;

            // 우선 그냥 배틀로 지정하자
            ConnectServer(Global.PLAY_TYPE.BATTLE, msg.ServerAddr, msg.Port, msg.PlayerSessionId);
        }
        UnityUtil.Print("RECV MATCH STATUS => ", string.Format("server:{0}, player-session-id:{1}", msg.ServerAddr + ":" + msg.Port, msg.PlayerSessionId), "green");
    }


    public void StopMatchReq(string ticketId)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
        {
            return;
        }

        NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
        MsgStopMatchReq msg = new MsgStopMatchReq();
        msg.TicketId = ticketId;
        _httpSender.StopMatchReq(msg);
        UnityUtil.Print("SEND MATCH STOP => ticketid", ticketId, "green");
    }


    void OnStopMatchAck(MsgStopMatchAck msg)
    {
        if (msg.ErrorCode == GameErrorCode.SUCCESS || msg.ErrorCode == GameErrorCode.ERROR_GAMELIFT_MATCH_STATE_INVALID)
        {
            UI_SearchingPopup searchingPopup = FindObjectOfType<UI_SearchingPopup>();
            searchingPopup.ClickSearchingCancelResult();
        }
        else
        {
            NetMatchStep = Global.E_MATCHSTEP.MATCH_START;
        }

        UnityUtil.Print("RECV MATCH STOP => userid", UserInfoManager.Get().GetUserInfo().userID, "green");
        UnityUtil.Print("RECV MATCH STOP => ErrorCode", msg.ErrorCode.ToString(), "green");
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
    public MsgPlayerInfo coopInfo;
    public bool myInfoGet = false;
    public bool otherInfoGet = false;
    public bool coopInfoGet = false;

    public NetInfo()
    {

    }

    public void Clear()
    {
        myInfoGet = false;
        otherInfoGet = false;
        coopInfoGet = false;
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

    public void SetCoopInfo(MsgPlayerInfo info)
    {
        coopInfo = info;
        coopInfoGet = true;
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

    public int CoopUID()
    {
        return coopInfo.PlayerUId;
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

    public static uint[] MsgUshortArrToIntArr(ushort[] value)
    {
        uint[] rtn = new uint[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            rtn[i] = MsgUshortToUInt(value[i]);
        }

        return rtn;
    }

    private static uint MsgUshortToUInt(ushort value)
    {
        return Convert.ToUInt32(value);
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

    public static sbyte MsgFloatToByte(float value)
    {
        return Convert.ToSByte(Mathf.RoundToInt(value * 10));
    }

    public static float MsgByteToFloat(sbyte value)
    {
        return Convert.ToInt32(value) * 0.1f;
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

        chVec.X = MsgFloatToByte(val.x);
        chVec.Y = MsgFloatToByte(val.y);

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

        vecVal.x = MsgByteToFloat(msgVec.X);
        vecVal.y = 0;
        vecVal.z = MsgByteToFloat(msgVec.Y);

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


    public static MsgDestroyMinionRelay GetDestroyMinionRelayMsg(int id)
    {
        MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }


    public static MsgDestroyMagicRelay GetDestroyMagicRelayMsg(int id)
    {
        MsgDestroyMagicRelay msg = new MsgDestroyMagicRelay();

        msg.BaseStatId = MsgIntToUshort(id);

        return msg;
    }

    public static MsgFireballBombRelay GetFireballBombRelayMsg(int id)
    {
        MsgFireballBombRelay msg = new MsgFireballBombRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgHealMinionRelay GetHealMinionRelayMsg(int id, float heal)
    {
        MsgHealMinionRelay msg = new MsgHealMinionRelay();

        msg.Id = MsgIntToUshort(id);
        msg.Heal = MsgFloatToInt(heal);

        return msg;
    }

    public static MsgMineBombRelay GetMineBombRelayMsg(int id)
    {
        MsgMineBombRelay msg = new MsgMineBombRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgSturnMinionRelay GetSturnMinionRelayMsg(int id, int sturnTime)
    {
        MsgSturnMinionRelay msg = new MsgSturnMinionRelay();

        msg.Id = MsgIntToUshort(id);
        msg.SturnTime = MsgIntToShort(sturnTime);

        return msg;
    }

    public static MsgRocketBombRelay GetRocketBombRelayMsg(int id)
    {
        MsgRocketBombRelay msg = new MsgRocketBombRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgIceBombRelay GetIceBombRelayMsg(int id)
    {
        MsgIceBombRelay msg = new MsgIceBombRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgFireManFireRelay GetFireManFireRelayMsg(int id)
    {
        MsgFireManFireRelay msg = new MsgFireManFireRelay();

        msg.Id = MsgIntToUshort(id);

        return msg;
    }

    public static MsgMinionCloackingRelay GetMinionCloackingRelayMsg(int id, bool isCloacking)
    {
        MsgMinionCloackingRelay msg = new MsgMinionCloackingRelay();

        msg.Id = MsgIntToUshort(id);
        msg.IsCloacking = isCloacking;

        return msg;
    }

    public static MsgMinionFlagOfWarRelay GetMinionFlagOfWarRelayMsg(int id, int effect, bool isFlagOfWar)
    {
        MsgMinionFlagOfWarRelay msg = new MsgMinionFlagOfWarRelay();

        msg.BaseStatId = MsgIntToUshort(id);
        msg.Effect = MsgIntToShort(effect);
        msg.IsFogOfWar = isFlagOfWar;

        return msg;
    }

    public static MsgScarecrowRelay GetScarecrowRelayMsg(int id, int eyeLevel)
    {
        MsgScarecrowRelay msg = new MsgScarecrowRelay();

        msg.BaseStatId = MsgIntToUshort(id);
        msg.EyeLevel = MsgIntToByte(eyeLevel);

        return msg;
    }

    public static MsgLayzerTargetRelay GetLayzerTargetRelayMsg(int id, int[] target)
    {
        MsgLayzerTargetRelay msg = new MsgLayzerTargetRelay();

        msg.Id = MsgIntToUshort(id);
        msg.TargetIdArray = MsgIntArrToUshortArr(target);

        return msg;
    }

    public static MsgMinionInvincibilityRelay GetMinionInvincibilityRelayMsg(int id, int time)
    {
        MsgMinionInvincibilityRelay msg = new MsgMinionInvincibilityRelay();

        msg.Id = MsgIntToUshort(id);
        msg.Time = MsgIntToShort(time);

        return msg;
    }

    public static MsgFireBulletRelay GetFireBulletRelayMsg(uint id, uint targetId, int damage, int speed, int type)
    {
        MsgFireBulletRelay msg = new MsgFireBulletRelay();

        msg.Id = MsgUIntToUshort(id);
        msg.targetId = MsgUIntToUshort(targetId);
        msg.Damage = damage;
        msg.MoveSpeed = MsgIntToShort(speed);
        msg.Type = MsgIntToByte(type);

        return msg;
    }

    public static MsgFireCannonBallRelay GetFireCannonBallRelayMsg(MsgVector3 shootPos,
        MsgVector3 targetPos, int damage, int range, int type)
    {
        MsgFireCannonBallRelay msg = new MsgFireCannonBallRelay();

        msg.ShootPos = shootPos;
        msg.TargetPos = targetPos;
        msg.Power = damage;
        msg.Range = MsgIntToShort(range);
        msg.Type = MsgIntToByte(type);

        return msg;
    }

    public static MsgSetMinionAnimationTriggerRelay GetMinionAnimationTriggerRelayMsg(int id, int trigger,
        int target)
    {
        MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();

        msg.Id = MsgIntToUshort(id);
        msg.Trigger = MsgIntToByte(trigger);
        msg.TargetId = MsgIntToUshort(target);

        return msg;
    }

    public static MsgSetMagicTargetIdRelay GetMagicTargetIDRelayMsg(int id, uint targetId)
    {
        MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();

        msg.Id = MsgIntToUshort(id);
        msg.TargetId = MsgUIntToUshort(targetId);

        return msg;
    }

    public static MsgSetMagicTargetRelay GetMagicTargetPosRelayMsg(int id, int x, int z)
    {
        MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();

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

    public static MsgSendMessageVoidRelay GetSendMessageVoidRelayMsg(int id, int message)
    {
        MsgSendMessageVoidRelay msg = new MsgSendMessageVoidRelay();

        msg.Id = MsgIntToUshort(id);
        msg.Message = MsgIntToByte(message);

        return msg;
    }

    public static MsgSendMessageParam1Relay GetSendMessageParam1RelayMsg(int id, int message, uint targetId)
    {
        MsgSendMessageParam1Relay msg = new MsgSendMessageParam1Relay();

        msg.Id = MsgIntToUshort(id);
        msg.Message = MsgIntToByte(message);
        msg.TargetId = MsgUIntToUshort(targetId);

        return msg;
    }

    public static MsgSetMinionTargetRelay GetMinionTargetRelayMsg(int id, uint targetId)
    {
        MsgSetMinionTargetRelay msg = new MsgSetMinionTargetRelay();

        msg.Id = MsgIntToUshort(id);
        msg.TargetId = MsgUIntToUshort(targetId);

        return msg;
    }

    public static MsgPushMinionRelay GetPushMinionRelayMsg(int id, int x, int y, int z, int power)
    {
        MsgPushMinionRelay msg = new MsgPushMinionRelay();

        msg.Id = MsgIntToUshort(id);
        msg.Dir = new MsgVector3 { X = MsgIntToShort(x), Y = MsgIntToShort(y), Z = MsgIntToShort(z) };
        msg.PushPower = MsgIntToShort(power);

        return msg;
    }

    #endregion

    public static ushort MsgUIntToUshort(uint value)
    {
        return Convert.ToUInt16(value);
    }
}
#endregion


