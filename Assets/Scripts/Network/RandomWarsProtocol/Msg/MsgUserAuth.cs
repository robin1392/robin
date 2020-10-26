using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgUserAuthReq
    {
        public string UserId;
    }

    [Serializable]
    public class MsgUserAuthAck
    {
        public short ErrorCode;
        public MsgUserInfo UserInfo;
        public MsgUserDeck[] UserDeck;
        public MsgUserDice[] UserDice;
    }
}
