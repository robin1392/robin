using System;
using System.Collections.Generic;

namespace MirageTest.Scripts
{
    //TODO : 많아지면 데이터로 뺀다
    public static class BuffInfos
    {
        public const byte NinjaClocking = 0;

        public static BuffState[] Data = new[]
        {
            BuffState.Clocking,
        };
    }

    [Flags]
    public enum BuffState
    {
        None = 0,
        Clocking = 1 << 1,
    }
}