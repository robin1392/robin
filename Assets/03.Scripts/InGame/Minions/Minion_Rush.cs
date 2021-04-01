#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections.Generic;
using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Rush : Minion
    {
        public ParticleSystem ps_Rush;

        [Header("AudioClip")]
        public AudioClip clip_Rush;
        
        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Skill += SkillEvent;
        }

        protected override IEnumerator Root()
        {
            yield return new WaitForSeconds(1f);
            
            // target
            var ts = transform;
            var rush = (Minion_Rush)ActorProxy.baseStat;
            var cols = Physics.OverlapSphere(ts.position, rush.searchRange, rush.targetLayer);
            var distance = 0f;
            Minion dashTarget = null;
           
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                var minion = col.GetComponentInParent<Minion>();
                if (minion == null || minion.isAlive == false)
                {
                    continue;
                }

                var dis = Vector3.Distance(ts.position, minion.transform.position); 
                if (dis > distance)
                {
                    distance = dis;
                    dashTarget = minion;
                }
            }

            if (dashTarget != null)
            {
                var action = new RushAction();
                yield return action.ActionWithSync(ActorProxy, dashTarget.ActorProxy);
            }
            
            while (isAlive)
            {
                target = SetTarget();
                if (target != null)
                {
                    yield return Combat();
                }

                yield return _waitForSeconds0_1;
            }
        }
        
        public void SkillEvent()
        {
            SoundManager.instance.Play(clip_Rush);
        }
    }
    
    
    public class RushAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
#if UNITY_EDITOR
            if (targetActorProxy != null)
            {
                Debug.DrawLine(actorProxy.transform.position + Vector3.up * 0.2f, targetActorProxy.transform.position, Color.red, 2f);
            }
#endif
            var minionRush = actorProxy.baseStat as Minion_Rush;
            minionRush.ps_Rush.Play();
            
            actorProxy.PlayAnimationWithRelay(AnimationHash.SkillLoop, null);

            var ts = actorProxy.transform;
            float tick = 0.1f;
            while (targetActorProxy != null && targetActorProxy.baseStat != null)
            {
                ts.LookAt(targetActorProxy.transform);
                ts.position += (targetActorProxy.transform.position - ts.position).normalized * (minionRush.moveSpeed * 2.5f) * Time.deltaTime;

                if (tick < 0)
                {
                    tick = 0.1f;
                    
                    var hits = Physics.RaycastAll(ts.position + Vector3.up * 0.1f, ts.forward, minionRush.range, minionRush.targetLayer);
                    foreach (var raycastHit in hits)
                    {
                        var bs = raycastHit.collider.GetComponentInChildren<BaseStat>();
                        if (bs != null && bs.isAlive)
                        {
                            if (actorProxy.isPlayingAI)
                            {
                                bs.ActorProxy.HitDamage(minionRush.effect);
                            }

                            PoolManager.Get().ActivateObject("Effect_Stone", raycastHit.point);
                        }
                    } 
                }

                if (Vector3.Distance(targetActorProxy.transform.position, ts.position) <= minionRush.range)
                    break;

                tick -= Time.deltaTime;
                yield return null;
            }
            
            minionRush.ps_Rush.Stop();
            
            actorProxy.PlayAnimationWithRelay(AnimationHash.Idle, null);
        }

        public override void OnActionCancel(ActorProxy actorProxy)
        {
            base.OnActionCancel(actorProxy);
            var minionRush = actorProxy.baseStat as Minion_Rush;
            minionRush.ps_Rush.Stop();
        }
    }
}
