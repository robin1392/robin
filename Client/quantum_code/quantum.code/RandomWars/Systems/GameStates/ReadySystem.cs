using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ReadySystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            f.Global->StartDelay = FP._1;
            f.Global->State = StateType.Ready;
        }

        public override void Update(Frame f)
        {
            if (f.Global->State != StateType.Ready)
            {
                return;
            }

            f.Global->StartDelay -= f.DeltaTime;
            if(f.Global->StartDelay < FP._0)
            {
                f.Global->State = StateType.Countdown;
            }
        }
    }
}