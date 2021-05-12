using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct HittableFilter
    {
        public EntityRef Entity;
        public Health* Health;
        public DamagePerSec* DamagePerSec;
    }
    
    public unsafe class DamagePerSecSystem : SystemMainThreadFilter<HittableFilter>
    {
        public override void Update(Frame f, ref HittableFilter filter)
        {
            filter.Health->Value -= filter.DamagePerSec->Damage * f.DeltaTime;
        }
    }
}