using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgRemoveMinionRelay : Serializer<MsgRemoveMinionRelay>
    {
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageMinionRelay : Serializer<MsgHitDamageMinionRelay>
    {
        public int Id;
        public float Damage;
        public float Delay;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgDestroyMinionRelay : Serializer<MsgDestroyMinionRelay>
    {
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHealMinionRelay : Serializer<MsgHealMinionRelay>
    {
        public int Id;
        public float Heal;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgPushMinionRelay : Serializer<MsgPushMinionRelay>
    {
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Dir = new float[3];

        public float PushPower;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMinionAnimationTriggerRelay : Serializer<MsgSetMinionAnimationTriggerRelay>
    {
        public int Id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Trigger;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgRemoveMagicRelay : Serializer<MsgRemoveMagicRelay>
    {
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireArrowRelay : Serializer<MsgFireArrowRelay>
    {
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Dir = new float[3];

        public float Damage;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireballBombRelay : Serializer<MsgFireballBombRelay>
    {
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgMineBombRelay : Serializer<MsgMineBombRelay>
    {
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMagicTargetIdRelay : Serializer<MsgSetMagicTargetIdRelay>
    {
        public int Id;
        public int TargetId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMagicTargetRelay : Serializer<MsgSetMagicTargetRelay>
    {
        public int Id;
        public float X;
        public float Z;
    }
}
