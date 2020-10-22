using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgUpgradeSpReq
    {
    }


    [Serializable]
    public class MsgUpgradeSpAck
    {
        public short ErrorCode;

        // 인게임 업그레이드 수치
        public short Upgrade;

        // 현재 Sp
        public int CurrentSp;
    }


    [Serializable]
    public class MsgUpgradeSpNotify
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 인게임 업그레이드 수치
        public short Upgrade;
    }
}
