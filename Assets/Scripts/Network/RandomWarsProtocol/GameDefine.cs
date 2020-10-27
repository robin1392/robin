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
        Victory,
        Defeat,
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
