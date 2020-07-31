using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class RageOfTower : Magic
    {
        public ParticleSystem ps_Bomb;
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);
            
            Fire();
        }

        private void Fire()
        {
            ps_Bomb.transform.position = controller.transform.position;
            ps_Bomb.Play();

            var cols = Physics.OverlapSphere(controller.transform.position, 3f, targetLayer);
            foreach (var col in cols)
            {
                DamageToTarget(col.GetComponentInParent<Minion>());
            }
            
            Destroy(1f);
        }

        public override void SetTarget() { }
    }
}
