using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Saint : Minion
    {
        private float healTime;

        public override void Initialize()
        {
            base.Initialize();
            
            healTime = -effectCooltime;
        }

        public override IEnumerator Attack()
        {
            if (target == null || _spawnedTime < healTime + effectCooltime)
            {
                yield break;
            }
            
            var pos = transform.position;
            pos.y = 0.1f;
            
            var cols = Physics.OverlapSphere(pos, range, friendlyLayer);
            if (cols.Length > 0)
            {
                ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);

                if (ActorProxy.isPlayingAI)
                {
                    foreach (var col in cols)
                    {
                        if (col != null && col.CompareTag("Minion_Ground") && col.gameObject != gameObject)
                        {
                            var minion = col.GetComponentInParent<Minion>();
                            if (minion != null)
                            {
                                minion.ActorProxy.Heal(effect);   
                            }
                        }
                    }   
                }
                healTime = _spawnedTime;

                yield return new WaitForSeconds(attackSpeed);
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
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis > closeToDistance && m != null && m.GetType() != typeof(Minion_Healer) && m.GetType() != typeof(Minion_Saint))
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
                return ActorProxy.GetEnemyTower();
            }
        }
    }
}