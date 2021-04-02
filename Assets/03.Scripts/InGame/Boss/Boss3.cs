using System.Collections;
using System.Collections.Generic;
using ED;
using ED.Boss;
using Microsoft.Win32.SafeHandles;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

public class Boss3 : BossBase
{
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;
    
    public override IEnumerator Attack()
    {
        var action = new Boss3SkillAction();
        yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
    }
}

public class Boss3SkillAction : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        var boss = actorProxy.baseStat as Boss3;

        if (targetActorProxy == null)
        {
            yield break;
        }
        
        var actorTransform = actorProxy.transform;
        var targetTransform = targetActorProxy.transform;

        var targetPos = targetTransform.position;
        targetPos.y = 0;
        actorTransform.LookAt(targetPos);
        boss.animator.SetTrigger("AttackReady");
        
        yield return new WaitForSeconds(3f);
        
        boss.animator.SetTrigger("Attack");
        
        if (actorProxy.isPlayingAI)
        {
            var col = boss.ts_ShootingPos.GetComponent<BoxCollider>();
            var hits = Physics.BoxCastAll(actorTransform.position + col.center, col.size, boss.ts_ShootingPos.forward);
            for (int i = 0; i < hits.Length; i++)
            {
                var m = hits[i].collider.GetComponent<BaseStat>();
                if (m != null && m.isAlive)
                {
                    m.ActorProxy.HitDamage(actorProxy.power);
                }
            }
        }
        
        yield return new WaitForSeconds(1f);
    }
}
