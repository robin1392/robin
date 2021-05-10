using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct DestroyFilter
    {
        public EntityRef Entity;
        public Destroy* Destroy;
    }
    public unsafe class DestroyByComponent : SystemMainThreadFilter<DestroyFilter>
    {
        public override void Update(Frame f, ref DestroyFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }
            
            f.Destroy(filter.Entity);
        }
    }
}