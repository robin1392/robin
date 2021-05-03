
using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class SuddenDeathSystem : SystemSignalsOnly, ISignalOnWave
    {
        private int _battleModeSuddenDeathWave;
        private int _battleModeSuddenDeathSecondWave = 16;
        private int _WaveTime;
        private int _battleModeSuddenDeathWaveTime;
        private int _battleModeSuddenDeathSecondWaveTime;
        private FP _suddenDeathAttackSpeedFactor;
        private FP _suddenDeathMoveSpeedFactor;
        
        public override void OnInit(Frame f)
        {
            var tableData = f.Context.TableData;
            _WaveTime = tableData.VsMode.KeyValues[(int)EVsmodeKey.WaveTime].value;
            _battleModeSuddenDeathWave = tableData.VsMode.KeyValues[(int) EVsmodeKey.StartSuddenDeathWave].value;
            _battleModeSuddenDeathWaveTime= tableData.VsMode.KeyValues[(int) EVsmodeKey.SuddenDeathWaveTime1].value;
            _battleModeSuddenDeathSecondWaveTime= tableData.VsMode.KeyValues[(int) EVsmodeKey.SuddenDeathWaveTime2].value;
            _suddenDeathAttackSpeedFactor = tableData.VsMode.KeyValues[(int) EVsmodeKey.SuddenDeathAtkSpeed].value / FP._100;
            _suddenDeathMoveSpeedFactor = tableData.VsMode.KeyValues[(int) EVsmodeKey.SuddenDeathMoveSpeed].value / FP._100;
        }

        public void OnWave(Frame f, int wave)
        {
            if (wave >= _battleModeSuddenDeathSecondWave)
            {
                f.Global->WaveTime = _battleModeSuddenDeathSecondWaveTime;
                f.Global->SuddenDeathAttackSpeedFactor = _suddenDeathAttackSpeedFactor;
                f.Global->SuddenDeathMoveSpeedFactor = _suddenDeathMoveSpeedFactor;
                f.Global->IsSuddenDeath = true;
                return;
            }
            
            if (wave >= _battleModeSuddenDeathWave)
            {
                f.Global->WaveTime = _battleModeSuddenDeathWaveTime;
                f.Global->SuddenDeathAttackSpeedFactor = _suddenDeathAttackSpeedFactor;
                f.Global->SuddenDeathMoveSpeedFactor = _suddenDeathMoveSpeedFactor;
                f.Global->IsSuddenDeath = true;
                return;
            }

            f.Global->IsSuddenDeath = false;
            f.Global->WaveTime = _WaveTime;
            f.Global->SuddenDeathAttackSpeedFactor = FP._1;
            f.Global->SuddenDeathMoveSpeedFactor = FP._1;
        }
    }
}