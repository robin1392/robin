using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class SetWaveTimeSystem : SystemSignalsOnly, ISignalOnWave
    {
        public void OnWave(Frame f, int wave)
        {
            f.Global->WaveRemainTime = f.Global->WaveTime;
            f.Events.OnWaveChanged();
        }
    }
}