using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public struct MsgPlayerInfo
    {
        // �÷��̾� ����ũ ���̵�(���� ���Ǻ� ����ũ ����)
        public int PlayerUId;

        // ���� ���� �÷��̾ bottom
        public bool IsBottomPlayer;

        // �÷��̾��
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        // �ֻ��� ���̵� �迭
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] DiceIdArray;

        // �ֻ��� ���׷��̵� ��ġ �迭
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public short[] DiceUpgradeArray;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinGameReq : Serializer<MsgJoinGameReq>
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PlayerSessionId;

        // ��� �� �ε���
        public sbyte DeckIndex;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public class MsgJoinGameAck : Serializer<MsgJoinGameAck>
    {
        // ���� �ڵ�
        public short ErrorCode;

        // �÷��̾� ����.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public class MsgJoinGameNotify : Serializer<MsgJoinGameNotify>
    {
        // �ٸ� �÷��̾� ����
        public MsgPlayerInfo OtherPlayerInfo;
    }
}