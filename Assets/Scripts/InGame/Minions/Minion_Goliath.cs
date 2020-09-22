#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

        public override void Attack()
        {
            if (target == null)
            {
                return;
            }
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , target.isFlying ? "Attack2" : "Attack1");
                controller.MinionAniTrigger(id, target.isFlying ? "Attack2" : "Attack1");
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                base.Attack();
                animator.SetTrigger(target.isFlying ? "Attack2" : "Attack1");
            }
        }

        public override BaseStat SetTarget()
        {
            if (isPushing)
            {
                return null;
            }
            
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            if (cols.Length == 0)
            {
                if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
                {
                    return controller.targetPlayer;
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
                var bs = col.GetComponentInParent<BaseStat>();
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
                animator.SetTrigger(_animatorHashIdle);
            }

            return firstTarget ? firstTarget.GetComponentInParent<BaseStat>() : controller.targetPlayer;
        }

        public void FireArrow()
        {
            if (target == null)
            {
                return;
            }
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            if ((InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false)
            {
                if (target.isFlying)
                    controller.ActionFireBullet(_spear, ts_ShootingPos2.position, target.id, effect, bulletMoveSpeedByFlying);
                else 
                    controller.ActionFireBullet(_arrow, ts_ShootingPos2.position, target.id, power, bulletMoveSpeedByGround);
            }
            
            /*//if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET,
                    //ts_ShootingPos.position, target.id, target.isFlying ? effect : power, 
                    //target.isFlying ? bulletMoveSpeedByFlying : bulletMoveSpeedByGround);

                    if (target.isFlying)
                        controller.ActionFireBullet(_spear, ts_ShootingPos2.position, target.id, effect, bulletMoveSpeedByFlying);
                    else 
                        controller.ActionFireBullet(_arrow, ts_ShootingPos2.position, target.id, power, bulletMoveSpeedByGround);
                                    
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                if (target.isFlying)
                {
                    controller.FireBullet(_spear, ts_ShootingPos2.position, target.id, effect, bulletMoveSpeedByFlying);
                }
                else
                {
                    controller.FireBullet(_arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeedByGround);
                }
            }*/
            
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
