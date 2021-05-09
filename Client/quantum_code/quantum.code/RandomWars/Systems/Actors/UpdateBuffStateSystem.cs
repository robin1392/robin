using System;
using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct ActorFilter
    {
        public EntityRef Entity;
        public Actor* Actor;
    }
    public unsafe class UpdateBuffStateSystem : SystemMainThreadFilter<ActorFilter>
    {
        public override void Update(Frame f, ref ActorFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }

            if (filter.Actor->BuffState != filter.Actor->BuffStateApplied)
            {
                var cantActPrev = (filter.Actor->BuffStateApplied & (Int32)BuffType.CantAct) > 0;
                var cantActNext = (filter.Actor->BuffState & (Int32)BuffType.CantAct) > 0;

                if (cantActPrev != cantActNext)
                {
                    var bb = f.Unsafe.GetPointer<AIBlackboardComponent>(filter.Entity);
                    bb->Set(f, "CanAct", !cantActNext)->TriggerDecorators(new BTParams()
                    {
                        Frame = f,
                        Entity = filter.Entity,
                        BtAgent = f.Unsafe.GetPointer<BTAgent>(filter.Entity),
                        Blackboard = bb,
                    });
                }
                
                filter.Actor->BuffStateApplied = filter.Actor->BuffState;
                f.Events.BuffStateChanged(filter.Entity, filter.Actor->BuffStateApplied);
            }
        }
    }
}