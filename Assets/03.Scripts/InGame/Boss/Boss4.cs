using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss4 : Minion
{
    public GameObject pref_Spear;
    
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;

    protected override void Start()
    {
        base.Start();
        
        PoolManager.instance.AddPool(pref_Spear, 2);
    }

    public override void Initialize()
    {
        base.Initialize();
        target = controller.targetPlayer;
        transform.position = transform.position.z > 0
            ? transform.position + Vector3.back
            : transform.position + Vector3.forward;
        //KZSee:
        // attackSpeed = 3f;
    }

    public override IEnumerator Attack()
    {
        ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);

        yield return new WaitForSeconds(2f);
        
        ActorProxy.FireBulletWithRelay(E_BulletType.VALLISTA_SPEAR, target, power, 12f);
        
        
        // yield return base.Attack();
        // if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
        //
        // if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
        // {
        //     base.Attack();
        //     controller.MinionAniTrigger(id, "Attack", target.id);
        //     StartCoroutine(SkillCoroutine());
        // }
        // else if(InGameManager.IsNetwork == false)
        // {
        //     base.Attack();
        //     animator.SetTrigger(_animatorHashAttack);
        // }
    }

    public void Skill()
    {
        StartCoroutine(SkillCoroutine());
    }

    IEnumerator SkillCoroutine()
    {
        yield return new WaitForSeconds(2f);

        ActorProxy.FireBulletWithRelay(E_BulletType.VALLISTA_SPEAR, target, power, 12f);
    }
}
