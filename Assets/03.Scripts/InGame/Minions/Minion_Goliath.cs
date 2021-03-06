#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Goliath : Minion
    {
        public float bulletMoveSpeedByGround = 10f;
        public float bulletMoveSpeedByFlying = 6f;
        public Transform ts_ShootingPos2;
        public ParticleSystem ps_FireTargetFlying;
        
        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
        }

        public override IEnumerator Attack()
        {
            _attackedTarget = target;
            var aniHash = target.isFlying ? AnimationHash.Attack2 : AnimationHash.Attack1; 
            ActorProxy.PlayAnimationWithRelay(aniHash, target);

            yield return AttackCoroutine(attackSpeed);
        }

        public override BaseEntity SetTarget()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            if (cols.Length == 0)
            {
                if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
                {
                    return ActorProxy.GetEnemyTowerOrBossEgg();
                }
                else
                {
                    return null;
                }
            }

            Collider firstTarget = null;
            var distance = float.MaxValue;
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseEntity>();
                var m = bs as Minion;

                if (bs == null || bs.isAlive == false || (m != null && m.isCloacking) || (bs.CompareTag("Minion_Ground") && Vector3.Distance(bs.transform.position, transform.position) > 1.5f))
                {
                    continue;
                }
                
                var sqr = Vector3.SqrMagnitude(transform.position - col.transform.position);
                
                if (sqr < distance)
                {
                    distance = sqr;
                    firstTarget = col;
                }
            }

            if (firstTarget == null && animator != null)
            {
                animator.SetTrigger(AnimationHash.Idle);
            }

            if (firstTarget)
            {
                return firstTarget.GetComponentInParent<BaseEntity>();
            }
            else
            {
                return ActorProxy.GetEnemyTowerOrBossEgg();
            }
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
                var bulletSpeed = target.isFlying ? bulletMoveSpeedByFlying : bulletMoveSpeedByGround;
                ActorProxy.FireBulletWithRelay(E_BulletType.SPEAR_BULLET, target, effect, bulletSpeed);
            }
        }
        
        public void FireLightOn()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                return;
            }

            if (target.isFlying) ps_FireTargetFlying.Play();
        }
    }
}
