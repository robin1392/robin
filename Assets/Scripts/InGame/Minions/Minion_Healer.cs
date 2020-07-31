using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Healer : Minion
    {
        public override void Attack()
        {
            if (target == null || !(target.currentHealth / target.maxHealth < 1f)) return;
            if (PhotonNetwork.IsConnected && isMine)
            {
                if (!(target.currentHealth / target.maxHealth < 1f)) return;
                base.Attack();
                controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.HealMinion(target.id, effect);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                ((Minion)target).Heal(effect);
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public override BaseStat SetTarget()
        {
            var cols = new Collider[50];
            var size = Physics.OverlapSphereNonAlloc(transform.position, 10f, cols, friendlyLayer);
            Collider firstTarget = null;
            Collider closeToTarget = null;
            var closeToDistance = float.MaxValue;
            var distance = float.MaxValue;
            var oldHp = 1f;

            foreach (var col in cols)
            {
                if (col != null && col.CompareTag($"Minion_Ground") && col.gameObject != gameObject)
                {
                    var m = col.GetComponent<Minion>();
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
                return firstTarget.GetComponent<BaseStat>();
            }

            return closeToTarget != null ? closeToTarget.GetComponent<BaseStat>() : null;
        }
    }
}
