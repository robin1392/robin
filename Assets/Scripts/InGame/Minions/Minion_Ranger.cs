using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Ranger : Minion
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
                //controller.photonView.RPC("FireArrow", RpcTarget.All, shootingPos.position, target.id, power);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , shootingPos.position, target.id, power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireArrow(shootingPos.position, target.id, power);
            }
        }
    }
}