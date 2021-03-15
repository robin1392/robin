using System.Collections.Generic;

namespace MirageTest.Scripts
{
    public static class SummonActorInfos
    {
        public const int SinzedPoison = 0;
        private static Dictionary<byte, string> dic = new Dictionary<byte, string>()
        {
            {0, "Effect_Poison"},
        };

        public static string GetSummonActorPrefab(byte id)
        {
            return dic[id];
        }
    }
}