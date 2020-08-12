using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Bomber : Minion
    {
        public override void Attack()
        {
            if (target == null) return;
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIRECANNONBALL , ts_ShootingPos.position, target.transform.position, power);
            }
        }
        
        public void Skill()
        {
            
        }
    }
}
