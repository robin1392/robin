using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;

namespace RandomWarsProtocol
{
    public class HttpReceiver : IHttpReceiver
    {
        public delegate Task<string> AuthUserReqDelegate(MsgUserAuthReq msg);
        public AuthUserReqDelegate AuthUserReq;
        public delegate void AuthUserAckDelegate(MsgUserAuthAck msg);
        public AuthUserAckDelegate AuthUserAck;

        public delegate Task<string> UpdateDeckReqDelegate(MsgUpdateDeckReq msg);
        public UpdateDeckReqDelegate UpdateDeckReq;
        public delegate void UpdateDeckAckDelegate(MsgUpdateDeckAck msg);
        public UpdateDeckAckDelegate UpdateDeckAck;

        public delegate Task<string> StartMatchReqDelegate(MsgStartMatchReq msg);
        public StartMatchReqDelegate StartMatchReq;
        public delegate void StartMatchAckDelegate(MsgStartMatchAck msg);
        public StartMatchAckDelegate StartMatchAck;

        public delegate Task<string> StatusMatchReqDelegate(MsgStatusMatchReq msg);
        public StatusMatchReqDelegate StatusMatchReq;
        public delegate void StatusMatchAckDelegate(MsgStatusMatchAck msg);
        public StatusMatchAckDelegate StatusMatchAck;

        public delegate Task<string> StopMatchReqDelegate(MsgStopMatchReq msg);
        public StopMatchReqDelegate StopMatchReq;
        public delegate void StopMatchAckDelegate(MsgStopMatchAck msg);
        public StopMatchAckDelegate StopMatchAck;



        IJsonSerializer _jsonSerializer;

        public HttpReceiver(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }


        public async Task<string> ProcessAsync(int protocolId, string json)
        {
            string ackJson = string.Empty;
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_REQ:
                    {
                        if (AuthUserReq == null)
                            return ackJson;

                        MsgUserAuthReq msg = _jsonSerializer.DeserializeObject<MsgUserAuthReq>(json);
                        ackJson = await AuthUserReq(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_REQ:
                    {
                        if (UpdateDeckReq == null)
                            return ackJson;

                        MsgUpdateDeckReq msg = _jsonSerializer.DeserializeObject<MsgUpdateDeckReq>(json);
                        ackJson = await UpdateDeckReq(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_REQ:
                    {
                        if (StartMatchReq == null)
                            return ackJson;

                        MsgStartMatchReq msg = _jsonSerializer.DeserializeObject<MsgStartMatchReq>(json);
                        ackJson = await StartMatchReq(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_REQ:
                    {
                        if (StatusMatchReq == null)
                            return ackJson;

                        MsgStatusMatchReq msg = _jsonSerializer.DeserializeObject<MsgStatusMatchReq>(json);
                        ackJson = await StatusMatchReq(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_REQ:
                    {
                        if (StopMatchReq == null)
                            return ackJson;

                        MsgStopMatchReq msg = _jsonSerializer.DeserializeObject<MsgStopMatchReq>(json);
                        ackJson = await StopMatchReq(msg);
                    }
                    break;
            }

            return ackJson;
        }


        public bool Process(int protocolId, string json)
        {
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_ACK:
                    {
                        if (AuthUserAck == null)
                            return false;

                        MsgUserAuthAck msg = _jsonSerializer.DeserializeObject<MsgUserAuthAck>(json);
                        AuthUserAck(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_ACK:
                    {
                        if (UpdateDeckAck == null)
                            return false;

                        MsgUpdateDeckAck msg = _jsonSerializer.DeserializeObject<MsgUpdateDeckAck>(json);
                        UpdateDeckAck(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_ACK:
                    {
                        if (StartMatchAck == null)
                            return false;

                        MsgStartMatchAck msg = _jsonSerializer.DeserializeObject<MsgStartMatchAck>(json);
                        StartMatchAck(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_ACK:
                    {
                        if (StatusMatchAck == null)
                            return false;

                        MsgStatusMatchAck msg = _jsonSerializer.DeserializeObject<MsgStatusMatchAck>(json);
                        StatusMatchAck(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_ACK:
                    {
                        if (StopMatchAck == null)
                            return false;

                        MsgStopMatchAck msg = _jsonSerializer.DeserializeObject<MsgStopMatchAck>(json);
                        StopMatchAck(msg);
                    }
                    break;
            }

            return true;
        }
    }
}
