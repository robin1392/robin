using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgPlayerBase
    {
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
    }


    [Serializable]
    public class MsgPlayerInfo
    {
        // �÷��̾� ����ũ ���̵�(���� ���Ǻ� ����ũ ����)
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceUpgradeArray;
    }


    [Serializable]
    public class MsgJoinGameReq
    {
        public string PlayerSessionId;

        // ��� �� �ε���
        public sbyte DeckIndex;
    }


    [Serializable]
    public class MsgJoinGameAck
    {
        // ���� �ڵ�
        public short ErrorCode;

        // �÷��̾� ����.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinGameNotify
    {
        // �ٸ� �÷��̾� ����
        public MsgPlayerInfo OtherPlayerInfo;
    }
}