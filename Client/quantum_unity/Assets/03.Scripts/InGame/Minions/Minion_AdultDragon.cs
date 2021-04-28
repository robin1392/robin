using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ED
{
    public class Minion_AdultDragon : Minion
    {
        public ParticleSystem ps_Smoke;

        [Header("Prefab")] 
        public GameObject pref_Fireball;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Fireball, 1);
        }

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();

            ae.event_FireSpear += FireSpear;
        }

        public override void Initialize()
        {
            base.Initialize();

            ps_Smoke.Play();
        }

        public void FireSpear()
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
                ActorProxy.FireBulletWithRelay(E_BulletType.BABYDRAGON_BULLET, target, power, 10f);
            }
        }
    }
}