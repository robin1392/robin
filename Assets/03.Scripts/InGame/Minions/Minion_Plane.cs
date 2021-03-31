#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Plane : Minion
    {
        public float bulletMoveSpeed = 6f;

        public GameObject pref_Bullet;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow -= FireArrow;
            ae.event_FireSpear -= FireSpear;
            ae.event_FireArrow += FireArrow;
            ae.event_FireSpear += FireSpear;
            PoolManager.instance.AddPool(pref_Bullet, 2);
        }

        public void FireSpear()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.SPEAR_BULLET , target, power , bulletMoveSpeed);
            }
        }
        
        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }
            
            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.ARROW_BULLET , target, power , bulletMoveSpeed);
            }
        }
    }
}
