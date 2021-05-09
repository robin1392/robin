using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class RangeAttack : BTLeaf
    {
        private static readonly string PROJECTILE_PROTOTYPE = "Resources/DB/EntityPrototypes/Projectile|EntityPrototype";
        
        public AIBlackboardValueKey Target;
        public AIBlackboardValueKey IsEnemyTargetAttacked;
        public BTDataIndex StartTimeIndex;
        public BTDataIndex HitIndex;
        public string ProjectileModel;
        public string HitEffect;
        public FP ProjectTileSpeed = FP._6;
        
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
            
            var e = p.Entity;
            var actor = f.Get<Actor>(e);
            var currentTime = f.DeltaTime * f.Number;
            var startTime = p.BtAgent->GetFPData(f, StartTimeIndex.Index);
            if (currentTime > startTime + actor.GetAttackHitEvent())
            {
                var hit = p.BtAgent->GetIntData(f, HitIndex.Index);
                if (hit < 1)
                {
                    var transform = f.Unsafe.GetPointer<Transform2D>(e);
                    var targetTransform = f.Get<Transform2D>(target);
                    var targetCollider = f.Get<PhysicsCollider2D>(target);
                    var rotation = FPVector2.RadiansSigned(FPVector2.Up, targetTransform.Position - transform->Position);
                    transform->Rotation = rotation;
                    p.BtAgent->SetIntData(f, 1, HitIndex.Index);
                    
                    var prototype = f.FindAsset<EntityPrototype>(PROJECTILE_PROTOTYPE);
                    var projectileEntity = f.Create(prototype);
                    var projectile = f.Unsafe.GetPointer<Projectile>(projectileEntity);
                    projectile->Owner = actor.Owner;
                    projectile->Attacker = e;
                    projectile->Defender = target;
                    projectile->Power = actor.Power;
                    projectile->Team = actor.Team;
                    projectile->Model = ProjectileModel;
                    projectile->HitEffect = HitEffect;

                    var distance = FPVector2.Distance(targetTransform.Position, transform->Position) - targetCollider.Shape.Circle.Radius;
                    var hitTime = distance / ProjectTileSpeed; 
                    projectile->HitTime = currentTime + hitTime;
                    
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