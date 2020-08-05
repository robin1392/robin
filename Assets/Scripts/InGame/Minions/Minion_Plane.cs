﻿using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Plane : Minion
    {
        public override void Attack()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireSpear()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                controller.photonView.RPC("FireSpear", RpcTarget.All, shootingPos.position, target.id, power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireSpear(shootingPos.position, target.id, power);
            }
        }
    }
}
