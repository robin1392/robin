using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class IsTargetOutOfRange : BTDecorator
    {
        public AIBlackboardValueKey TargetRef;

        public override Boolean DryRun(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var bb = p.Blackboard;
            
            var enemy = bb->GetEntityRef(f, TargetRef.Key);
            if (f.Exists(enemy) == false)
            {
                return false;
            }

            var actor = f.Get<Attackable>(e);
            var col2d = f.Get<PhysicsCollider2D>(e);
            var targetCol2d = f.Get<PhysicsCollider2D>(enemy);
            
            var transform = f.Get<Transform2D>(e);
            var targetTransform = f.Get<Transform2D>(enemy);

            var range = actor.Range + col2d.Shape.Circle.Radius + targetCol2d.Shape.Circle.Radius;
            if (FPVector2.DistanceSquared(transform.Position, targetTransform.Position) < range * range)
            {
                return false;
            }
            
            return true;
        }
    }
}