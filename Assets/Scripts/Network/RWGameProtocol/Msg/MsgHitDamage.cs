using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageReq : Serializer<MsgHitDamageReq>
    {
        public float Damage;
        public float Delay;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageAck : Serializer<MsgHitDamageAck>
    {
        public short ErrorCode;
        public float Damage;
        public float Delay;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageNotify : Serializer<MsgHitDamageNotify>
    {
        public float Damage;
        public float Delay;
    }
}
