using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ED;
using Mirage;
using UnityEngine;

namespace ED
{
    public class Minion_Ranger : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow -= FireArrow;
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight -= FireLightOn;
            ae.event_FireLight += FireLightOn;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (light_Fire != null) light_Fire.enabled = false;
        }

        public void FireArrow()
        {
            //TODO: 빼도 되지 않을까? 고민해보자
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.ARROW_BULLET, target, power, bulletMoveSpeed);
            }
        }

        public void FireLightOn()
        {
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }

            if (light_Fire != null)
            {
                light_Fire.enabled = true;
                Invoke("FireLightOff", 0.15f);
            }
        }

        private void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }
    }
}

