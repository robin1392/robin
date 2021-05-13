using System;
using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct TauntedFilter
    {
        public EntityRef Entity;
        public Taunted* Taunted;
        public Buff* Actor;
    }
    public unsafe class UpdateTauntedSystem : SystemMainThreadFilter<TauntedFilter>
    {
        public override void Update(Frame f, ref TauntedFilter filter)
        {
            var currentTime = f.Number * f.DeltaTime;
            if (filter.Taunted->EndTime <= currentTime)
            {
                f.Remove<Taunted>(filter.Entity);
                filter.Actor->BuffState = filter.Actor->BuffState & (Int32)(~BuffType.Taunted);
            }
            else
            {
                filter.Actor->BuffState = filter.Actor->BuffState | (Int32)(BuffType.Taunted);
            }
        }
    }
}