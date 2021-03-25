using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;

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
                minion.ActorProxy.HitDamage(effect);
            }
        }

        yield return new WaitForSeconds(0.25f);
    }
}
