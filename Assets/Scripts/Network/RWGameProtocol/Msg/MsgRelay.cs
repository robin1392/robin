using System;
using System.Collections.Generic;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public struct MsgVector3
    {
        public int X;
        public int Y;
        public int Z;
    }

    [Serializable]
    public struct MsgQuaternion
    {
        public int X;
        public int Y;
        public int Z;
        public int W;
    }



    [Serializable]
    public class MsgRemoveMinionRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgHitDamageMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public int Damage;
    }


    [Serializable]
    public class MsgDestroyMinionRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgHealMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public int Heal;
    }


    [Serializable]
    public class MsgPushMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public int[] Dir = new int[3];
        public int PushPower;
    }


    [Serializable]
    public class MsgSetMinionAnimationTriggerRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
        public int Trigger;
    }


    [Serializable]
    public class MsgRemoveMagicRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    
    public class MsgFireArrowRelay
    {
        public int PlayerUId;
        public int Id;
        public int[] Dir = new int[3];
        public int Damage;
        public int MoveSpeed;
    }


    [Serializable]
    public class MsgFireballBombRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgMineBombRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgSetMagicTargetIdRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
    }


    [Serializable]
    public class MsgSetMagicTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int X;
        public int Z;
    }


    [Serializable]
    public class MsgSturnMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public int SturnTime;
    }


    [Serializable]
    public class MsgRocketBombRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgIceBombRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgDestroyMagicRelay
    {
        public int PlayerUId;
        public int BaseStatId;
    }


    [Serializable]
    public class MsgFireCannonBallRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public MsgVector3 TargetPos;
        public int Power;
        public int Range;
        public int Type;
    }


    [Serializable]
    public class MsgFireSpearRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int MoveSpeed;
    }


    [Serializable]
    public class MsgFireManFireRelay
    {
        public int PlayerUId;
        public int Id;
    }


    [Serializable]
    public class MsgActivatePoolObjectRelay
    {
        public int PlayerUId;
        public int PoolName;
        public MsgVector3 HitPos;
        public MsgQuaternion Rotation;
        public MsgVector3 LocalScale;
    }


    [Serializable]
    public class MsgMinionCloackingRelay
    {
        public int PlayerUId;
        public int Id;
        public bool IsCloacking;
    }


    [Serializable]
    public class MsgMinionFogOfWarRelay
    {
        public int PlayerUId;
        public int BaseStatId;
        public int Effect;
        public bool IsFogOfWar;
    }


    [Serializable]
    public class MsgSendMessageVoidRelay
    {
        public int PlayerUId;
        public int Id;
        public int Message;
    }


    [Serializable]
    public class MsgSendMessageParam1Relay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
        public int Message;
    }


    [Serializable]
    public class MsgNecromancerBulletRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int BulletMoveSpeed;
    }


    [Serializable]
    public class MsgSetMinionTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
    }


    [Serializable]
    public class MsgScarecrowRelay
    {
        public int PlayerUId;
        public int BaseStatId;
        public int EyeLevel;
    }


    [Serializable]
    public class MsgLazerTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int[] TargetIdArray;
    }


    [Serializable]
    public class MsgMinionStatusRelay
    {
        public int PlayerUId;
        public byte PosIndex;
        public MsgVector3[] Pos;
        public Dictionary<GameProtocol, List<object>> Relay;
    }


    [Serializable]
    public class MsgFireBulletRelay
    {
        public int PlayerUId;
        public int Id;
        public int[] Dir = new int[3];
        public int Damage;
        public int MoveSpeed;
        public int Type;
    }


    [Serializable]
    public class MsgMinionInvincibilityRelay
    {
        public int PlayerUId;
        public int Id;
        public int Time;
    }
}
