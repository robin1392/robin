using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class UpdateWaveSystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            f.Global->Wave = 0;
            f.Global->WaveRemainTime = 0;
            f.Global->SpWave = 4;
            var tableData = f.Context.TableData; 
            f.Global->WaveTime = tableData.VsMode.KeyValues[(int)EVsmodeKey.WaveTime].value;
        }

        public override void Update(Frame f)
        {
            if (f.Global->State != StateType.Play)
            {
                return;
            }
            
            f.Global->WaveRemainTime -= f.DeltaTime;
            
            var spWaveUnit = f.Global->WaveTime / FP._4;
            var spWave = FPMath.CeilToInt(f.Global->WaveRemainTime / spWaveUnit);
            if (f.Global->SpWave != spWave)
            {
                f.Global->SpWave = spWave;
                f.Signals.OnSpWave(f.Global->Wave, f.Global->SpWave);
            }

            if (f.Global->WaveRemainTime < FP._0)
            {
                f.Global->SpWave = 4;
                f.Global->Wave += 1;
                f.Signals.OnWave(f.Global->Wave);
            }
        }
    }
}