using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgRemoveMinionRelay : Serializer<MsgRemoveMinionRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHitDamageMinionRelay : Serializer<MsgHitDamageMinionRelay>
    {
        public int PlayerUId;
        public int Id;
        public int Damage;
        public int Delay;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgDestroyMinionRelay : Serializer<MsgDestroyMinionRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHealMinionRelay : Serializer<MsgHealMinionRelay>
    {
        public int PlayerUId;
        public int Id;
        public int Heal;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgPushMinionRelay : Serializer<MsgPushMinionRelay>
    {
        public int PlayerUId;
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] Dir = new int[3];

        public int PushPower;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMinionAnimationTriggerRelay : Serializer<MsgSetMinionAnimationTriggerRelay>
    {
        public int PlayerUId;
        public int Id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Trigger;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgRemoveMagicRelay : Serializer<MsgRemoveMagicRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireArrowRelay : Serializer<MsgFireArrowRelay>
    {
        public int PlayerUId;
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] Dir = new int[3];

        public int Damage;
        public int MoveSpeed;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireballBombRelay : Serializer<MsgFireballBombRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgMineBombRelay : Serializer<MsgMineBombRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMagicTargetIdRelay : Serializer<MsgSetMagicTargetIdRelay>
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMagicTargetRelay : Serializer<MsgSetMagicTargetRelay>
    {
        public int PlayerUId;
        public int Id;
        public int X;
        public int Z;
    }
}
