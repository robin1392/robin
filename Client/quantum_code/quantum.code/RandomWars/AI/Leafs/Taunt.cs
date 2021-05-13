using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class Taunt : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var transform = f.Get<Transform2D>(e);
            var attackable = f.Get<Attackable>(e);

            var actor = f.Get<Actor>(e);
            var hits = f.Physics2D.OverlapShape(transform, Shape2D.CreateCircle(attackable.EffectRangeValue));
            var endTime = attackable.EffectDurationTime + f.Now;
            for (int i = 0; i < hits.Count; i++)
            {
                var hitEntity = hits[i].Entity;
                if (hitEntity == EntityRef.None)
                {
                    continue;
                }

                if (f.Has<Hittable>(hitEntity) == false)
                {
                    continue;
                }

                if (f.TryGet(hitEntity, out Actor targetActor))
                {
                    if (actor.Team == targetActor.Team)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer(hitEntity, out Transform2D* targetTransform))
                {
                    f.Add<Taunted>(hitEntity);
                    var taunted = f.Unsafe.GetPointer<Taunted>(hitEntity);
                    taunted->EndTime = endTime;

                    var rotation =
                        FPVector2.RadiansSigned(FPVector2.Up, transform.Position - targetTransform->Position);
                    targetTransform->Rotation = rotation;

                    var targetBb = f.Unsafe.GetPointer<AIBlackboardComponent>(hitEntity);
                    targetBb->Set(f, "Target", e)->TriggerDecorators(p);
                }
            }

            var skill = f.Unsafe.GetPointer<Skill>(e);
            skill->AvailableTime = f.Now + skill->CoolTime;
            
            return BTStatus.Success;
        }
    }
}