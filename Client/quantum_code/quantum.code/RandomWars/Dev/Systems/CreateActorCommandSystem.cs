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
            frame.Add<ActorCreation>(entityRef);
            var actorCreation = frame.Unsafe.GetPointer<ActorCreation>(entityRef);
            var list = frame.ResolveList(actorCreation->creationList);

            var position = command.Position;
            if (command.ActorType == ActorType.Dice)
            {
                position = frame.Context.FieldPositions.GetPosition(player->Team, command.FieldIndex);
            }

            Log.Debug($"{command.FieldIndex}, {command.Position}");

            list.Add(new ActorCreationSpec()
            {
                Owner = player->PlayerRef,
                ActorType =  command.ActorType,
                DataId =  command.DataId,
                IngameLevel =  command.IngameLevel,
                OutgameLevel = command.OutgameLevel,
                DiceScale = command.DiceScale,
                Position = position,
                FieldIndex = command.FieldIndex,
            });
        }
    }
}