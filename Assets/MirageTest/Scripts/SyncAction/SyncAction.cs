using System.Collections;
using ED;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts.SyncAction
{
    public abstract class SyncActionBase
    {
        public virtual void OnActionCancel(ActorProxy actorProxy)
        {
            //마스터와 동기화를 받는 플레이어 양쪽 모두 수행됨.
            actorProxy.baseEntity.RunningAction = null;
            actorProxy.baseEntity.SyncAction = null;
        }

        public virtual bool NeedMoveSync => false;
    }
    
    public abstract class SyncActionWithTarget : SyncActionBase
    {
        public IEnumerator ActionWithSync(ActorProxy actor, ActorProxy target)
        {
            if (actor == null || actor.Client == null || actor.Client.Player == null)
            {
                yield break;
            }
            
            if (target == null)
            {
                yield break;
            }
            
            actor.SyncActionWithTarget(actor.Client.Player.Identity.NetId, GetType().Name, target.NetId);
            actor.baseEntity.RunningAction = this;
            yield return Action(actor, target);
            actor.baseEntity.RunningAction = null;
        }

        public abstract IEnumerator Action(ActorProxy actor, ActorProxy target);
    }
    
    public abstract class SyncActionWithoutTarget : SyncActionBase
    {
        public IEnumerator ActionWithSync(ActorProxy actor)
        {
            if (actor == null || actor.Client == null || actor.Client.Player == null)
            {
                yield break;
            }
            
            actor.SyncActionWithoutTarget(actor.Client.Player.Identity.NetId, GetType().Name);
            actor.baseEntity.RunningAction = this;
            yield return Action(actor);
            actor.baseEntity.RunningAction = null;
        }

        public abstract IEnumerator Action(ActorProxy actor);
    }
    
    public class ShielderAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actor)
        {
            var shielder = actor.baseEntity as Minion_Shielder;
            SoundManager.instance.Play(shielder.clip_ShieldMode);
            shielder.animator.SetTrigger(AnimationHash.Skill);
            yield return new WaitForSeconds(actor.baseEntity.effectDuration);
        }
    }
    
    public class ShieldAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actor)
        {
            var shield = actor.baseEntity as Minion_Shield;
            SoundManager.instance.Play(shield.clip_ShieldMode);
            shield.animator.SetTrigger(AnimationHash.SkillLoop);
            shield.halfDamage = true;
            
            var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_HalfDamage",
                ActorProxy.GetEffectPosition(shield, EffectLocation.Mid));
            ad.transform.SetParent(actor.transform);
            shield.effectShield = ad;
            
            yield return new WaitForSeconds(actor.effect);
            DisableShield(shield);
        }

        void DisableShield(Minion_Shield shield)
        {
            shield.halfDamage = false;
            if (shield.effectShield != null)
            {
                shield.effectShield.Deactive();
                shield.effectShield = null;
            }
        }

        public override void OnActionCancel(ActorProxy actorProxy)
        {            
            base.OnActionCancel(actorProxy);
            var shield = actorProxy.baseEntity as Minion_Shield;
            DisableShield(shield);
            shield.animator.SetTrigger(AnimationHash.Idle);
        }
    }
}