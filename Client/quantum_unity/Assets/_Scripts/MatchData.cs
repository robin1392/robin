using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
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
        public string UserId;
        public int Trophy;
        public int WinStreak;
        public DeckInfo Deck;
        public bool EnableAI;

        private static readonly string NickNamePropsKey = "NICK";
        private static readonly string UserIdPropsKey = "ID";
        private static readonly string TrophyPropsKey = "TROPHY";
        private static readonly string WinStreakPropsKey = "WINSTREAK";
        private static readonly string GuardianKey = "GUARDIAN";
        private static readonly string DiceIds = "DICEIDS";
        private static readonly string DiceLevels = "DICELVS";
        public Hashtable ToPlayerCustomProperty()
        {
            var ht = new Hashtable();
            ht.Add(NickNamePropsKey, NickName);
            ht.Add(UserIdPropsKey, UserId);
            ht.Add(TrophyPropsKey, Trophy);
            ht.Add(WinStreakPropsKey, WinStreak);
            ht.Add(GuardianKey, Deck.GuardianId);
            ht.Add(DiceIds, Deck.DiceInfos.Select(d =>d.DiceId).ToArray());
            ht.Add(DiceLevels, Deck.DiceInfos.Select(d =>d.OutGameLevel).ToArray());
            return ht;
        }
        
        public static MatchPlayer CreateFromPlayerCustomProperty(Hashtable ht)
        {
            var p = new MatchPlayer();
            p.NickName = ht[NickNamePropsKey] as string; 
            p.UserId = ht[UserIdPropsKey] as string;
            p.Trophy = (int)ht[TrophyPropsKey];
            p.WinStreak = (int)ht[WinStreakPropsKey];
            p.Deck = new DeckInfo((int)ht[GuardianKey], (int[])ht[DiceIds], (int[])ht[DiceLevels]);
            return p;
        }
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