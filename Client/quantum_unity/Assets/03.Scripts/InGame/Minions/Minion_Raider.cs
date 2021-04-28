#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Raider : Minion
    {
        [Header("Effect")]
        public GameObject pref_EffectDash;

        [Header("AudioClip")]
        public AudioClip clip_Dash;
        
        private float _skillCastedTime;
        private Transform dashTarget;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_EffectDash, 1);
        }

        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Skill += SkillEvent;
        }

        public override void Initialize()
        {
            base.Initialize();
            _skillCastedTime = -effectCooltime;
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
            if (_spawnedTime >= _skillCastedTime + effectCooltime)
            {
                var cols = Physics.OverlapSphere(ActorProxy.transform.position, 5, targetLayer);
                var distance = float.MaxValue;
                Minion dashTarget = null;
                foreach (var col in cols)
                {
                    if (col.CompareTag("Player")) continue;
                    var m = col.GetComponentInParent<Minion>();
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (dis < distance && (m != null && m.CanBeTarget()))
                    {
                        distance = dis; 
                        dashTarget = m;
                    }
                }

                if (dashTarget != null)
                {
                    _skillCastedTime = _spawnedTime;
                    var action = new DashAction();
                    yield return action.ActionWithSync(ActorProxy, dashTarget.ActorProxy);
                }
            }
        }

        public void SkillEvent()
        {
            SoundManager.instance.Play(clip_Dash);
        }
    }
    
    public class DashAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var raider = (Minion_Raider) actorProxy.baseEntity;
            var t = PoolManager.instance.ActivateObject(raider.pref_EffectDash.name, raider.ts_HitPos.position);
            if (targetActorProxy.transform != null) t.LookAt(targetActorProxy.transform.position);

            raider.animator.SetTrigger(AnimationHash.Skill);

            Transform ts = actorProxy.transform;
            while (targetActorProxy != null && targetActorProxy.baseEntity != null && targetActorProxy.baseEntity.isAlive)
            {
                ts.LookAt(targetActorProxy.transform);
                ts.position += (targetActorProxy.transform.position - ts.position).normalized * (raider.moveSpeed * 5f) * Time.deltaTime;

                if (Vector3.Distance(targetActorProxy.transform.position, ts.position) < raider.range)
                    break;
                
                yield return null;
            }

            if (actorProxy.isPlayingAI == false) yield break;
            
            if (targetActorProxy != null && targetActorProxy.baseEntity != null && targetActorProxy.baseEntity.CanBeTarget())
            {
                targetActorProxy.AddBuff(BuffInfos.Stun, 10f);
            }
        }
    }
}
