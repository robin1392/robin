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

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            attackSpeed = effectCooltime;
            _animationEvent.event_Attack += AttackEvent;
        }

        public override void Death()
        {
            base.Death();

            _animationEvent.event_Attack -= AttackEvent;
        }

        public void AttackEvent()
        {
            SoundManager.instance?.Play(clip_Heal);
        }

        public override IEnumerator Attack()
        {
            _attackedTarget = target;
            ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);
            
            yield return AttackCoroutine();
            controller.HealerMinion(target.id, effect);
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
                    // var hp = m.currentHealth / m.maxHealth;
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis < closeToDistance && m.GetType() != typeof(Minion_Healer))
                    {
                        closeToDistance = dis;
                        closeToTarget = col;
                    }

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
                return controller.targetPlayer;
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
