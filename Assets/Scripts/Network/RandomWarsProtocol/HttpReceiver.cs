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


        IJsonSerializer _jsonSerializer;

        public HttpReceiver(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }


        public async Task<string> ProcessRequest(int protocolId, string json)
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
            }

            return ackJson;
        }


        public bool ProcessResponse(int protocolId, string json)
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
            }

            return true;
        }
    }
}
