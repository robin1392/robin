using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class CoopModeSpawnSystem : SystemSignalsOnly, ISignalSpawnByWave
    {
        public void SpawnByWave(Frame f, int wave)
        {
            Log.Debug($"CoopModeSpawnSystem Spawn {wave}");
        }
    }
}