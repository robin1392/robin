using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Healer : Minion
    {
        [Header("Prefab")] public GameObject pref_Heal;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Heal, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            attackSpeed = effectCooltime;
        }

        public override void Attack()
        {
            if (target == null || !IsFriendlyLayer(target.gameObject) || target.currentHealth >= target.maxHealth) return;
            
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HEALMINION, target.id, effect);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
                controller.HealMinion(target.id, effect);
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

            return closeToTarget != null ? closeToTarget.GetComponentInParent<BaseStat>() : controller.targetPlayer;
        }
    }
}
