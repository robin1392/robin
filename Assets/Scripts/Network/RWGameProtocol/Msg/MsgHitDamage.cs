using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageReq : Serializer<MsgHitDamageReq>
    {
        // 데미지량. (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageAck : Serializer<MsgHitDamageAck>
    {
        public short ErrorCode;

        // 데미지량 (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageNotify : Serializer<MsgHitDamageNotify>
    {
        // 플레이어 UID
        public int PlayerUId;

        // 데미지량 (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;
    }
}
