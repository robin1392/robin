using Photon.Deterministic;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class CountDownSystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            f.Global->StartCountdownInt = f.Context.TableData.VsMode.KeyValues[(int)EVsmodeKey.StartCoolTime].value;
            f.Global->StartCountdown = f.Global->StartCountdownInt + 1;
        }

        public override void Update(Frame f)
        {
            if (f.Global->State != StateType.Countdown)
            {
                return;
            }
            
            var countDownInt = FPMath.FloorToInt(f.Global->StartCountdown);
            if (countDownInt != f.Global->StartCountdownInt)
            {
                f.Events.CountDown(countDownInt);
                f.Global->StartCountdownInt = countDownInt;
            }
            
            f.Global->StartCountdown -= f.DeltaTime;
            if(f.Global->StartCountdown < FP._0)
            {
                f.Global->State = StateType.Play;
            }
        }
    }
}