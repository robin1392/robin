using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class ApproachToTarget : BTLeaf
    {
        public AIBlackboardValueKey Target;
        private NavMesh _navMesh;

        public override void Init(Frame frame, AIBlackboardComponent* bbComponent, BTAgent* btAgent)
        {
            base.Init(frame, bbComponent, btAgent);
            _navMesh = frame.Context.NavMesh;
        }

        public override void OnEnter(BTParams p)
        {
            base.OnEnter(p);
            var f = p.Frame;
            var e = p.Entity;
            var agent = f.Unsafe.GetPointer<NavMeshPathfinder>(e);
            agent->IsActive = true;
            
            var body = f.Unsafe.GetPointer<PhysicsBody2D>(e);
            body->FreezeRotation = false;
            
            f.Events.ActionChanged(p.Entity, ActionStateType.Walk);
        }

        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var bb = f.Get<AIBlackboardComponent>(e);
            var target = bb.GetEntityRef(f, Target.Key);
            
            if (f.Exists(target) == false)
            {
                return BTStatus.Failure;
            }
            
            if (target == EntityRef.None)
            {
                return BTStatus.Failure;
            }

            var targetTransform = f.Get<Transform2D>(target);
            var transform = f.Get<Transform2D>(e);
            
            var acceptDistance = FP._0_03;
            var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position);
            if (distanceSquared < acceptDistance * acceptDistance)
            {
                return BTStatus.Success;
            }
            
            var col2d = f.Get<PhysicsCollider2D>(e);
            var targetCol2d = f.Get<PhysicsCollider2D>(target);
            var targetRadius = targetCol2d.Shape.Circle.Radius;
            var targetVector = (transform.Position - targetTransform.Position).Normalized
                               * (targetRadius + FP._0_25);
            
            var segmentCount = FPMath.RoundToInt(targetRadius * FP.Pi /  col2d.Shape.Circle.Radius);  
            var targetPosition = GetTargetPosition(f, targetTransform.Position, targetVector, segmentCount);
            var agent = f.Unsafe.GetPointer<NavMeshPathfinder>(e);
            agent->SetTarget(f, targetPosition.XOY, _navMesh);

            return BTStatus.Running;
        }

        FPVector2 GetTargetPosition(Frame f, FPVector2 target, FPVector2 targetVector, Int32 segmentCount)
        {
            var segment = 360 / segmentCount;
            var slot = targetVector;
            for (var i = 1; i <= segmentCount; ++i)
            {
                var hit = f.Physics2D.Linecast(target, target + slot);
                if (hit == null)
                {
                    return target + slot;
                }

                slot = FPVector2.Rotate(slot, FP.Deg2Rad * i * segment * (i % 2 == 0 ? 1 : -1));
            }
            return target + targetVector;
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            var f = p.Frame;
            var e = p.Entity;
            var body = f.Unsafe.GetPointer<PhysicsBody2D>(e);
            body->FreezeRotation = true;
            
            var agent = f.Unsafe.GetPointer<NavMeshPathfinder>(e);
            agent->Stop(f, p.Entity);
            f.Events.ActionChanged(e, ActionStateType.Idle);
        }
    }
}