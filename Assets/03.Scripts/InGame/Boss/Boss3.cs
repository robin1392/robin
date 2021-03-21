using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss3 : Minion
{
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;
    
    public override IEnumerator Attack()
    {
        yield return SkillCoroutine(target.id);
    }
    
    IEnumerator SkillCoroutine(uint targetId)
    {
        BaseStat targetBS = null;
        if (targetId % 10000 == 0)
        {
            // targetBS = controller.targetPlayer;
        }
        else
        {
            targetBS = ActorProxy.GetBaseStatWithNetId(targetId);
        }

        if (targetBS == null) yield break;

        var targetPos = targetBS.transform.position;
        targetPos.y = 0;
        transform.LookAt(targetPos);
        animator.SetTrigger("AttackReady");
        
        yield return new WaitForSeconds(3f);
        
        animator.SetTrigger("Attack");
        if (ActorProxy.isPlayingAI)
        {
            var col = ts_ShootingPos.GetComponent<BoxCollider>();
            var hits = Physics.BoxCastAll(transform.position + col.center, col.size, ts_ShootingPos.forward);
            //var hits = Physics.RaycastAll(ts_ShootingPos.position, ts_ShootingPos.forward, 10f, targetLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                var m = hits[i].collider.GetComponent<BaseStat>();
                if (m != null)
                {
                    // controller.AttackEnemyMinionOrMagic(m.UID, m.id, power, 0);
                }
            }
        }
        
        yield return new WaitForSeconds(1f);
    }
}
