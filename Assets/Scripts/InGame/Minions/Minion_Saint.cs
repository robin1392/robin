﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Saint : Minion
    {
        public GameObject pref_HealArea;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_HealArea, 1);
        }

        public override void Attack()
        {
            if (target == null || target.currentHealth >= target.maxHealth) return;

            var pos = transform.position;
            pos.y = 0.1f;
            controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, pref_HealArea.name, pos,
                Quaternion.identity, Vector3.one);
            var cols = Physics.OverlapSphere(pos, range, friendlyLayer);
            
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
                foreach (var col in cols)
                {
                    if (col != null && col.CompareTag("Minion_Ground") && col.gameObject != gameObject)
                    {
                        controller.HealMinion(col.GetComponentInParent<Minion>().id, effect);
                    }
                }
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashSkill);
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
            var closeToDistance = 0f;
            var distance = float.MaxValue;
            var oldHp = 1f;

            foreach (var col in cols)
            {
                if (col != null && col.CompareTag($"Minion_Ground") && col.gameObject != gameObject)
                {
                    var m = col.GetComponentInParent<Minion>();
                    var hp = m.currentHealth / m.maxHealth;
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis > closeToDistance && m != null && m.GetType() != typeof(Minion_Healer))
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