using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss5 : Minion
{
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
        _skillCastedTime = -effectCooltime;
        attackSpeed = 1f;
        effectCooltime = 1f;
        Skill();
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
