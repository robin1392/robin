using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class AddSpBySpWaveSystem : SystemSignalsOnly, ISignalOnSpWave
    {
        public void OnSpWave(Frame f, int wave, int spWave)
        {
            for (var i = 0; i < f.Global->Players.Length; ++i)
            {
                var player = f.Global->Players[i];
                if (player.PlayerRef == PlayerRef.None)
                {
                    continue;
                }
                
                var sp = f.Unsafe.GetPointer<Sp>(player.EntityRef);
                sp->CurrentSp += sp->CommingSp;
                f.Events.SpIncreased(player.PlayerRef);
            }
        }
    }
}