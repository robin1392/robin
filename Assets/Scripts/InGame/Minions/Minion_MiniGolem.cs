using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_MiniGolem : Minion
    {
        public override void Attack()
        {
            if (target == null) return;

            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
                controller.MinionAniTrigger(id, "Attack");
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }
    }
}