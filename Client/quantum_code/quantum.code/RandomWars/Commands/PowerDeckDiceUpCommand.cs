using Photon.Deterministic;

namespace Quantum.Commands
{
    public class PowerDeckDiceUpCommand : DeterministicCommand
    {
        public int DeckIndex;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref DeckIndex);
        }
    }
}