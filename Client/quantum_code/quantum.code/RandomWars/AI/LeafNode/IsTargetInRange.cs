using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class IsTargetInRange : BTLeaf
    {
        public AIBlackboardValueKey Target;
        public FP Range;

        protected override BTStatus OnUpdate(BTParams p)
        {
            var transform =  p.Frame.Get<Transform2D>(p.Entity);
                
            var bb = p.Frame.Get<AIBlackboardComponent>(p.Entity);
            var target = bb.GetEntityRef(p.Frame, Target.Key);
            if (target == EntityRef.None)
            {
                return BTStatus.Failure;
            }
            
            var targetTransform = p.Frame.Get<Transform2D>(target);
            
            var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position);
            if (distanceSquared < Range * Range)
            {
                return BTStatus.Success; 
            }

            return BTStatus.Failure;
        }
    }
}