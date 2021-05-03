using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class UpdateWaveSystem : SystemMainThread
    {
        private int _waveTime;
        
        public override void OnInit(Frame f)
        {
            f.Global->Wave = 0;
            f.Global->WaveRemainTime = 0; 
            _waveTime = f.Context.TableData.VsMode.KeyValues[(int)EVsmodeKey.WaveTime].value;
        }

        public override void Update(Frame f)
        {
            if (f.Global->State != StateType.Play)
            {
                return;
            }

            f.Global->WaveRemainTime -= f.DeltaTime;
            if (f.Global->WaveRemainTime < FP._0)
            {
                f.Global->Wave += 1;
                f.Global->WaveRemainTime = _waveTime;
                f.Signals.SpawnByWave(f.Global->Wave);
            }
        }
    }
}