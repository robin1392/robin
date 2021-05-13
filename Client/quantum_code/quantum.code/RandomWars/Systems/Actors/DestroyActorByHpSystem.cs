using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct ActorDeathFilter
    {
        public EntityRef Entity;
        public Health* Health;
    }
    public unsafe class DestroyActorByHpSystem : SystemMainThreadFilter<ActorDeathFilter>
    {
        public override void Update(Frame f, ref ActorDeathFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }

            if (f.Has<Hittable>(filter.Entity) == false)
            {
                return;
            }

            if (filter.Health->Value <= FP._0)
            {
                if (f.Has<Tower>(filter.Entity))
                {
                    f.Signals.OnTowerDestroyed(filter.Entity);
                }
                
                f.Destroy(filter.Entity);
            }
        }
    }
}