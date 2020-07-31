using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Thunder : Magic
    {
        public Transform ts_ShootPoint;

        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            
            SetColor();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == targetLayer)
            {
                
            }
        }

        private void Shoot(BaseStat longTarget)
        {
            //StartCoroutine(MoveBall(longTarget));
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}