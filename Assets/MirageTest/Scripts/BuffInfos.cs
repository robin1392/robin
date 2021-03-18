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
        public const byte Freeze = 3;
        public const byte Invincibility = 4;
        public const byte Scarecrow = 5;

        public static BuffState[] Data = new[]
        {
            BuffState.Clocking,
            BuffState.HalfDamage,
            BuffState.Sturn,
            BuffState.Freeze,
            BuffState.Invincibility,
            BuffState.Scarecrow,
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
        Invincibility = 1 << 5,
        Scarecrow = 1 << 6,

        CantAI = Sturn | Freeze | Scarecrow,
    }
}