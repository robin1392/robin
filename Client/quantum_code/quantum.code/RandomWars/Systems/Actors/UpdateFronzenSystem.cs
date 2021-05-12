using System;
using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe struct FrozenFilter
    {
        public EntityRef Entity;
        public Frozen* Frozen;
        public Buff* Actor;
    }
    public unsafe class UpdateFronzenSystem : SystemMainThreadFilter<FrozenFilter>
    {
        public override void Update(Frame f, ref FrozenFilter filter)
        {
            var currentTime = f.Number * f.DeltaTime;
            if (filter.Frozen->EndTime <= currentTime)
            {
                f.Remove<Frozen>(filter.Entity);
                filter.Actor->BuffState = filter.Actor->BuffState & (Int32)(~BuffType.Freeze);
            }
            else
            {
                filter.Actor->BuffState = filter.Actor->BuffState | (Int32)(BuffType.Freeze);
            }
        }
    }
}