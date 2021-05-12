using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ReadySystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            f.Global->State = StateType.Ready;
        }

        public override void Update(Frame f)
        {
            if (f.Global->State != StateType.Ready)
            {
                return;
            }

            if (f.Global->Players[0].PlayerRef != PlayerRef.None &&
                f.Global->Players[1].PlayerRef != PlayerRef.None)
            {
                f.Global->State = StateType.Countdown;
            }
        }
    }
}