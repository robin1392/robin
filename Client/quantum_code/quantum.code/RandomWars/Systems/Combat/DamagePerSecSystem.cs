using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct HittableFilter
    {
        public EntityRef Entity;
        public Hittable* Hittable;
        public DamagePerSec* DamagePerSec;
    }
    
    public unsafe class DamagePerSecSystem : SystemMainThreadFilter<HittableFilter>
    {
        public override void Update(Frame f, ref HittableFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }

            filter.Hittable->Health -= filter.DamagePerSec->Damage * f.DeltaTime;
        }
    }
}