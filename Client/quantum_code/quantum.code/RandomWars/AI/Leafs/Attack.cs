using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class Attack : BTLeaf
    {
        public AIBlackboardValueKey Target;
        public AIBlackboardValueKey IsEnemyTargetAttacked;
        public FP Duration = 1;
        public FP HitFrame = FP._0_50;
        public BTDataIndex StartTimeIndex;
        public BTDataIndex HitIndex;
        
        public override void Init(Frame frame, AIBlackboardComponent* bbComponent, BTAgent* btAgent)
        {
            base.Init(frame, bbComponent, btAgent);

            // We allocate space for the End Time on the Agent so we can change it in runtime
            btAgent->AddFPData(frame, 0);
            btAgent->AddIntData(frame, 0);
        }
        
        public override void OnEnter(BTParams p)
        {
            base.OnEnter(p);
            var startTime = p.Frame.DeltaTime * p.Frame.Number;
            p.BtAgent->SetFPData(p.Frame, startTime, StartTimeIndex.Index);
            p.BtAgent->SetIntData(p.Frame, 0, HitIndex.Index);
            
            p.Frame.Events.ActionChanged(p.Entity, ActionStateType.Attack);
        }
        
        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var bb = p.Blackboard;
            var target = bb->GetEntityRef(f, Target.Key);
            if (f.Exists(target) == false)
            {
                return BTStatus.Failure;
            }
            
            if (target == EntityRef.None)
            {
                return BTStatus.Failure;
            }
            
            var currentTime = f.DeltaTime * f.Number;
            var startTime = p.BtAgent->GetFPData(f, StartTimeIndex.Index);
            if (currentTime > startTime + HitFrame)
            {
                var hit = p.BtAgent->GetIntData(f, HitIndex.Index);
                if (hit < 1)
                {
                    var transform = f.Unsafe.GetPointer<Transform2D>(p.Entity);
                    var targetTransform = f.Get<Transform2D>(target);
                    transform->Rotation = FPVector2.Angle(targetTransform.Position - transform->Position, FPVector2.Up);
                    p.BtAgent->SetIntData(f, 1, HitIndex.Index);
                    var actor = f.Get<Actor>(p.Entity);
                    var targetActor = f.Unsafe.GetPointer<Actor>(target);
                    targetActor->Health -= actor.Power;
                    f.Events.ActorHitted(p.Entity, target);
                    
                    bb->Set(p.Frame, IsEnemyTargetAttacked.Key, true);
                }    
            }
            
            if (currentTime > startTime + Duration)
            {
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            p.Frame.Events.ActionChanged(p.Entity, ActionStateType.Idle);
        }
    }
}