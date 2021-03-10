using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Posu : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public GameObject pref_Bullet;

        [Header("AudioClip")]
        public AudioClip clip_Fire;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            if (light_Fire != null) light_Fire.enabled = false;
        }

        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                SetControllEnable(true);
                return;
            }

            SoundManager.instance?.Play(clip_Fire);

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.POSU_BULLET, target, power, bulletMoveSpeed);
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