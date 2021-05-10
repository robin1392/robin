using Photon.Deterministic;

namespace Quantum.Combat
{
    public unsafe class StoneBallTriggerSystem : SystemSignalsOnly, ISignalOnTriggerEnter2D
    {
        public void OnTriggerEnter2D(Frame frame, TriggerInfo2D info)
        {
            if (frame.Has<StoneBall>(info.Other))
            {
                OnStoneBallCollision(frame, info.Other, info.Entity, info);
                return;
            }

            if (frame.Has<StoneBall>(info.Entity))
            {
                OnStoneBallCollision(frame, info.Entity, info.Other, info);
                return;
            }
        }

        void OnStoneBallCollision(Frame frame, EntityRef stoneBall, EntityRef other, TriggerInfo2D info)
        {
            var actor = frame.Get<Actor>(stoneBall);
            var transform = frame.Get<Transform2D>(stoneBall);

            if (frame.Has<Tower>(other))
            {
                return;
            }
            
            if (frame.TryGet<Actor>(other, out var otherActor))
            {
                if (otherActor.Team != actor.Team)
                {
                    if (frame.Unsafe.TryGetPointer(other, out Hittable* hittable))
                    {
                        var attackable = frame.Get<Attackable>(stoneBall);
                        hittable->Health -= attackable.Power;

                        frame.Events.PlayCasterEffect(stoneBall, "Effect_Stone");
                    }
                }
            }

            if (actor.Team == GameConstants.BottomCamp && transform.Position.Y < FP._0)
            {
                return;
            }

            if (actor.Team == GameConstants.TopCamp && transform.Position.Y > FP._0)
            {
                return;
            }

            if (info.IsStatic)
            {
                frame.Destroy(info.Entity);
            }
        }
        
    }
}