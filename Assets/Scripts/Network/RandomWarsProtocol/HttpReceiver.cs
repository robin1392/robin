using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RandomWarsService.Network.Http;

namespace RandomWarsProtocol
{
    public class HttpReceiver : IHttpReceiver
    {
        public delegate Task<string> AuthUserReqDelegate(string userId);
        public AuthUserReqDelegate AuthUserReq;
        public delegate void AuthUserAckDelegate(GameErrorCode error, MsgUserInfo userInfo, MsgUserDeck[] userDeck, MsgUserDice[] userDice);
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

                        string userId = "";
                        ackJson = await AuthUserReq(userId);
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

                        GameErrorCode errorCode = GameErrorCode.SUCCESS;
                        MsgUserInfo userInfo = null;
                        MsgUserDeck[] userDeck = null;
                        MsgUserDice[] userDice = null;
                        AuthUserAck(errorCode, userInfo, userDeck, userDice);
                    }
                    break;
            }

            return true;
        }
    }
}
