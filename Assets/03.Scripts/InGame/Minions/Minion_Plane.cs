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

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireSpear += FireSpear;
        }

        public void FireSpear()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                SetControllEnable(true);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.SPEAR , target, power , bulletMoveSpeed);
            }
        }
        
        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                SetControllEnable(true);
                return;
            }
            
            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.ARROW , target, power , bulletMoveSpeed);
            }
        }
    }
}
