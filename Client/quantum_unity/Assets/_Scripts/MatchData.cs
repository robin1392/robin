using System.Collections.Generic;
using System.Linq;
using Mirage.Logging;
using Photon.Realtime;
using UnityEngine;
using LogFactory = _Scripts.Logging.LogFactory;

namespace _Scripts
{
    public class MatchData
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerProxy));

        public MatchData(MatchPlayer player1, MatchPlayer player2)
        {
            PlayerInfos = new List<MatchPlayer>()
            {
                player1,
                player2,
            };
        }

        public List<MatchPlayer> PlayerInfos { get; }
    }

    public class MatchPlayer
    {
        public string NickName;
        public int Trophy;
        public int WinStreak;
        public DeckInfo Deck;
        public bool EnableAI;
    }

    public class DeckInfo
    {
        public int GuardianId;
        public DiceInfo[] DiceInfos;
        
        public DeckInfo(int guardianId, int[] diceIds, int[] diceOutGameLevels)
        {
            GuardianId = guardianId;
            
            DiceInfos = new DiceInfo[5];
            for (var i = 0; i < diceIds.Length; ++i)
            {
                DiceInfos[i].DiceId = diceIds[i];
                DiceInfos[i].OutGameLevel = diceOutGameLevels[i];
            }
        }
    }

    public struct DiceInfo
    {
        public int DiceId;
        public int OutGameLevel;

        public override string ToString()
        {
            return $"id:{DiceId}/lev:{OutGameLevel}";
        }
    }
}