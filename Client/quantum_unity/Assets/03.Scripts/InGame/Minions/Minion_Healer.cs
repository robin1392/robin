using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ED
{
    public class Minion_Healer : Minion
    {
        [Header("Prefab")] public GameObject pref_Heal;

        [Header("AudioClip")]
        public AudioClip clip_Heal;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Heal, 1);
        }

        public override void Initialize()
        {
            base.Initialize();
            //KZSee:
            // attackSpeed = effectCooltime;
            _animationEvent.event_Attack -= AttackEvent;
            _animationEvent.event_Attack += AttackEvent;
        }

        public void AttackEvent()
        {
            SoundManager.instance.Play(clip_Heal);
        }

        public override IEnumerator Attack()
        {
            if (target == null || !IsFriendlyLayer(target.gameObject) || !target.isAlive || target.IsHpFull)
            {
                yield break;
            }
            
            target.ActorProxy.Heal(effect);
            
            yield return base.Attack();
        }

        public override BaseEntity SetTarget()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, friendlyLayer);
            Collider firstTarget = null;
            Collider closeToTarget = null;
            var closeToDistance = float.MaxValue;
            var distance = float.MaxValue;

            foreach (var col in cols)
            {
                if (col != null && col.CompareTag($"Minion_Ground") && col.gameObject != gameObject)
                {
                    var m = col.GetComponentInParent<Minion>();
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis < closeToDistance && m.GetType() != typeof(Minion_Healer))
                    {
                        closeToDistance = dis;
                        closeToTarget = col;
                    }
                    
                    if (!m.IsHpFull && dis < distance)
                    {
                        firstTarget = col;
                        distance = dis;
                    }
                }
            }

            if (firstTarget != null)
            {
                return firstTarget.GetComponentInParent<BaseEntity>();
            }

            if (closeToTarget != null)
            {
                return closeToTarget.GetComponentInParent<BaseEntity>();
            }
            else
            {
                return ActorProxy.GetEnemyTowerOrBossEgg();
            }
        }
    }
}
