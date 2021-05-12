using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class CoopModeSpawnSystem : SystemSignalsOnly, ISignalOnWave
    {
        public void OnWave(Frame f, int wave)
        {
            Log.Debug($"CoopModeSpawnSystem Spawn {wave}");
        }
    }
}