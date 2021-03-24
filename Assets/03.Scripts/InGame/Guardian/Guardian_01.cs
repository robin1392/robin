using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;

public class Guardian_01 : Minion
{
    private float _skillCastedTime;

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = -effectCooltime;
    }

    public override IEnumerator Attack()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            _skillCastedTime = _spawnedTime;
            var action = new Guardian01Action();
            RunningAction = action;
            yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;
        }

        yield return base.Attack();
    }
}

public class Guardian01Action : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        actorProxy.baseStat.animator.SetTrigger(AnimationHash.Skill);

        yield return new WaitForSeconds(0.716f);

        if (actorProxy.isPlayingAI == false)
        {
            yield break;
        }
            
        var cols = Physics.OverlapSphere(actorProxy.transform.position, 2f, actorProxy.baseStat.targetLayer);
        foreach (var col in cols)
        {
            if (col.CompareTag("Player")) continue;

            var m = col.GetComponentInParent<BaseStat>();
            if (m != null && m.isAlive)
            {
                m.ActorProxy.HitDamage(actorProxy.baseStat.power);
            }
        }
        
        yield return new WaitForSeconds(0.284f);
    }
}