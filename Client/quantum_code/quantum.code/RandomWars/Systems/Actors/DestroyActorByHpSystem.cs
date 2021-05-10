using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct ActorDeathFilter
    {
        public EntityRef Entity;
        public Hittable* Hittable;
    }
    public unsafe class DestroyActorByHpSystem : SystemMainThreadFilter<ActorDeathFilter>
    {
        public override void Update(Frame f, ref ActorDeathFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }
            
            //HACK:
            if(f.Has<Mine>(filter.Entity))
            {
                return;
            }
            
            if (filter.Hittable->Health <= FP._0)
            {
                f.Destroy(filter.Entity);
            }
        }
    }
}