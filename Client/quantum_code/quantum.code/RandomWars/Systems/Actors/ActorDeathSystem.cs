using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct ActorDeathFilter
    {
        public EntityRef Entity;
        public Actor* Actor;
    }
    public unsafe class ActorDeathSystem : SystemMainThreadFilter<ActorDeathFilter>
    {
        public override void Update(Frame f, ref ActorDeathFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }
            
            if (filter.Actor->Health <= FP._0)
            {
                f.Destroy(filter.Entity);
                f.Events.ActorDeath(filter.Entity, filter.Actor->Team);
            }
        }
    }
}