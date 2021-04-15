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

        if (IsTargetInnerRange() == false)
        {
            yield break;
        }

        yield return base.Attack();
    }
}

public class Guardian01Action : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        actorProxy.baseEntity.animator.SetTrigger(AnimationHash.Skill);

        yield return new WaitForSeconds(0.716f);

        if (actorProxy.isPlayingAI)
        {
            var cols = Physics.OverlapSphere(actorProxy.transform.position, 2f, actorProxy.baseEntity.targetLayer);
            foreach (var col in cols)
            {
                var m = col.GetComponentInParent<Minion>();
                if (m != null && m.isAlive)
                {
                    m.ActorProxy.HitDamage(actorProxy.baseEntity.power);
                }
            }
        }

        yield return new WaitForSeconds(0.284f);
    }
}