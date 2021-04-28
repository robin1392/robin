using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;

namespace ED
{
    public class RageOfTower : Magic
    {
        public ParticleSystem ps_Bomb;
        private BaseEntity tower;
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            var rwClient = ActorProxy.Client as RWNetworkClient;
            tower = rwClient.GetTower(ActorProxy.ownerTag);

            if (ps_Bomb != null)
            {
                ps_Bomb.transform.position = tower.transform.position;
                ps_Bomb.Play();    
            }
            
            ActorProxy.Destroy(2f);
        }

        protected override IEnumerator Cast()
        {
            var cols = Physics.OverlapSphere(tower.transform.position, range, targetLayer);
            foreach (var col in cols)
            {
                col.GetComponentInParent<BaseEntity>().ActorProxy.HitDamage(power);
            }
            
            yield break;
        }
    }
}
