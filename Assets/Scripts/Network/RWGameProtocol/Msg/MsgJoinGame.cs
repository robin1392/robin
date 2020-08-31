using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class PlayerInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] DeckInfo;

        public bool IsBottomPlayer;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinGameReq : Serializer<MsgJoinGameReq>
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PlayerSessionId;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinGameAck : Serializer<MsgJoinGameAck>
    {
        // 에러 코드
        public short ErrorCode;

        // 상대 플레이어 정보.
        public PlayerInfo OtherPlayerInfo;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgJoinGameNotify : Serializer<MsgJoinGameNotify>
    {
        public PlayerInfo JoinPlayerInfo;
    }
}