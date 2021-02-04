using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;

public class Guardian_03 : Minion
{
    public GameObject pref_Bullet;

    protected override void Start()
    {
        base.Start();
        
        PoolManager.instance.AddPool(pref_Bullet, 5);
    }
    
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
            
            controller.MinionAniTrigger(id, "Skill", target.id);

            yield return new WaitForSeconds(1.716f);

            Invincibility(2f);
        }
    }
}
