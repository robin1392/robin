using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgHitDamageReq
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지량. (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;
    }


    [Serializable]
    public class MsgHitDamageAck
    {
        public short ErrorCode;

        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지량 (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;

        // 현재 타워 Hp
        public int CurrentHp;
    }


    [Serializable]
    public class MsgHitDamageNotify
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지량 (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;

        // 현재 타워 Hp
        public int CurrentHp;
    }
}
