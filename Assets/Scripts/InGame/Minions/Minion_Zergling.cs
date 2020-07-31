using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Zergling : Minion
    {
        public override void Attack()
        {
            if (target == null) return;
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
    }
}
