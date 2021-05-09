using System;

namespace Quantum
{
    [Flags]
    public enum BuffType : Int32
    {
        None = 0,
        Freeze = 1 << 1,
        Taunted = 1 << 2,
        Knockbacked = 1 << 3,

        CantAct = Freeze,
    }
}