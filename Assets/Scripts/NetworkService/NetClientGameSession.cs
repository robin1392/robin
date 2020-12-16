using System.Collections;
using System.Collections.Generic;
using Service.Net;
using Template.Account.RandomWarsAccount;
using Template.Account.RandomWarsAccount.Common;
using Template.Item.RandomWarsDice;
using Template.Item.RandomWarsDice.Common;
using Template.Player.RandomWarsPlayer;
using Template.Player.RandomWarsPlayer.Common;
using Template.Stage.RandomWarsMatch;
using Template.Stage.RandomWarsMatch.Common;


public class NetClientGameSession : GameSessionClient
{
    public NetClientPlayer Player { get; set; }
    RandomWarsAccountTemplate _randomWarsAccountTemplate;
    RandomWarsPlayerTemplate _randomWarsPlayerTemplate;
    RandomWarsDiceTemplate _randomWarsDiceTemplate;
    RandomWarsMatchTemplate _randomWarsMatchTemplate;


    public NetClientGameSession()
    {
        Player = new NetClientPlayer();
        _randomWarsAccountTemplate = new RandomWarsAccountTemplate();
        _randomWarsPlayerTemplate = new RandomWarsPlayerTemplate();
        _randomWarsDiceTemplate = new RandomWarsDiceTemplate();
        _randomWarsMatchTemplate = new RandomWarsMatchTemplate();

        _messageController.AddControllers(_randomWarsAccountTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsPlayerTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsDiceTemplate.MessageControllers);
        _messageController.AddControllers(_randomWarsMatchTemplate.MessageControllers);

    }


    protected override void OnConnectClient(ClientSession clientSession)
    {
        Player.SetClientSession(clientSession);
        Send(ERandomWarsMatchProtocol.JOIN_MATCH_REQ, UserInfoManager.Get().GetActiveDeckIndex());
    }

    protected override void OnReconnectClient(ClientSession clientSession)
    {
    }

    protected override void OnOfflineClient(ClientSession clientSession)
    {
    }

    protected override void OnDisconnectClient(ClientSession clientSession)
    {
    }


    #region send
    public bool Send(ERandomWarsAccountProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ:
                {
                    _randomWarsAccountTemplate.SendLoginAccountReq(NetworkService.Get().HttpClient, param[0].ToString(), (EPlatformType)param[1]);
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsPlayerProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsPlayerProtocol.EDIT_NAME_REQ:
                {
                    _randomWarsPlayerTemplate.SendEditNameReq(NetworkService.Get().HttpClient, param[0].ToString(), param[1].ToString());
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsDiceProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsDiceProtocol.UPDATE_DECK_REQ:
                {
                    _randomWarsDiceTemplate.SendUpdateDeckReq(NetworkService.Get().HttpClient, param[0].ToString(), (int)param[1], (int[])param[2]);
                }
                break;
            case ERandomWarsDiceProtocol.LEVELUP_DICE_REQ:
                {
                    _randomWarsDiceTemplate.SendLevelupDiceReq(NetworkService.Get().HttpClient, param[0].ToString(), (int)param[1]);
                }
                break;
            case ERandomWarsDiceProtocol.OPEN_BOX_REQ:
                {
                    _randomWarsDiceTemplate.SendOpenBoxReq(NetworkService.Get().HttpClient, param[0].ToString(), (int)param[1]);
                }
                break;
        }
        return true;
    }


    public bool Send(ERandomWarsMatchProtocol protocolId, params object[] param)
    {
        switch (protocolId)
        {
            case ERandomWarsMatchProtocol.REQUEST_MATCH_REQ:
                {
                    if (NetworkService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_START
                        || NetworkService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
                    {
                        // 매칭 중이면 요청할 수 없다.
                        break;
                    }

                    NetworkService.Get().NetMatchStep = Global.E_MATCHSTEP.MATCH_START;
                    _randomWarsMatchTemplate.SendRequestMatchReq(NetworkService.Get().HttpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.STATUS_MATCH_REQ:
                {
                    if (NetworkService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
                    {
                        // 매칭 중에서 상태 요청을 할 수 있다ㅏ.
                        break;
                    }

                    _randomWarsMatchTemplate.SendStatusMatchReq(NetworkService.Get().HttpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.CANCEL_MATCH_REQ:
                {
                    if (NetworkService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
                    {
                        break;
                    }

                    NetworkService.Get().NetMatchStep = Global.E_MATCHSTEP.MATCH_CANCEL;
                    _randomWarsMatchTemplate.SendCancelMatchReq(NetworkService.Get().HttpClient, param[0].ToString());
                }
                break;
            case ERandomWarsMatchProtocol.JOIN_MATCH_REQ:
                {
                    _randomWarsMatchTemplate.SendJoinMatchReq(Player, (int)param[0]);
                }
                break;
        }

        UnityUtil.Print("SEND => ", string.Format("protocolId: {0}, params: {1}", protocolId.ToString(), string.Join(", ", param)), "green");

        return true;
    }


    #endregion

}
