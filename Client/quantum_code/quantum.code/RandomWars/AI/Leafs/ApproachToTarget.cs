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

            var actor = f.Get<Actor>(e);
            var direction = (targetTransform.Position - transform.Position).Normalized * actor.MoveSpeed;
            var body =f.Unsafe.GetPointer<PhysicsBody2D>(e);
            body->AddForce(direction * f.DeltaTime);
            
            return BTStatus.Running;
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            var f = p.Frame;
            var e = p.Entity;
            f.Unsafe.GetPointer<PhysicsBody2D>(e)->Velocity = FPVector2.Zero;
            f.Events.ActionChanged(e, ActionStateType.Idle);
        }
    }
}