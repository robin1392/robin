using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsProtocol
{
    public enum ETeamKind
    {
        Red,
        Blue
    }


    public enum EGameResult
    {
        None,
        // 승리
        Victory,
        // 부전승
        VictoryByDefault,
        // 패배
        Defeat,
        // 무승부
        Draw
    }


    public enum ERewardType : byte
    {
        None,
        Trophy,
        Gold,
        Diamond,
        Key,
        Box,
    }


    public enum EBoxType
    {
        None,
        TeamMatch,
        Boss,
    }
}
