using System.Collections;
using System.Collections.Generic;
using ED;
using ED.Boss;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss4 : BossBase
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

    public override IEnumerator Attack()
    {
        ActorProxy.PlayAnimationWithRelay(AnimationHash.Attack, target);

        yield return new WaitForSeconds(2f);
        
        ActorProxy.FireBulletWithRelay(E_BulletType.VALLISTA_SPEAR, target, power, 12f);
    }
}
