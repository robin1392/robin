namespace Quantum
{
    public unsafe class UpdateCommingSpByWaveSystem : SystemSignalsOnly, ISignalOnWave
    {
        public void OnWave(Frame f, int wave)
        {
            for (var i = 0; i < f.Global->Players.Length; ++i)
            {
                var player = f.Global->Players[i];
                if (player.PlayerRef == PlayerRef.None)
                {
                    continue;
                }
                
                var sp = f.Unsafe.GetPointer<Sp>(player.EntityRef);
                sp->CommingSp = f.CalculateCommingSp(sp->CommingSpGrade);
                
                f.Events.CommingSpChanged(player.PlayerRef);
            }
        }
    }
}