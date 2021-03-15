using System;
using System.Collections.Generic;

namespace MirageTest.Scripts
{
    //TODO : 많아지면 데이터로 뺀다
    public static class BuffInfos
    {
        public const byte NinjaClocking = 0;
        public const byte HalfDamage = 1;
        public const byte Sturn = 2;

        public static BuffState[] Data = new[]
        {
            BuffState.Clocking,
            BuffState.HalfDamage,
            BuffState.Sturn,
        };
    }

    [Flags]
    public enum BuffState
    {
        None = 0,
        Clocking = 1 << 1,
        HalfDamage = 1 << 2,
        Sturn = 1 << 3,
        Freeze = 1 << 4,

        CantAI = Sturn | Freeze,
    }
}