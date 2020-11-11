using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Saint : Minion
    {
        public GameObject pref_HealArea;

        private float healTime;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_HealArea, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            
            //attackSpeed = effectCooltime;
            healTime = -effectCooltime;
        }

        public override void Attack()
        {
            if (target == null || target.currentHealth >= target.maxHealth || healTime + effectCooltime > _spawnedTime) return;

            var pos = transform.position;
            pos.y = 0.1f;
            
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, pref_HealArea.name, pos, Quaternion.identity, Vector3.one);
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Heal", transform.position, Quaternion.identity, new Vector3(1.5f, 1.5f, 0.3f));
            controller.ActionActivePoolObject(pref_HealArea.name, pos, Quaternion.identity, Vector3.one);
            
            var cols = Physics.OverlapSphere(pos, range, friendlyLayer);
            if (cols.Length > 0)
            {
                //if (PhotonNetwork.IsConnected && isMine)
                if (InGameManager.IsNetwork && isMine)
                {
                    base.Attack();
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
                    controller.MinionAniTrigger(id, "Skill", target.id);
                    
                    foreach (var col in cols)
                    {
                        if (col != null && col.CompareTag("Minion_Ground") && col.gameObject != gameObject)
                        {
                            controller.HealMinion(col.GetComponentInParent<Minion>().id, effect);
                        }
                    }
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if (InGameManager.IsNetwork == false)
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

                healTime = _spawnedTime;
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

            if (closeToTarget != null)
            {
                return closeToTarget.GetComponentInParent<BaseStat>();
            }
            else
            {
                switch (NetworkManager.Get().playType)
                {
                    case Global.PLAY_TYPE.BATTLE:
                        return controller.targetPlayer;
                    case Global.PLAY_TYPE.COOP:
                        return controller.coopPlayer;
                    default:
                        return null;
                }
            }
        }

        public override void SetAnimationTrigger(string triggerName, int targetID)
        {
            var target = controller.GetBaseStatFromId(targetID);
            transform.LookAt(target.transform);
            
            animator.SetTrigger(triggerName);
        }
    }
}