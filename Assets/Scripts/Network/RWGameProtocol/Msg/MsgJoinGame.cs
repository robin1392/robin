using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    /// <summary>
    /// �� �ֻ��� ����
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgDeckDiceInfo
    {
        public int Id;
        public short Upgrade;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public struct MsgPlayerInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public MsgDeckDiceInfo[] DiceInfoArray;

        public bool IsBottomPlayer;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinGameReq : Serializer<MsgJoinGameReq>
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PlayerSessionId;

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
        public MsgPlayerInfo OtherPlayerInfo;
    }
}