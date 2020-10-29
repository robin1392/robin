﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;
using Newtonsoft.Json;

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



        public async Task<string> ProcessAsync(int protocolId, string json)
        {
            string ackJson = string.Empty;
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_REQ:
                    {
                        if (AuthUserReq == null)
                            return ackJson;

                        MsgUserAuthReq msg = JsonConvert.DeserializeObject<MsgUserAuthReq>(json);
                        ackJson = await AuthUserReq(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_REQ:
                    {
                        if (UpdateDeckReq == null)
                            return ackJson;

                        MsgUpdateDeckReq msg = JsonConvert.DeserializeObject<MsgUpdateDeckReq>(json);
                        ackJson = await UpdateDeckReq(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_REQ:
                    {
                        if (StartMatchReq == null)
                            return ackJson;

                        MsgStartMatchReq msg = JsonConvert.DeserializeObject<MsgStartMatchReq>(json);
                        ackJson = await StartMatchReq(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_REQ:
                    {
                        if (StatusMatchReq == null)
                            return ackJson;

                        MsgStatusMatchReq msg = JsonConvert.DeserializeObject<MsgStatusMatchReq>(json);
                        ackJson = await StatusMatchReq(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_REQ:
                    {
                        if (StopMatchReq == null)
                            return ackJson;

                        MsgStopMatchReq msg = JsonConvert.DeserializeObject<MsgStopMatchReq>(json);
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

                        MsgUserAuthAck msg = JsonConvert.DeserializeObject<MsgUserAuthAck>(json);
                        AuthUserAck(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_ACK:
                    {
                        if (UpdateDeckAck == null)
                            return false;

                        MsgUpdateDeckAck msg = JsonConvert.DeserializeObject<MsgUpdateDeckAck>(json);
                        UpdateDeckAck(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_ACK:
                    {
                        if (StartMatchAck == null)
                            return false;

                        MsgStartMatchAck msg = JsonConvert.DeserializeObject<MsgStartMatchAck>(json);
                        StartMatchAck(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_ACK:
                    {
                        if (StatusMatchAck == null)
                            return false;

                        MsgStatusMatchAck msg = JsonConvert.DeserializeObject<MsgStatusMatchAck>(json);
                        StatusMatchAck(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_ACK:
                    {
                        if (StopMatchAck == null)
                            return false;

                        MsgStopMatchAck msg = JsonConvert.DeserializeObject<MsgStopMatchAck>(json);
                        StopMatchAck(msg);
                    }
                    break;
            }

            return true;
        }
    }
}
