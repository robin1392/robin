using System.Collections;
using ED;
using UnityEngine;

namespace MirageTest.Scripts.SyncAction
{
    public abstract class SyncActionBase
    {
        public virtual void OnActionCancel(ActorProxy actorProxy)
        {
            //마스터와 동기화를 받는 플레이어 양쪽 모두 수행됨.
            actorProxy.baseStat.RunningAction = null;
            actorProxy.baseStat.SyncAction = null;
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
            actor.baseStat.RunningAction = this;
            yield return Action(actor, target);
            actor.baseStat.RunningAction = null;
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
            actor.baseStat.RunningAction = this;
            yield return Action(actor);
            actor.baseStat.RunningAction = null;
        }

        public abstract IEnumerator Action(ActorProxy actor);
    }
    
    public class ShielderAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actor)
        {
            var shielder = actor.baseStat as Minion_Shielder;
            SoundManager.instance.Play(shielder.clip_ShieldMode);
            shielder.animator.SetTrigger(AnimationHash.Skill);
            yield return new WaitForSeconds(actor.baseStat.effectDuration);
        }
    }
    
    public class ShieldAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actor)
        {
            var shield = actor.baseStat as Minion_Shield;
            SoundManager.instance.Play(shield.clip_ShieldMode);
            shield.animator.SetTrigger(AnimationHash.Skill);
            yield return new WaitForSeconds(actor.baseStat.effectDuration);
        }
    }
}