using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss2 : Minion
{
    private float _skillCastedTime;
    private bool _isSkillCasting;

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
        _skillCastedTime = -effectCooltime;
        attackSpeed = 1f;
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
            attackSpeed -= originAttackSpeed * 0.05f * loopCount;
            attackSpeed = Mathf.Clamp(attackSpeed, 0.25f, 4f);
            animator.speed = Mathf.Clamp(animationSpeed + 0.05f * loopCount, 0.25f, 4f);
        }
    }
}
