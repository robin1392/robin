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
            var pathfinder = p.Frame.Unsafe.GetPointer<NavMeshPathfinder>(p.Entity);
            pathfinder->IsActive = true;
            
            p.Frame.Events.ActionChanged(p.Entity, ActionStateType.Walk);
        }

        protected override BTStatus OnUpdate(BTParams p)
        {
            var bb = p.Frame.Get<AIBlackboardComponent>(p.Entity);
            var target = bb.GetEntityRef(p.Frame, Target.Key);
            
            if (p.Frame.Exists(target) == false)
            {
                return BTStatus.Failure;
            }
            
            if (target == EntityRef.None)
            {
                return BTStatus.Failure;
            }

            var targetTransform = p.Frame.Get<Transform2D>(target);
            var transform = p.Frame.Get<Transform2D>(p.Entity);
            
            var collider2D = p.Frame.Get<PhysicsCollider2D>(p.Entity);
            var targetCollider2D = p.Frame.Get<PhysicsCollider2D>(target);
            var acceptDistance = targetCollider2D.Shape.Circle.Radius + collider2D.Shape.Circle.Radius;
            
            var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position);
            if (distanceSquared < acceptDistance * acceptDistance)
            {
                return BTStatus.Success;
            }
            
            var pathfinder = p.Frame.Unsafe.GetPointer<NavMeshPathfinder>(p.Entity);
            pathfinder->SetTarget(p.Frame, targetTransform.Position, _navMesh);

            return BTStatus.Running;
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            var pathfinder = p.Frame.Unsafe.GetPointer<NavMeshPathfinder>(p.Entity);
            pathfinder->Stop(p.Frame, p.Entity, true);
            
            p.Frame.Events.ActionChanged(p.Entity, ActionStateType.Idle);
        }
    }
}