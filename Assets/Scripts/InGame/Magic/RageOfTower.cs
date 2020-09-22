using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class RageOfTower : Magic
    {
        public ParticleSystem ps_Bomb;
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            Fire();
        }

        private void Fire()
        {
            ps_Bomb.transform.position = controller.transform.position;
            ps_Bomb.Play();

            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false)
            {
                var cols = Physics.OverlapSphere(controller.transform.position, range, targetLayer);
                foreach (var col in cols)
                {
                    DamageToTarget(col.GetComponentInParent<Minion>());
                }
            }

            Destroy(2f);
        }

        public override void SetTarget() { }
    }
}
