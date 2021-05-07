using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class BattleModeSystem : SystemSignalsOnly, ISignalSpawnByWave, ISignalOnPlayerDataSet
    {
        public override void OnInit(Frame f)
        {
            f.Context.SetNavmesh(f.FindAsset<NavMesh>("Resources/DB/BattleMap_Navmesh"));
        }

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
            f.Add<ActorCreationSpec>(entityRef);
            var actorCreation = f.Unsafe.GetPointer<ActorCreationSpec>(entityRef);
            actorCreation->Owner = rwPlayer->PlayerRef;
            actorCreation->ActorType = ActorType.Tower;
            actorCreation->Position = position;
            actorCreation->Team = rwPlayer->Team;
        }
    }
}