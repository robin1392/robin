using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Saint : Minion
    {
        public ParticleSystem ps_Heal;

        public override void Attack()
        {
            if (target == null || target.currentHealth >= target.maxHealth) return;

            controller.SendPlayer(RpcTarget.All,
                E_PTDefine.PT_ACTIVATEPOOLOBJECT,
                "Effect_Heal",
                transform.position,
                Quaternion.identity,
                new Vector3(1.5f, 1.5f, 0.3f));
            var cols = Physics.OverlapSphere(transform.position, range, friendlyLayer);
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.Get().IsNetwork() && isMine )
            {
                base.Attack();
                
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
                controller.MinionAniTrigger(id, "Attack");
                
                foreach (var col in cols)
                {
                    if (col != null && col.CompareTag("Minion_Ground") && col.gameObject != gameObject)
                    {
                        controller.HealMinion(col.GetComponentInParent<Minion>().id, effect);
                    }
                }
                
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.Get().IsNetwork() == false )
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
                foreach (var col in cols)
                {
                    if (col != null && col.CompareTag("Minion_Ground") && col.gameObject != gameObject)
                    {
                        controller.HealMinion(col.GetComponentInParent<Minion>().id, effect);
                    }
                }
            }
        }
        
        public override BaseStat SetTarget()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, friendlyLayer);
            Collider firstTarget = null;
            Collider closeToTarget = null;
            var closeToDistance = float.MaxValue;
            var distance = float.MaxValue;
            var oldHp = 1f;

            foreach (var col in cols)
            {
                if (col != null && col.CompareTag($"Minion_Ground") && col.gameObject != gameObject)
                {
                    var m = col.GetComponentInParent<Minion>();
                    var hp = m.currentHealth / m.maxHealth;
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis < closeToDistance && m.GetType() != typeof(Minion_Healer))
                    {
                        closeToDistance = dis;
                        closeToTarget = col;
                    }

                    if (hp < 1f && dis < distance)
                    {
                        oldHp = hp;
                        firstTarget = col;
                        distance = dis;
                    }
                }
            }

            if (firstTarget != null)
            {
                return firstTarget.GetComponentInParent<BaseStat>();
            }

            return closeToTarget != null ? closeToTarget.GetComponentInParent<BaseStat>() : null;
        }
    }
}