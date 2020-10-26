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