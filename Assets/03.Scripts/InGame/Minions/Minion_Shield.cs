using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        public bool halfDamage = false;
        public PoolObjectAutoDeactivate effectShield;
        

        public override void Initialize()
        {
            base.Initialize();
            skillCastedTime = -effectCooltime;
            halfDamage = false;
        }
        
        protected override IEnumerator Combat()
        {
            yield return _waitForSeconds0_1;
            
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

            if (target == null || target.CanBeTarget() == false)
            {
                yield break;
            }

            yield return Attack();
        }

        async UniTask Debug(Vector3 position, float radius)
        {
            //TODO: 와이어로 변경 후 반경 디버깅해보자. 가끔 멀리있는 적이 도발에 걸리는 경우가 있는 것 같다.
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = position;
            s.transform.localScale = Vector3.one * radius * 2;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            Destroy(s);
        }
        
        public IEnumerator Skill()
        {
            if (_spawnedTime >= skillCastedTime + effectCooltime)
            {
                var targets = new List<Minion>();
                //Hack: EffectDuration이지만 성장치 적용을 위해 스킬범위로 사용중
                var skillDistance = (ActorProxy as DiceActorProxy).diceInfo.effectDurationTime; 
                var cols = Physics.OverlapSphere(transform.position, skillDistance, targetLayer);
                // Debug(transform.position, skillDistance).Forget();
                
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

                foreach (var target in targets)
                {
                    target.ActorProxy.AddBuffWithNetId(BuffInfos.Taunted, ActorProxy.NetId, duration);
                }
                
                var action = new ShieldAction();
                yield return action.ActionWithSync(ActorProxy);
            }
        }

        public override bool OnBeforeHitDamage(float damage)
        {
            return base.OnBeforeHitDamage(damage);
        }

        public override float ModifyDamage(float damage)
        {
            if (halfDamage)
            {
                return damage / 2;
            }
            
            return base.ModifyDamage(damage);
        }

        public override void OnHitDamageOnClient(float damage)
        {
        }
    }
}
