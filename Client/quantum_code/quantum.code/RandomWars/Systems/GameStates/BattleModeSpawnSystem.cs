using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class BattleModeSpawnSystem : SystemSignalsOnly, ISignalSpawnByWave
    {
        public void SpawnByWave(Frame f, int wave)
        {
            Log.Debug($"BattleModeSpawnSystem Spawn {wave}");
        }
    }
}