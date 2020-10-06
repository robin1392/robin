using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameReq : Serializer<MsgStartSyncGameReq>
    {
        // 미니언 상태 정보
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameAck : Serializer<MsgStartSyncGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameNotify : Serializer<MsgStartSyncGameNotify>
    {
        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;

        // 상대방 정보
        public MsgPlayerInfo OtherPlayerInfo;

        // 상대방이 보낸 미니언 상태 정보

    }
}
