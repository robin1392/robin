using System.Collections;
using ED.Boss;
using UnityEngine;

public class Boss2 : BossBase
{
    public override IEnumerator Attack()
    {
        var elapsed = (float)ActorProxy.NetworkTime.Time - ActorProxy.spawnTime;
        var loopCount = (int)(elapsed / effectCooltime);
        var attackSpeed = Mathf.Clamp(1f + 0.05f * loopCount, 1f, 5f);
        
        return AttackCoroutine(attackSpeed);
    }
}