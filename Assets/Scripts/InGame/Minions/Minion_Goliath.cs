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
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                return;
            }

            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , target.isFlying ? "Attack2" : "Attack1");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(target.isFlying ? "Attack2" : "Attack1");
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
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            if (PhotonNetwork.IsConnected && isMine)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET,
            ts_ShootingPos.position, target.id, target.isFlying ? effect : power, 
                    target.isFlying ? bulletMoveSpeedByFlying : bulletMoveSpeedByGround);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                if (target.isFlying)
                {
                    controller.FireBullet(_spear, ts_ShootingPos2.position, target.id, effect, bulletMoveSpeedByFlying);
                }
                else
                {
                    controller.FireBullet(_arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeedByGround);
                }
            }
        }
        
        public void FireLightOn()
        {
            if (target.isFlying) ps_FireTargetFlying.Play();
        }
    }
}
