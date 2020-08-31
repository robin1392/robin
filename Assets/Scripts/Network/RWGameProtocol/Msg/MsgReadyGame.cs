using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReadyGameReq : Serializer<MsgReadyGameReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReadyGameAck : Serializer<MsgReadyGameAck>
    {
        public short ErrorCode;
        
        // 게임 플레이 여부(true이면 게임 시작)
        public bool IsPlayGame;
    }
}
