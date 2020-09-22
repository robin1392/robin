using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgUpgradeSpReq : Serializer<MsgUpgradeSpReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgUpgradeSpAck : Serializer<MsgUpgradeSpAck>
    {
        public short ErrorCode;

        // 인게임 업그레이드 수치
        public short Upgrade;

        // 현재 Sp
        public int CurrentSp;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgUpgradeSpNotify : Serializer<MsgUpgradeSpNotify>
    {
        // 플레이어 UID
        public int PlayerUId;

        // 인게임 업그레이드 수치
        public short Upgrade;
    }
}
