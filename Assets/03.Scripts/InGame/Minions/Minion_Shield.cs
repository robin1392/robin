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
                //Hack: EffectDuration이지만 성장치 적용을 위해 스킬범위로 사용중
                var skillDistance = (ActorProxy as DiceActorProxy).diceInfo.effectDuration; 
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
                
                //TODO: 호스트모드 플레이 시 AddBuff에서 CC가 추가되면 AI를 재시작한다. 따라서 재귀가 반복될 수 있다. 다른 대안을 강구할 것.
                skillCastedTime = _spawnedTime;

                var duration = ActorProxy.effect;
                ActorProxy.AddBuff(BuffInfos.HalfDamage, duration);

                foreach (var target in targets)
                {
                    target.ActorProxy.AddBuffWithNetId(BuffInfos.Taunted, ActorProxy.NetId, duration);
                }
                
                var action = new ShieldAction();
                yield return action.ActionWithSync(ActorProxy);
            }
        }

        public override void OnHitDamageOnClient(float damage)
        {
        }
    }
}
