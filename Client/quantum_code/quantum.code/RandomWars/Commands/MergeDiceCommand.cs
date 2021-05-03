using Photon.Deterministic;

namespace Quantum.Commands
{
    public class MergeDiceCommand : DeterministicCommand
    {
        public int SourceFieldIndex;
        public int TargetFieldIndex;
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref SourceFieldIndex);
            stream.Serialize(ref TargetFieldIndex);
        }
    }
}