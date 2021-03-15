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
            while (true )
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

                    if (!col.CompareTag("Player") && dis < distance && (m != null && m.CanBeTarget()))
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

        protected void Dash()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = float.MaxValue;
            Collider dashTarget = null;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                //var dis = Vector3.SqrMagnitude(col.transform.position);
                var transform1 = transform;
                // Physics.Raycast(transform1.position + Vector3.up * 0.2f,
                //     transform1.forward,
                //     out var hit,
                //     7f,
                //     targetLayer);

                //if (col.collider != null)
                {
                    var m = col.GetComponentInParent<Minion>();
                    var dis = Vector3.Distance(transform.position, col.transform.position);

                    if (!col.CompareTag("Player") &&
                        dis < distance &&
                        (m != null && m.isCloacking == false))
                    {
                        distance = dis; 
                        dashTarget = col;
                    }
                }
            }

            if (dashTarget != null)
            {
                _skillCastedTime = _spawnedTime;
                StartCoroutine(DashCoroutine(dashTarget.transform));

                
                //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEPARAM1, id, "DashMessage", dashTarget.GetComponentInParent<BaseStat>().id);
                controller.ActionSendMsg(id, "DashMessage", dashTarget.GetComponentInParent<BaseStat>().id);
                
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, dashTarget.transform.position + Vector3.up * 0.2f, Color.red, 2f);
            }
        }

        public void DashMessage(uint targetId)
        {
            var bs =  ActorProxy.GetBaseStatWithNetId(targetId);
            if (bs != null)
            {
                Transform ts = bs.transform;
                StartCoroutine(DashCoroutine(ts)); 
            }
        }

        private IEnumerator DashCoroutine(Transform dashTarget)
        {
            var t = PoolManager.instance.ActivateObject(pref_EffectDash.name, ts_HitPos.position);
            if (dashTarget != null) t.LookAt(dashTarget.position);
            isPushing = true;
            animator.SetTrigger(_animatorHashSkill);

            if (dashTarget != null)
            {
                ActorProxy.PlayAnimationWithRelay(_animatorHashSkill, dashTarget.GetComponentInParent<BaseStat>());    
            }
            

            var ts = transform;
            while (dashTarget != null)
            {
                ts.LookAt(dashTarget);
                ts.position += (dashTarget.position - transform.position).normalized * (moveSpeed * 5f) * Time.deltaTime;

                if (Vector3.Distance(dashTarget.position, transform.position) < range)
                    break;
                
                yield return null;
            }

            isPushing = false;

            if (dashTarget != null && dashTarget.gameObject.activeSelf)
            {
                var bs = dashTarget.GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    var targetID = bs.id;
                    if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI)
                    {
                        controller.ActionSturn(true , targetID , 1f);
                    }
                }
            }
        }
    }
    
    public class DashAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var raider = (Minion_Raider) actorProxy.baseStat;
            var t = PoolManager.instance.ActivateObject(raider.pref_EffectDash.name, raider.ts_HitPos.position);
            if (targetActorProxy.transform != null) t.LookAt(targetActorProxy.transform.position);

            raider.animator.SetTrigger(Minion._animatorHashSkill);

            Transform ts = raider.transform;
            while (targetActorProxy != null && targetActorProxy.baseStat.isAlive)
            {
                ts.LookAt(targetActorProxy.transform);
                ts.position += (targetActorProxy.transform.position - ts.position).normalized * (raider.moveSpeed * 5f) * Time.deltaTime;

                if (Vector3.Distance(targetActorProxy.transform.position, ts.position) < raider.range)
                    break;
                
                yield return null;
            }

            if (actorProxy.isPlayingAI == false) yield break;
            
            if (targetActorProxy != null && targetActorProxy.baseStat.CanBeTarget())
            {
                targetActorProxy.AddBuff(new ActorProxy.Buff()
                {
                    id = BuffInfos.Sturn,
                    endTime = (float)actorProxy.NetworkTime.Time + 1f,
                });
            }
        }
    }
}
