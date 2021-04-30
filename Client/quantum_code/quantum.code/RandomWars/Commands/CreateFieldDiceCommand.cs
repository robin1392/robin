using System;
using Photon.Deterministic;

namespace Quantum.Commands
{
    public class CreateFieldDiceCommand : DeterministicCommand
    {
        public Int32 DeckIndex;
        public Int32 FieldIndex;
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref DeckIndex);
            stream.Serialize(ref FieldIndex);
        }
    }
}