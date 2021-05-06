using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class BattleModeSpawnSystem : SystemSignalsOnly, ISignalSpawnByWave, ISignalOnPlayerDataSet
    {
        
        
        public void SpawnByWave(Frame f, int wave)
        {
            Log.Debug($"BattleModeSpawnSystem Spawn {wave}");
        }

        public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
        {
            var rwPlayer = f.Global->Players.GetPointer(playerRef);
            if (playerRef == 0)
            {
                rwPlayer->Team = GameConstants.BottomCamp;
                CreateTower(f, rwPlayer, f.Context.FieldPositions.GetBottomPlayerPosition());
            }
            else
            {
                rwPlayer->Team = GameConstants.TopCamp;
                CreateTower(f, rwPlayer, f.Context.FieldPositions.GetTopPlayerPosition());
            }
        }

        void CreateTower(Frame f, RWPlayer* rwPlayer, FPVector2 position)
        {
            var entityRef = f.Create();
            f.Add<ActorCreation>(entityRef);
            var actorCreation = f.Unsafe.GetPointer<ActorCreation>(entityRef);
            var list = f.ResolveList(actorCreation->creationList);

            list.Add(new ActorCreationSpec()
            {
                Owner = rwPlayer->PlayerRef,
                ActorType =  ActorType.Tower,
                Position = position,
                Team = rwPlayer->Team
            });
        }
    }
}