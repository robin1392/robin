﻿#if UNITY_EDITOR
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
        
        public override void Attack()
        {
            if (target == null) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.Get().IsNetwork() && isMine ) || InGameManager.Get().IsNetwork() == false)
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack");
            }
        }

        public void FireSpear()
        {
            if (target == null) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.Get().IsNetwork() && isMine ) || InGameManager.Get().IsNetwork() == false)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRESPEAR , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireSpear(ts_ShootingPos.position, target.id, power , bulletMoveSpeed);
            }
        }
        
        public void FireArrow()
        {
            if (target == null) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if((InGameManager.Get().IsNetwork() && isMine ) || InGameManager.Get().IsNetwork() == false)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
        }
    }
}
