using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ED;
using MirageTest.Scripts;

public class Guardian_03 : Minion
{
    public float bulletMoveSpeed = 6f;
    public GameObject pref_Bullet;

    private float _skillCastedTime;

    protected override void Start()
    {
        base.Start();
        
        _animationEvent.event_FireArrow -= FireArrow;
        _animationEvent.event_FireArrow += FireArrow;
        PoolManager.instance.AddPool(pref_Bullet, 5);
    }
    
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

    public void FireArrow()
    {
        //TODO: 빼도 되지 않을까? 고민해보자
        if (target == null || IsTargetInnerRange() == false)
        {
            animator.SetTrigger(AnimationHash.Idle);
            return;
        }

        if (ActorProxy.isPlayingAI)
        {
            ActorProxy.FireBulletWithRelay(E_BulletType.GUARDIAN3_BULLET, target, power, bulletMoveSpeed);
        }
    }

    IEnumerator SkillCoroutine()
    {
        var cols = Physics.OverlapSphere(transform.position, 2f, targetLayer);

        if (cols.Length <= 0) yield break;
        
        ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
        
        yield return new WaitForSeconds(0.75f);

        foreach (var col in cols)
        {
            var minion = col.GetComponentInParent<Minion>();
            if (minion != null)
            {
                minion.ActorProxy?.HitDamage(effect);
            }
        }

        yield return new WaitForSeconds(0.25f);
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
