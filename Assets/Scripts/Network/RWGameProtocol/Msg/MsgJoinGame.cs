using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public struct MsgPlayerInfo
    {
        // 플레이어 유니크 아이디(게임 세션별 유니크 생성)
        public int PlayerUId;

        // 최초 입장 플레이어가 bottom
        public bool IsBottomPlayer;

        // 플레이어명
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        // 주사위 아이디 배열
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] DiceIdArray;

        // 주사위 업그레이드 수치 배열
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