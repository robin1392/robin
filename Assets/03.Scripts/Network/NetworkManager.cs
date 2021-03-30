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
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.Events;
using Service.Core;
using Template.Match.RandomwarsMatch.Common;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsService.Network.Socket.NetSession;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using Percent.GameBaseClient;
using UnityEditor;
using Debug = ED.Debug;

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

    
    public static GameBaseClientSession session;
    
    public Action inappViewLockCallback = null;
    public Action inappViewUnLockCallback = null;
    public void SetViewLockEvents(System.Action lockEvent, System.Action unlockEvent)
    {
        inappViewLockCallback = lockEvent;
        inappViewUnLockCallback = unlockEvent;
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

    public MatchInfo LastMatchInfo;
    public class MatchInfo
    {
        public string PlayerGameSession;
        public PLAY_TYPE PlayType;
        public string ServerAddress;
        public int Port;    
    }

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

    [Header("Server Addr")] 
    public string serverAddr = "https://er12bk2rue.execute-api.ap-northeast-2.amazonaws.com/test";
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
        //
        _netInfo = new NetInfo();
        _recvJoinPlayerInfoCheck = false;

        SetReconnect(false);

        // recv 셋팅
        CombineRecvDelegate();
    }


    private void DestroyNetwork()
    {
        if (IsConnect())
        {
            
        }
        _netInfo = null;
    }
    #endregion


    #region connent
    
    public void ConnectServer(PLAY_TYPE type, string serverAddr, int port, string playerSessionId)
    {
        LastMatchInfo = new MatchInfo()
        {
            ServerAddress = serverAddr,
            Port = port,
            PlayerGameSession = playerSessionId,
            PlayType = type,
        };

        if (type == PLAY_TYPE.BATTLE)
        {
            GameStateManager.Get().MoveInGameBattle();    
        }
        else if(type == PLAY_TYPE.CO_OP)
        {
            GameStateManager.Get().MoveInGameCoop();    
        }
        else
        {
            Debug.LogError($"지원하지 않는 모드로 서버 접속 요청이 들어왔습니다. {type.ToString()}");
        }
    }


    public void OnClientReconnecting()
    {
        SetReconnect(true);        // reconnect
        // 시작하면서 상대 멈춤 초기화
        SetOtherDisconnect(false);    // disconnect
        SetResume(false);        // resume
    }

    public bool IsConnect()
    {
        var client = FindObjectOfType<RWNetworkClient>();
        if (client == null)
        {
            return false;
        }
        return client.IsConnected;
    }

    public void Send(GameProtocol protocol, params object[] param)
    {
        if (protocol != GameProtocol.MINION_STATUS_RELAY)
            UnityUtil.Print("SEND =>  ", protocol.ToString(), "magenta");
        
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
        
    }


    public void PauseGame()
    {
        
    }


    public void ResumeGame()
    {
        
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
    }

    #endregion


    #region reconnect to do
    public bool CheckReconnection()
    {
        //사전에 접속정보를 파일에 저장해두고 그 파일을 읽어드린다.
        return false;
    }

    public void ReconnectPacket(MsgReconnectGameAck msg)
    {
        //서버 접속이 끊어지면 메인으로 보낸다.
        // 에러코드가 0 이 아닐경우는 게임에 이상이 있다는 서버 메세지니...그냥 리턴 시켜서..메인으로
        if (msg.ErrorCode != 0)
        {
            //DeleteBattleInfo();
            // DisconnectSocket(false);

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
        yield return new WaitForSeconds(2.0f);
        matchSendCount++;
        StatusMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
    }


    public void StartMatchReq(string userId, EGameMode gameMode)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_START
            || NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중이면 요청할 수 없다.
            return;
        }

        matchSendCount = 0;
        NetMatchStep = Global.E_MATCHSTEP.MATCH_START;


        session.MatchTemplate.MatchRequestReq(session.HttpClient, (int)gameMode, OnStartMatchAck);
    }


    bool OnStartMatchAck(ERandomwarsMatchErrorCode errorCode, string ticketId)
    {
        if (string.IsNullOrEmpty(ticketId))
        {
            UnityUtil.Print("ticket id null");
            return false;
        }

        UserInfoManager.Get().SetTicketId(ticketId);
        StartCoroutine(WaitForMatch());
        return true;
    }


    public void StatusMatchReq(string ticketId)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중에서 상태 요청을 할 수 있다ㅏ.
            return;
        }

        session.MatchTemplate.MatchStatusReq(session.HttpClient, ticketId, OnStatusMatchAck);
    }


    bool OnStatusMatchAck(ERandomwarsMatchErrorCode errorCode, string playerSessionId, string ipAddress, int port)
    {
        if (string.IsNullOrEmpty(playerSessionId))
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
            ConnectServer(playType, ipAddress, port, playerSessionId);
        }
        return true;
    }


    public void StopMatchReq(string ticketId)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
        {
            return;
        }

        NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
        session.MatchTemplate.MatchCancelReq(session.HttpClient, ticketId, OnStopMatchAck);
    }


    bool OnStopMatchAck(ERandomwarsMatchErrorCode errorCode)
    {
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
            UI_SearchingPopup searchingPopup = FindObjectOfType<UI_SearchingPopup>();
            searchingPopup.ClickSearchingCancelResult();
        }
        else
        {
            NetMatchStep = Global.E_MATCHSTEP.MATCH_START;
        }
        return true;
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

        vecVal.x = MsgShortToFloat(msgVec.X);
        vecVal.y = 0;
        vecVal.z = MsgShortToFloat(msgVec.Y);

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


