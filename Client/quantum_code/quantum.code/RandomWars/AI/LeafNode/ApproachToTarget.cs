using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class ApproachToTarget : BTLeaf
    {
        public AIBlackboardValueKey Target;
        public FP AcceptDistance;
        private NavMesh _navMesh;

        public override void Init(Frame frame, AIBlackboardComponent* bbComponent, BTAgent* btAgent)
        {
            base.Init(frame, bbComponent, btAgent);
            
            _navMesh = frame.FindAsset<NavMesh>("Resources/DB/Map1_Navmesh");
        }

        protected override BTStatus OnUpdate(BTParams p)
        {
            var bb = p.Frame.Get<AIBlackboardComponent>(p.Entity);
            var target = bb.GetEntityRef(p.Frame, Target.Key);

            if (target == EntityRef.None)
            {
                return BTStatus.Failure;
            }

            var targetTransform = p.Frame.Get<Transform2D>(target);
            var transform = p.Frame.Get<Transform2D>(p.Entity);
            
            var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position);
            if (distanceSquared < AcceptDistance * AcceptDistance)
            {
                return BTStatus.Success;
            }
            
            var pathfinder = p.Frame.Unsafe.GetPointer<NavMeshPathfinder>(p.Entity);
            pathfinder->SetTarget(p.Frame, targetTransform.Position, _navMesh);

            return BTStatus.Running;
        }
    }
}