using System;
using System.Collections.Generic;
using System.Text;

namespace RWGameProtocol
{
    public enum GameErrorCode : short
    {
        SUCCESS = 0,

        ERROR_ACCEPT_PLAYER_SESSION = 10001,
    }
}
