using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;

public class Guardian_01 : Minion
{
    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);

        StartCoroutine(SkillCoroutine());
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

    IEnumerator SkillCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(effectCooltime);

            isPushing = true;
            controller.MinionAniTrigger(id, "Skill", 0);

            yield return new WaitForSeconds(1f);

            var cols = Physics.OverlapSphere(transform.position, 2f, targetLayer);
            for (int i = 0; i < cols.Length; i++)
            {
                var bs = cols[i].GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    DamageToTarget(bs);
                }
            }
            
            yield return new WaitForSeconds(1.2f);
            isPushing = false;
        }
    }
}
