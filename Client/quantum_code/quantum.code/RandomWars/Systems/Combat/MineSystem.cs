using Photon.Deterministic;

namespace Quantum.Combat
{
    public unsafe struct MineFilter
    {
        public EntityRef Entity;
        public Mine* Mine;
    }
    
    public unsafe class MineSystem : SystemMainThreadFilter<MineFilter>, ISignalOnTriggerEnter2D
    {
        public void OnTriggerEnter2D(Frame frame, TriggerInfo2D info)
        {
            if (frame.TryGet(info.Other, out Mine mine) && mine.Arrived)
            {
                OnMineCollision(frame, info.Other, info.Entity, info);
                return;
            }

            if (frame.TryGet(info.Entity, out Mine mine2) && mine2.Arrived)
            {
                OnMineCollision(frame, info.Entity, info.Other, info);
                return;
            }
        }

        void OnMineCollision(Frame frame, EntityRef mine, EntityRef other, TriggerInfo2D info)
        {
            if (frame.Has<Tower>(other))
            {
                return;
            }

            if (frame.Has<Hittable>(other) == false)
            {
                return;
            }

            if (frame.TryGet(other, out Actor otherActor))
            {
                var actor = frame.Get<Actor>(mine);
                if (otherActor.Team == actor.Team)
                {
                    return;   
                }
            }
            
            if (frame.Has<Trigger>(mine))
            {
                return;
            }
            
            frame.Add<Trigger>(mine);
        }

        public override void Update(Frame f, ref MineFilter filter)
        {
            if (f.Has<Trigger>(filter.Entity))
            {
                return;
            }

            if (f.Get<Health>(filter.Entity).Value <= FP._0)
            {
                f.Add<Trigger>(filter.Entity);
            }
        }
    }
}