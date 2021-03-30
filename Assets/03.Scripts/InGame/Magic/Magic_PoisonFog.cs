using System;
using System.Collections;
using Pathfinding.Util;
using UnityEngine;

namespace ED.SummonActor
{
    public class Magic_PoisonFog : Magic
    {
        private const float lifeTime = 5.0f; 
        private Coroutine _ai;

        public override void StartAI()
        {
            _ai = StartCoroutine(Root());
        }
        
        public override void StopAI()
        {
            if (_ai != null)
            {
                StopCoroutine(_ai);
                _ai = null;
            }
        }

        private IEnumerator Root()
        {
            while (ActorProxy.spawnTime + lifeTime > ActorProxy.NetworkTime.Time)
            {
                var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
                    
                foreach (var col in cols)
                {
                    if (col.CompareTag("Player")) continue;

                    var bs = col.transform.GetComponentInParent<BaseStat>();

                    if (bs == null || bs.CanBeTarget() == false ||  bs.id == id) continue;

                    bs.ActorProxy.HitDamage(power);
                }
                
                yield return new WaitForSeconds(0.1f);
            }   
            
            ActorProxy.Destroy();
        }
    }
}