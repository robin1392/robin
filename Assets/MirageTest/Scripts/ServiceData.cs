using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Core;

namespace MirageTest.Aws
{
    public class MatchPlayerAttribute
    {
        public string PlayerId;
        public Dictionary<string, double> dictAttributeNumber = null;
        public Dictionary<string, string> dictAttributeString = null;
        public Dictionary<string, List<string>> dictAttributeList = null;
    }

    public class MatchDiceInfo
    {
        public int DiceId;
        public byte Level;
        public int Count;
    }

    public class UserMatchResult
    {
        public string UserId;
        public int MatchResult;
        public List<ItemBaseInfo> listReward;
        public AdRewardInfo LoseReward;
    }

    public class QuestCompleteParam
    {
        public int QuestCompleteType { get; set; }
        public int Value { get; set; }
    }
}
