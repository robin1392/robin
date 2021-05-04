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
            }
            else
            {
                rwPlayer->Team = GameConstants.TopCamp;
            }
        }
    }
}