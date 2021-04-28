using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ED;
using MirageTest.Scripts;

public class Guardian_02 : Minion
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
            yield return SkillCoroutine();
        }

        yield return base.Attack();
    }
    
    IEnumerator SkillCoroutine()
    {
        ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
        
        yield return new WaitForSeconds(0.8f);
        
        ActorProxy.AddBuff(BuffInfos.Invincibility, 2f);
        
        yield return new WaitForSeconds(0.2f);
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
