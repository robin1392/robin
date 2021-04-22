using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;

public class Guardian_01 : Minion
{
    private float _skillCastedTime;
    public GameObject _effectSkill;

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = 0;
        PoolManager.instance.AddPool(_effectSkill, 1);
    }

    protected override IEnumerator Combat()
    {
        yield return Skill();
        
        yield return base.Combat();
    }

    public override IEnumerator Attack()
    {
        yield return Skill();

        if (IsTargetInnerRange() == false)
        {
            yield break;
        }

        yield return base.Attack();
    }

    IEnumerator Skill()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            _skillCastedTime = _spawnedTime;
            var action = new Guardian01Action();
            RunningAction = action;
            yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;
        }
    }
}

public class Guardian01Action : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        actorProxy.baseEntity.animator.SetTrigger(AnimationHash.Skill);

        yield return new WaitForSeconds(0.5f);

        PoolManager.instance.ActivateObject("Effect_Guardian_01_Skill", actorProxy.transform.position);
        
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