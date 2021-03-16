using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;

public class Guardian_01 : Minion
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

            yield return new WaitForSeconds(0.716f);

            var cols = Physics.OverlapSphere(transform.position, 2f, targetLayer);
            for (int i = 0; i < cols.Length; i++)
            {
                var bs = cols[i].GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    DamageToTarget(bs);
                }
            }
        }
    }
}
