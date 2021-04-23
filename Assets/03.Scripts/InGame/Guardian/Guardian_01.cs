using System.Collections;
using System.Linq;
using ED;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

public class Guardian_01 : Minion
{
    private float _skillCastedTime;
    public GameObject _effectSkill;

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = -effectCooltime;
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

    public override BaseEntity SetTarget()
    {
        if (_attackedTarget != null && _attackedTarget.CanBeTarget())
        {
            return _attackedTarget;
        }
        else if (ActorProxy != null && ActorProxy.isTaunted)
        {
            ActorProxy.DisableBuffEffect(BuffType.Taunted);
        }

        if (ActorProxy == null)
        {
            return null;
        }

        if (ActorProxy.isInAllyCamp)
        {
            var position = transform.position;

            var target = ActorProxy.GetEnemies().Where(e => e.ActorProxy.isInEnemyCamp)
                .OrderBy(e => (e.transform.position - position).sqrMagnitude).FirstOrDefault();

            if (target != null)
            {
                return target;
            }
        }

        var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);

        Minion closestTarget = null;
        var distance = float.MaxValue;

        foreach (var col in cols)
        {
            var bs = col.GetComponentInParent<Minion>();
            if (bs == null || !bs.CanBeTarget())
            {
                continue;
            }

            var sqr = Vector3.SqrMagnitude(transform.position - col.transform.position);

            if (sqr < distance)
            {
                distance = sqr;
                closestTarget = bs;
            }
        }

        if (closestTarget != null)
        {
            return closestTarget;
        }

        return null;
    }
}

public class Guardian01Action : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        actorProxy.baseEntity.animator.SetTrigger(AnimationHash.Skill);

        yield return new WaitForSeconds(0.2f);

        PoolManager.instance.ActivateObject("Effect_Guardian_01_Skill", actorProxy.transform.position);

        if (actorProxy.isPlayingAI)
        {
            var cols = Physics.OverlapSphere(actorProxy.transform.position, 5f, actorProxy.baseEntity.targetLayer);
            foreach (var col in cols)
            {
                var m = col.GetComponentInParent<Minion>();
                if (m != null && m.isAlive)
                {
                    m.ActorProxy.HitDamage(actorProxy.baseEntity.effect);
                }
            }
        }

        yield return new WaitForSeconds(0.284f);
    }
}