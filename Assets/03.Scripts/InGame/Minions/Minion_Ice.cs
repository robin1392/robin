#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ED
{
    public class Minion_Ice : Minion
    {
        [Header("Prefab")]
        public GameObject pref_Bullet;
        [Space]
        public float bulletMoveSpeed = 6f;

        [Header("AudioClip")]
        public AudioClip clip_Attack;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow -= FireArrow;
            ae.event_FireArrow += FireArrow;
        }

        public void FireArrow()
        {
            if (target == null)
            {
                return;
            }
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                var diceActorProxy = ActorProxy as DiceActorProxy;
                if (Random.Range(0, 100) < diceActorProxy.diceInfo.effectDuration)
                {
                    ActorProxy.FireBulletWithRelay(E_BulletType.ICE_FREEZE_BULLET, target, power, bulletMoveSpeed, diceActorProxy.effect);
                }
                else
                {
                    ActorProxy.FireBulletWithRelay(E_BulletType.ICE_NORMAL_BULLET, target, power, bulletMoveSpeed);
                }
            }
            
            SoundManager.instance.Play(clip_Attack);
        }
    }
}
