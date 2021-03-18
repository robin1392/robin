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
        private BaseStat tower;
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            var rwClient = ActorProxy.Client as RWNetworkClient;
            tower = rwClient.GetTower(ActorProxy.ownerTag);
            
            ps_Bomb.transform.position = tower.transform.position;
            ps_Bomb.Play();
            ActorProxy.Destroy(2f);
        }

        protected override IEnumerator Cast()
        {
            var cols = Physics.OverlapSphere(tower.transform.position, range, targetLayer);
            foreach (var col in cols)
            {
                col.GetComponentInParent<BaseStat>().ActorProxy.HitDamage(power);
            }
            
            yield break;
        }
    }
}
