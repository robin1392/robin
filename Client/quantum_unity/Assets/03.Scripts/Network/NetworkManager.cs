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
    public Global.E_MATCHSTEP NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;

    
    public static GameBaseClientSession session;
    
    public Action inappViewLockCallback = null;
    public Action inappViewUnLockCallback = null;
    public void SetViewLockEvents(System.Action lockEvent, System.Action unlockEvent)
    {
        inappViewLockCallback = lockEvent;
        inappViewUnLockCallback = unlockEvent;
    }

    public PLAY_TYPE playType;
    public string UserId;

    public MatchInfo LastMatchInfo;
    public class MatchInfo
    {
        public string PlayerGameSession;
        public PLAY_TYPE PlayType;
        public string ServerAddress;
        public int Port;    
    }
    
    public int matchSendCount;

    [Header("Server Addr")] 
    public string serverAddr = "https://er12bk2rue.execute-api.ap-northeast-2.amazonaws.com/test";

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

    public override void Awake()
    {
        if (Get() != null && this != Get())
        {
            Destroy(this.gameObject);
            return;
        }

        base.Awake();
        
        session = new GameBaseClientSession();
        session.Init(new GameBaseClientConfig
        {
            Logger = new NetLogger2(),
        });
    }

    void Update()
    {
        if (session != null)
        {
            session.Update();
        }
    }

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

    public bool IsConnect()
    {
        var client = FindObjectOfType<RWNetworkClient>();
        if (client == null)
        {
            return false;
        }
        return client.IsConnected;
    }
    
    public bool CheckReconnection()
    {
        //사전에 접속정보를 파일에 저장해두고 그 파일을 읽어드린다.
        return false;
    }

    //TODO: 재접속 복구해야함
    public void ReconnectPacket(MsgReconnectGameAck msg)
    {
        //서버 접속이 끊어지면 메인으로 보낸다.
        // 에러코드가 0 이 아닐경우는 게임에 이상이 있다는 서버 메세지니...그냥 리턴 시켜서..메인으로
        // if (msg.ErrorCode != 0)
        // {
        //     //DeleteBattleInfo();
        //     // DisconnectSocket(false);
        //
        //     SetReconnect(false);
        //
        //     //
        //     GameStateManager.Get().MoveMainScene();
        //     return;
        // }
        //
        // print("my info : " + msg.PlayerBase.PlayerUId + "  " + msg.PlayerBase.IsBottomPlayer + "    " + msg.PlayerBase.Name);
        // print("other info : " + msg.OtherPlayerBase.PlayerUId + "  " + msg.OtherPlayerBase.IsBottomPlayer + "    " + msg.OtherPlayerBase.Name);
        // _netInfo.SetPlayerBase(msg.PlayerBase);
        // _netInfo.SetOtherBase(msg.OtherPlayerBase);
        //
        GameStateManager.Get().MoveInGameBattle();
    }

    IEnumerator WaitForMatch()
    {
        yield return new WaitForSeconds(2.0f);
        matchSendCount++;
        StatusMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
    }


    public void StartMatchReq(EGameMode gameMode, int deckIndex)
    {
        if (NetMatchStep == Global.E_MATCHSTEP.MATCH_START
            || NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
        {
            // 매칭 중이면 요청할 수 없다.
            return;
        }

        matchSendCount = 0;
        NetMatchStep = Global.E_MATCHSTEP.MATCH_START;


        session.MatchTemplate.MatchRequestReq(session.HttpClient, (int)gameMode, deckIndex, OnStartMatchAck);
    }


    public bool OnStartMatchAck(ERandomwarsMatchErrorCode errorCode, string ticketId)
    {
        if (string.IsNullOrEmpty(ticketId))
        {
            UnityUtil.Print("ticket id null");
            return false;
        }

        NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
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
            UI_Main.Get().searchingPopup.ClickSearchingCancelResult();
        }
        else
        {
            NetMatchStep = Global.E_MATCHSTEP.MATCH_START;
        }
        return true;
    }
}


