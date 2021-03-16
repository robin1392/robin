using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;

public class Guardian_02 : Minion
{
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
            
            ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);

            yield return new WaitForSeconds(1.716f);

            Invincibility(2f);
        }
    }
}
