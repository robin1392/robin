using RandomWarsProtocol.Msg;
using System;
using System.Collections.Generic;
using System.Text;
using RandomWarsService.Network.Http;
using Newtonsoft.Json;

namespace RandomWarsProtocol
{
    public class HttpReceiver : IHttpReceiver
    {
        public delegate void AuthUserAckDelegate(GameErrorCode error, MsgUserInfo userInfo, MsgUserDeck[] userDeck, MsgUserDice[] userDice);
        public AuthUserAckDelegate AuthUserAck;


        public bool Process(int protocolId, string json)
        {
            switch((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_ACK:
                    {
                        if (AuthUserAck == null)
                            return false;

                        MsgUserAuthAck msg = JsonConvert.DeserializeObject<MsgUserAuthAck>(json);
                        AuthUserAck((GameErrorCode)msg.ErrorCode, msg.UserInfo, msg.UserDeck, msg.UserDice);
                    }
                    break;
            }

            return true;
        }
    }
}
