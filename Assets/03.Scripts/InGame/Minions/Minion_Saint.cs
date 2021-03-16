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

        public override void Initialize()
        {
            base.Initialize();
            
            //attackSpeed = effectCooltime;
            healTime = -effectCooltime;
        }

        public override IEnumerator Attack()
        {
            if (target == null || target.IsHpFull || healTime + effectCooltime > _spawnedTime)
            {
                yield break;
            }
            
            var pos = transform.position;
            pos.y = 0.1f;
            
            controller.ActionActivePoolObject(pref_HealArea.name, pos, Quaternion.identity, Vector3.one);
            
            var cols = Physics.OverlapSphere(pos, range, friendlyLayer);
            if (cols.Length > 0)
            {
                //if (PhotonNetwork.IsConnected && isMine)
                if (InGameManager.IsNetwork && isMine)
                {
                    base.Attack();
                    ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
                    
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
                    animator.SetTrigger(AnimationHash.Skill);
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
                    //KZSee:
                    // var hp = m.currentHealth / m.maxHealth;
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis > closeToDistance && m != null && m.GetType() != typeof(Minion_Healer))
                    {
                        closeToDistance = dis;
                        closeToTarget = col;
                    }

                    //KZSee:
                    // if (hp < 1f && dis < distance)
                    // {
                    //     oldHp = hp;
                    //     firstTarget = col;
                    //     distance = dis;
                    // }
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
                switch (Global.PLAY_TYPE.BATTLE)
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
    }
}