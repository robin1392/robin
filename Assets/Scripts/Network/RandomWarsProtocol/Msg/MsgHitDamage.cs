using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgHitDamageReq
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 미니언 아이디
        public ushort Id;

        // 데미지량. (실제 데미지량은 0.01을 곱해서 사용해야 함)
        public int Damage;
    }


    [Serializable]
    public class MsgHitDamageAck
    {
        public int ErrorCode;

        // 플레이어 UID
        public ushort PlayerUId;

        // 미니언 아이디
        public ushort Id;

        // 현재 Hp
        public int CurrentHp;
    }


    [Serializable]
    public class MsgHitDamageNotify
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 미니언 아이디
        public ushort Id;

        // 현재 Hp
        public int CurrentHp;
    }
}
