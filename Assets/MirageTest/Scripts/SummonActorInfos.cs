using System.Collections.Generic;
using ED;
using ED.SummonActor;

namespace MirageTest.Scripts
{
    public static class SummonActorInfos
    {
        public const int SinzedPoison = 0;
        private static Dictionary<int, SummonActorInfo> dic = new Dictionary<int, SummonActorInfo>()
        {
            {0, new SummonActorInfo()
            {
                id = 0,
                prefab = "Effect_Poison",
                targetMoveType = DICE_MOVE_TYPE.ALL,
            }},
        };

        public static SummonActorInfo GetSummonActorInfo(int id)
        {
            return dic[id];
        }

        public class SummonActorInfo
        {
            public int id;
            public string prefab;
            public DICE_MOVE_TYPE targetMoveType;
        }
    }
}