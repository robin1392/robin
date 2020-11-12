using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgHitDamageReq
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지 정보
        public MsgDamage[] Damage;
    }


    [Serializable]
    public class MsgHitDamageAck
    {
        public int ErrorCode;

        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지 적용 결과
        public MsgDamageResult[] DamageResult;
    }


    [Serializable]
    public class MsgHitDamageNotify
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 데미지 적용 결과
        public MsgDamageResult[] DamageResult;
    }
}
