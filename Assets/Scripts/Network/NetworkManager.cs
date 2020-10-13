#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using RWGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using RWCoreNetwork;
using RWGameProtocol.Msg;
using RWGameProtocol.Serializer;
using UnityEngine.Events;
using RWCoreNetwork.NetPacket;

public class NetworkManager : Singleton<NetworkManager>
{
    #region net variable

    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    // socket
    private SocketManager _clientSocket = null;
    // sender 
    private PacketSender _packetSend;

    /*public GamePacketSender SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }*/

    // 외부에서 얘를 건들일은 없도록하지
    private PacketReceiver _packetRecv;


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
    private bool _isOtherDisconnect;
    public bool IsOtherPause => _isOtherDisconnect;
    public UnityEvent<bool> event_OtherDisconnect = new UnityEvent<bool>();


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
        _packetSend = new StreamPacketSender();

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
        event_OtherDisconnect.Invoke(disconnect);
        _isOtherDisconnect = disconnect;
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
        _packetRecv = new StreamPacketReceiver();

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
        _packetRecv.MinionFogOfWarRelay = _socketRecv.OnMinionFogOfWarRelay;
        _packetRecv.SendMessageVoidRelay = _socketRecv.OnSendMessageVoidRelay;
        _packetRecv.SendMessageParam1Relay = _socketRecv.OnSendMessageParam1Relay;
        _packetRecv.NecromancerBulletRelay = _socketRecv.OnNecromancerBulletRelay;
        _packetRecv.SetMinionTargetRelay = _socketRecv.OnSetMinionTargetRelay;

        _packetRecv.ScarecrowRelay = _socketRecv.OnScarecrowRelay;
        _packetRecv.LazerTargetRelay = _socketRecv.OnLazerTargetRelay;

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
        _packetRecv.PauseGameAck = _socketRecv.OnPauseGameAck;
        _packetRecv.PauseGameNotify = _socketRecv.OnPauseGameNotify;
        _packetRecv.ResumeGameAck = _socketRecv.OnResumeGameAck;
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
            convData[i].minionId = syncData.netSyncMinionData[i].minionId;
            convData[i].minionDataId = syncData.netSyncMinionData[i].minionDataId;
            convData[i].minionHp = MsgFloatToInt(syncData.netSyncMinionData[i].minionHp);
            convData[i].minionMaxHp = MsgFloatToInt(syncData.netSyncMinionData[i].minionMaxHp);
            convData[i].minionPower = MsgFloatToInt(syncData.netSyncMinionData[i].minionPower);
            convData[i].minionEffect = MsgFloatToInt(syncData.netSyncMinionData[i].minionEffect);
            convData[i].minionEffectUpgrade = MsgFloatToInt(syncData.netSyncMinionData[i].minionEffectUpgrade);
            convData[i].minionEffectIngameUpgrade =
                MsgFloatToInt(syncData.netSyncMinionData[i].minionEffectIngameUpgrade);
            convData[i].minionDuration = MsgFloatToInt(syncData.netSyncMinionData[i].minionDuration);
            convData[i].minionCooltime = MsgFloatToInt(syncData.netSyncMinionData[i].minionCooltime);

            convData[i].minionPos = VectorToMsg(syncData.netSyncMinionData[i].minionPos);
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

            miniondata.minionPos = MsgToVector(minionData[i].minionPos);

            syncData.Add(miniondata);
        }

        return syncData;
    }
    #endregion

    #region server msg convert

    public static int MsgFloatToInt(float value)
    {
        int convValue = (int)(value * Global.g_networkBaseValue);

        return convValue;
    }

    public static float MsgIntToFloat(int netValue)
    {
        float convValue = (float)netValue / Global.g_networkBaseValue;
        return convValue;
    }

    public static MsgVector3 VectorToMsg(Vector3 val)
    {
        MsgVector3 chVec = new MsgVector3();

        chVec.X = MsgFloatToInt(val.x);
        chVec.Y = MsgFloatToInt(val.y);
        chVec.Z = MsgFloatToInt(val.z);

        return chVec;
    }

    public static MsgQuaternion QuaternionToMsg(Quaternion quat)
    {
        MsgQuaternion chMsgQuat = new MsgQuaternion();

        chMsgQuat.X = MsgFloatToInt(quat.x);
        chMsgQuat.Y = MsgFloatToInt(quat.y);
        chMsgQuat.Z = MsgFloatToInt(quat.z);
        chMsgQuat.W = MsgFloatToInt(quat.w);

        return chMsgQuat;
    }

    public static Vector3 MsgToVector(MsgVector3 msgVec)
    {
        Vector3 vecVal = new Vector3();

        vecVal.x = MsgIntToFloat(msgVec.X);
        vecVal.y = MsgIntToFloat(msgVec.Y);
        vecVal.z = MsgIntToFloat(msgVec.Z);

        return vecVal;
    }

    public static Vector3 MsgToVector(int[] msgVec)
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

    #endregion

}
#endregion


