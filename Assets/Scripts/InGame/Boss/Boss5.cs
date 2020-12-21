using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss5 : Minion
{
    [Header("Effect")] public GameObject obj_Attack;
    public GameObject obj_AttackHit;
    
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;

    protected override void Start()
    {
        base.Start();
        
        PoolManager.Get().AddPool(obj_Attack, 2);
        PoolManager.Get().AddPool(obj_AttackHit, 2);
        GetComponentInChildren<MinionAnimationEvent>().event_Attack += Callback_Attack;
    }

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
    }

    public override void Attack()
    {
        if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
        
        if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
        {
            base.Attack();
            controller.MinionAniTrigger(id, "Attack", target.id);
        }
        else if(InGameManager.IsNetwork == false)
        {
            base.Attack();
            animator.SetTrigger(_animatorHashAttack);
        }
    }

    public override void Death()
    {
        base.Death();

        GetComponentInChildren<MinionAnimationEvent>().event_Attack -= Callback_Attack;
    }

    public void Callback_Attack()
    {
        if (target != null)
        {
            var attack = PoolManager.Get().ActivateObject(obj_Attack.name);
            if (attack != null)
            {
                var ts_hit = target.ts_HitPos;
                if (ts_hit == null) ts_hit = target.transform;
                attack.DOMove(ts_hit.position, 0.3f).OnComplete(() =>
                {
                    attack.GetComponent<PoolObjectAutoDeactivate>().Deactive();
                    if (isMine || controller.isPlayingAI) DamageToTarget(target, 0);
                    PoolManager.Get().ActivateObject(obj_AttackHit.name);
                });
            }
        }
    }

    public void Skill()
    {
        StartCoroutine(SkillCoroutine());
    }

    IEnumerator SkillCoroutine()
    {
        float originAttackSpeed = attackSpeed;
        float animationSpeed = 1f;
        int loopCount = 1;
        
        while (true)
        {
            yield return new WaitForSeconds(effectCooltime);

            loopCount++;
            _localAttackSpeed = Mathf.Clamp(1f + 0.05f * loopCount, 1f, 5f);
            attackSpeed = 1f / _localAttackSpeed;
            animator.SetFloat("AttackSpeed", _localAttackSpeed);
        }
    }
}
