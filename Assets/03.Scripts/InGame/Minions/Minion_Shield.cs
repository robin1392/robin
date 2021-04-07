using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Shield : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_ShieldMode;
        
        private float skillCastedTime;
        

        public override void Initialize()
        {
            base.Initialize();
            skillCastedTime = -effectCooltime;
        }
        
        protected override IEnumerator Combat()
        {
            while (true)
            {
                yield return Skill();

                if (!IsTargetInnerRange())
                {
                    ApproachToTarget();
                }
                else
                {
                    break;
                }

                yield return null;

                target = SetTarget();
            }

            StopApproachToTarget();

            if (target == null)
            {
                yield break;
            }

            yield return Attack();
        }

        public IEnumerator Skill()
        {
            if (_spawnedTime >= skillCastedTime + effectCooltime)
            {
                var targets = new List<Minion>();
                var skillDistance = (ActorProxy as DiceActorProxy).diceInfo.range;
                var cols = Physics.OverlapSphere(transform.position, skillDistance, targetLayer);
                foreach (var col in cols)
                {
                    var minion = col.GetComponentInParent<Minion>();
                    if(minion != null && minion.CanBeTarget())
                    {
                        targets.Add(minion);
                    }
                }

                if (targets.Any() == false)
                {
                    yield break;
                }
                
                var duration = 5;//effectDuration;
                ActorProxy.AddBuff(BuffInfos.HalfDamage, duration);

                foreach (var target in targets)
                {
                    target.ActorProxy.AddBuffWithNetId(BuffInfos.Taunted, ActorProxy.NetId, duration);
                }
                
                var action = new ShieldAction();
                yield return action.ActionWithSync(ActorProxy);

                skillCastedTime = _spawnedTime;
            }
        }

        public override void OnHitDamageOnClient(float damage)
        {
        }
    }
}
