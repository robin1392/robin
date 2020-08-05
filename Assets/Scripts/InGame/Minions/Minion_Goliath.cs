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
        public override void Attack()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireArrow()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                //controller.photonView.RPC(target.isFlying ? "FireSpear" : "FireArrow", RpcTarget.All, 
                    //shootingPos.position, target.id, target.isFlying ? power * 1.5f : power);
                controller.SendPlayer(RpcTarget.All , target.isFlying?E_PTDefine.PT_FIRESPEAR:E_PTDefine.PT_FIREARROW,
                    shootingPos.position, target.id, target.isFlying ? power * 1.5f : power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                if (target.isFlying)
                {
                    controller.FireSpear(shootingPos.position, target.id, power * 1.5f);
                }
                else
                {
                    controller.FireArrow(shootingPos.position, target.id, power);
                }
            }
        }
    }
}
