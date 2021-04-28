using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Bomber : Minion
    {
        public GameObject pref_Bullet;
        public ParticleSystem ps_Fire;
        
        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        protected override void Start()
        {
            base.Start();
            
            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLight;
        }

        public void FireArrow()
        {
            if (target != null)
            {
                if(ActorProxy.isPlayingAI)
                {
                    ActorProxy.FireCannonBallWithRelay(E_CannonType.BOMBER, target.transform.position);
                }
            }
        }

        public void FireLight()
        {
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }
        }
    }
}
