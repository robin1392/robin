using System;

namespace Quantum
{
    partial struct FieldDice
    {
        private const Int32 EmptyIndex = -1;

        public void SetEmpty()
        {
            DeckIndex = EmptyIndex;
            DiceScale = 0;
        }

        public bool IsEmpty => DeckIndex == EmptyIndex;
    }
}