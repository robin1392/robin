using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class SetNearestEnemyToTarget : BTLeaf
    {
        public AIBlackboardValueKey Target;
        public FP Range;

        protected override BTStatus OnUpdate(BTParams p)
        {
            var transform = p.Frame.Get<Transform2D>(p.Entity);
            var actor = p.Frame.Get<Actor>(p.Entity);
            
            var hits = p.Frame.Physics2D.OverlapShape(
                transform, Shape2D.CreateCircle(Range));

            var nearest = EntityRef.None;
            var nearestDistanceSquared = FP.MaxValue;
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].Entity == EntityRef.None)
                {
                    continue;
                }
                
                if (p.Frame.Unsafe.TryGetPointer(hits[i].Entity, out Actor* target))
                {
                    if (actor.Team == target->Team)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                
                if (p.Frame.TryGet(hits[i].Entity, out Transform2D targetTransform))
                {
                    var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position); 
                    if (distanceSquared < nearestDistanceSquared)
                    {
                        nearestDistanceSquared = distanceSquared;
                        nearest = hits[i].Entity;
                    }
                }
            }
            
            var bb = p.Frame.Unsafe.GetPointer<AIBlackboardComponent>(p.Entity);
            bb->Set(p.Frame, Target.Key, nearest);

            if (nearest != EntityRef.None)
            {
                return BTStatus.Success;    
            }

            return BTStatus.Failure;
        }
    }
}