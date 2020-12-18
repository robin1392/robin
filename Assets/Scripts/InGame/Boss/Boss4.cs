﻿using System.Collections;
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

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
        target = controller.targetPlayer;
        PoolManager.instance.AddPool(pref_Spear, 2);
    }

    public override void Attack()
    {
        if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
        
        if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
        {
            base.Attack();
            controller.MinionAniTrigger(id, "Attack", target.id);
            StartCoroutine(SkillCoroutine());
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
        yield return new WaitForSeconds(1f);

        if (InGameManager.IsNetwork && (isMine || controller.isPlayingAI))
        {
            controller.NetSendPlayer(GameProtocol.FIRE_BULLET_RELAY, id, target.id, power, 3f, (int)E_BulletType.VALLISTA_SPEAR);
        }
        controller.FireBullet(E_BulletType.VALLISTA_SPEAR, id, target.id, power, 3f);
        //NetSendPlayer(GameProtocol.FIRE_BULLET_RELAY, id, targetId , chDamage ,chSpeed , (int)bulletType);
    }
}
