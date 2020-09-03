using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public struct MsgPlayerInfo
    {
        public int PlayerUId;
        public bool IsBottomPlayer;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] DiceIdArray;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public short[] DiceUpgradeArray;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinGameReq : Serializer<MsgJoinGameReq>
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PlayerSessionId;

        // 사용 덱 인덱스
        public sbyte DeckIndex;
    }


    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public class MsgJoinGameAck : Serializer<MsgJoinGameAck>
    {
        // 에러 코드
        public short ErrorCode;

        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public class MsgJoinGameNotify : Serializer<MsgJoinGameNotify>
    {
        // 다른 플레이어 정보
        public MsgPlayerInfo OtherPlayerInfo;
    }
}