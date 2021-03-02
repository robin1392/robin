using System;
using System.Collections.Generic;
using System.IO;

namespace Template.Season.RandomwarsSeason.Common
{
    [Serializable]
    public class MsgRankInfo
    {
        public int Ranking;
        public string Name;
        public short Class;
        public int Trophy;
        public int[] DeckInfo;
    }

}