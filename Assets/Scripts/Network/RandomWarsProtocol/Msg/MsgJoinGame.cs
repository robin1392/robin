using System;
using System.IO;

namespace RandomWarsProtocol.Msg
{

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
        public int ErrorCode;

        // �÷��̾� ����.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinGameNotify
    {
        // �ٸ� �÷��̾� ����
        public MsgPlayerInfo OtherPlayerInfo;
    }

    [Serializable]
    public class MsgJoinCoopGameReq
    {
        public string PlayerSessionId;

        // ��� �� �ε���
        public sbyte DeckIndex;
    }


    [Serializable]
    public class MsgJoinCoopGameAck
    {
        // ���� �ڵ�
        public int ErrorCode;

        // �÷��̾� ����.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinCoopGameNotify
    {
        // �Ʊ�(����) �÷��̾� ����
        public MsgPlayerInfo CoopPlayerInfo;

        // ���� �÷��̾� ����
        public MsgPlayerInfo[] OtherPlayerInfo;
    }
}