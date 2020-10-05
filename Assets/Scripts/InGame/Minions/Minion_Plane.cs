#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using Photon.Pun;

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

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.IsNetwork && isMine ) || InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
        }

        public void FireSpear()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            //if (target == null) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.IsNetwork && isMine ) || InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, _spear, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRESPEAR , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.ActionFireSpear(ts_ShootingPos.position, target.id, power , bulletMoveSpeed);
                controller.ActionFireBullet(_spear ,ts_ShootingPos.position, target.id, power , bulletMoveSpeed);
            }
        }
        
        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.IsNetwork && isMine ) || InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, _arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireBullet(_arrow ,ts_ShootingPos.position, target.id, power , bulletMoveSpeed);
            }
        }
    }
}
