using System;
using System.Collections.Generic;

namespace MirageTest.Scripts
{
    //TODO : 많아지면 데이터로 뺀다
    public static class BuffInfos
    {
        public const byte NinjaClocking = 0;
        public const byte HalfDamage = 1;
        public const byte Stun = 2;
        public const byte Freeze = 3;
        public const byte Invincibility = 4;
        public const byte Scarecrow = 5;
        public const byte IceFreeze = 6;
        public const byte Taunted = 7;
        public const byte Knockbacked = 8;

        public static BuffType[] Data = new[]
        {
            BuffType.Clocking,
            BuffType.HalfDamage,
            BuffType.Stun,
            BuffType.Freeze,
            BuffType.Invincibility,
            BuffType.Scarecrow,
            BuffType.Freeze,
            BuffType.Taunted,
            BuffType.Knockbacked
        };
    }

    [Flags]
    public enum BuffType
    {
        None = 0,
        Clocking = 1 << 1,
        HalfDamage = 1 << 2,
        Stun = 1 << 3,
        Freeze = 1 << 4,
        Invincibility = 1 << 5,
        Scarecrow = 1 << 6,
        Taunted = 1 << 7,
        Knockbacked = 1 << 8,

        CantAI = Stun | Freeze | Scarecrow | Knockbacked,
    }
}