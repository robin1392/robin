using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum
{
    public unsafe class CreateActorCommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                if (f.GetPlayerCommand(playerID) is CreateActorCommand command)
                {
                    var player = f.Global->Players.GetPointer(playerID);
                    CreateActor(f, command, player);
                }
            }
        }

        private void CreateActor(Frame frame, CreateActorCommand command, RWPlayer* player)
        {
            var entityRef = frame.Create();
            frame.Add<ActorCreationSpec>(entityRef);
            frame.Add<ActorCreation>(entityRef);
            var actorCreation = frame.Unsafe.GetPointer<ActorCreationSpec>(entityRef);

            var position = command.Position;
            if (command.ActorType == ActorType.Dice)
            {
                position = frame.Context.FieldPositions.GetPosition(player->Team, command.FieldIndex);
            }

            actorCreation->Owner = player->PlayerRef;
            actorCreation->ActorType = command.ActorType;
            actorCreation->DataId = command.DataId;
            actorCreation->IngameLevel = command.IngameLevel;
            actorCreation->OutgameLevel = command.OutgameLevel;
            actorCreation->DiceScale = command.DiceScale;
            actorCreation->Position = position;
            actorCreation->FieldIndex = command.FieldIndex;
            actorCreation->Team = player->Team;
        }
    }
}