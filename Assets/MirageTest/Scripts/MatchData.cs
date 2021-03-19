using System.Collections.Generic;
using System.Linq;
using Mirage;
using Mirage.Logging;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class MatchData
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerProxy));
        
        public List<MatchPlayer> PlayerInfos { get; private set; } = new List<MatchPlayer>();

        public string GameMode { get; private set; } = string.Empty;

        public void AddPlayerInfo(string userId, string userNickName, int trophy, DeckInfo deck)
        {
            if(PlayerInfos.Find(p => p.UserId == userId) != null)
            {
                logger.LogError($"이미 추가된 플레이어 입니다.");
                return;
            }
            
            PlayerInfos.Add(new MatchPlayer()
            {
                UserId = userId,
                UserNickName = userNickName,
                Trophy = trophy,
                Deck = deck,
            });
        }

        public void SetGameMode(string gameMode)
        {
            GameMode = gameMode;
        }
    }

    public class MatchPlayer
    {
        public string UserId;
        public string UserNickName;
        public int Trophy;
        public DeckInfo Deck;
        public string PlayerSessionId;
    }

    public class DeckInfo
    {
        public int GuadianId;
        public DiceInfo[] DiceInfos;

        public DeckInfo()
        {
        }

        public DeckInfo(int guadianId, params int[] diceIds)
        {
            GuadianId = guadianId;
            DiceInfos = diceIds.Select(id => new DiceInfo()
            {
                DiceId = id,
                OutGameLevel = 0,
            }).ToArray();
        }
    }

    public struct DiceInfo
    {
        public int DiceId;
        public byte OutGameLevel;
    }
}