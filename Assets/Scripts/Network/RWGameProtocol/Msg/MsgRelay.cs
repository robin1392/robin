using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct MsgVector3
    {
        public int X;
        public int Y;
        public int Z;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct MsgQuaternion
    {
        public int X;
        public int Y;
        public int Z;
        public int W;
    }



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


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSturnMinionRelay : Serializer<MsgSturnMinionRelay>
    {
        public int PlayerUId;
        public int Id;
        public int SturnTime;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgRocketBombRelay : Serializer<MsgRocketBombRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgIceBombRelay : Serializer<MsgIceBombRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgDestroyMagicRelay : Serializer<MsgDestroyMagicRelay>
    {
        public int PlayerUId;
        public int BaseStatId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireCannonBallRelay : Serializer<MsgFireCannonBallRelay>
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public MsgVector3 TargetPos;
        public int Power;
        public int Range;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireSpearRelay : Serializer<MsgFireSpearRelay>
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int MoveSpeed;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgFireManFireRelay : Serializer<MsgFireManFireRelay>
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgActivatePoolObjectRelay : Serializer<MsgActivatePoolObjectRelay>
    {
        public int PlayerUId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string PoolName;
        public MsgVector3 HitPos;
        public MsgQuaternion Rotation;
        public MsgVector3 LocalScale;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgMinionCloackingRelay : Serializer<MsgMinionCloackingRelay>
    {
        public int PlayerUId;
        public int Id;
        public bool IsCloacking;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgMinionFogOfWarRelay : Serializer<MsgMinionFogOfWarRelay>
    {
        public int PlayerUId;
        public int BaseStatId;
        public int Effect;
        public bool IsFogOfWar;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSendMessageVoidRelay : Serializer<MsgSendMessageVoidRelay>
    {
        public int PlayerUId;
        public int Id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Message;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSendMessageParam1Relay : Serializer<MsgSendMessageParam1Relay>
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Message;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgNecromancerBulletRelay : Serializer<MsgNecromancerBulletRelay>
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int BulletMoveSpeed;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetMinionTargetRelay : Serializer<MsgSetMinionTargetRelay>
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgScarecrowRelay : Serializer<MsgScarecrowRelay>
    {
        public int PlayerUId;
        public int BaseStatId;
        public int EyeLevel;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLazerTargetRelay : Serializer<MsgLazerTargetRelay>
    {
        public int PlayerUId;
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public int[] TargetIdArray = new int[30];
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgMinionStatusRelay : Serializer<MsgMinionStatusRelay>
    {
        public int PlayerUId;

        // Pos배열 분할 인덱스
        public byte PosIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public MsgVector3[] Pos = new MsgVector3[30];
    }


}
