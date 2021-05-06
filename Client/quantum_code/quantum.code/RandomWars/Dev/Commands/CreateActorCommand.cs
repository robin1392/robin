using System;
using Photon.Deterministic;

namespace Quantum.Commands
{
    public class CreateActorCommand : DeterministicCommand
    {
        public ActorType ActorType;
        public Int32 DataId;
        public Int32 IngameLevel;
        public Int32 OutgameLevel;
        public Int32 DiceScale;
        public Int32 FieldIndex;
        public FPVector2 Position;
        
        public override void Serialize(BitStream stream)
        {
            if (stream.Writing)
            {
                var actorType = (int) ActorType;
                stream.Serialize(ref actorType);
            }
            else
            {
                var actorType = default(int);
                stream.Serialize(ref actorType);
                ActorType = (ActorType)actorType;
            }
            
            stream.Serialize(ref DataId);
            stream.Serialize(ref IngameLevel);
            stream.Serialize(ref OutgameLevel);
            stream.Serialize(ref DiceScale);
            stream.Serialize(ref FieldIndex);
            stream.Serialize(ref Position);
        }
    }
}