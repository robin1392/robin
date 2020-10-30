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
        None = 0,
        Trophy,
        Gold,
        Diamond,
        Key,
        Box,
        DiceNormal,
        DiceMagic,
        DiceEpic,
        DiceLegend,
    }


    public enum EBoxType
    {
        None,
        CooperationBox,
        BossBox,
    }


    public enum EGlobalDataType
    {
        None,
        startCoolTime,
        waveTime,
    }
}
