using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class Attack : BTLeaf
    {
        public AIBlackboardValueKey Target;
        public AIBlackboardValueKey IsEnemyTargetAttacked;
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
            var f = p.Frame;
            var e = p.Entity;
            var startTime = f.DeltaTime * p.Frame.Number;
            p.BtAgent->SetFPData(f, startTime, StartTimeIndex.Index);
            p.BtAgent->SetIntData(f, 0, HitIndex.Index);

            var actor = f.Get<Attackable>(e);
            p.Frame.Events.ActionChangedWithSpeed(p.Entity, ActionStateType.Attack, actor.GetAttackAniSpeed());
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
            
            var e = p.Entity;
            var actor = f.Get<Attackable>(e);
            
            var currentTime = f.DeltaTime * f.Number;
            var startTime = p.BtAgent->GetFPData(f, StartTimeIndex.Index);
            if (currentTime > startTime + actor.GetAttackHitEvent())
            {
                var hit = p.BtAgent->GetIntData(f, HitIndex.Index);
                if (hit < 1)
                {
                    var transform = f.Unsafe.GetPointer<Transform2D>(e);
                    var targetTransform = f.Get<Transform2D>(target);
                    var rotation = FPVector2.RadiansSigned(FPVector2.Up, targetTransform.Position - transform->Position);
                    transform->Rotation = rotation;
                    p.BtAgent->SetIntData(f, 1, HitIndex.Index);

                    var targetActor = f.Unsafe.GetPointer<Hittable>(target);
                    targetActor->Health -= actor.Power;
                    f.Events.ActorHitted(p.Entity, target, HitColor.None);

                    bb->Set(p.Frame, IsEnemyTargetAttacked.Key, true);
                }    
            }
            
            if (currentTime > startTime + actor.AttackSpeed)
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