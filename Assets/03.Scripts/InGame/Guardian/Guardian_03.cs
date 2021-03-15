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
    
    public override void Initialize()
    {
        base.Initialize();

        StartCoroutine(SkillCoroutine());
    }
    
    IEnumerator SkillCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(effectCooltime);
            
            ActorProxy.PlayAnimationWithRelay(_animatorHashSkill, target);

            yield return new WaitForSeconds(1.716f);

            Invincibility(2f);
        }
    }
}
