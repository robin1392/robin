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
            var action = new RushAction();
            yield return action.ActionWithSync(ActorProxy);
            
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
    
    
    public class RushAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy)
        {
            yield return new WaitForSeconds(1f);
            
            // target
            var ts = actorProxy.transform;
            var rush = (Minion_Rush)actorProxy.baseStat;
            var cols = Physics.OverlapSphere(ts.position, rush.searchRange, rush.targetLayer);
            var distance = 0f;
            Collider dashTarget = null;
            var hitPoint = Vector3.zero;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;

                var dis = Vector3.Distance(ts.position, col.transform.position); 
                if (dis > distance)
                {
                    distance = dis;
                    dashTarget = col;
                }
            }

#if UNITY_EDITOR
            if (dashTarget != null)
            {
                Debug.DrawLine(actorProxy.transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
#endif
            
            rush.ps_Rush.Play();
            
            actorProxy.PlayAnimationWithRelay(AnimationHash.SkillLoop, null);

            float tick = 0.1f;
            while (dashTarget != null)
            {
                ts.LookAt(dashTarget.transform);
                ts.position += (dashTarget.transform.position - ts.position).normalized * (rush.moveSpeed * 2.5f) * Time.deltaTime;

                if (tick < 0)
                {
                    tick = 0.1f;
                    
                    var hits = Physics.RaycastAll(ts.position + Vector3.up * 0.1f, ts.forward, rush.range, rush.targetLayer);
                    foreach (var raycastHit in hits)
                    {
                        //if (list.Contains(raycastHit.collider) == false)
                        {
                            var bs = raycastHit.collider.GetComponentInChildren<BaseStat>();
                            if (bs != null && bs.isAlive)
                            {
                                if (actorProxy.isPlayingAI)
                                {
                                    bs.ActorProxy.HitDamage(rush.effect);
                                }

                                PoolManager.Get().ActivateObject("Effect_Stone", raycastHit.point);
                            }
                        }
                    } 
                }

                if (Vector3.Distance(dashTarget.transform.position, ts.position) <= rush.range)
                    break;

                tick -= Time.deltaTime;
                yield return null;
            }
            
            rush.ps_Rush.Stop();
            
            actorProxy.PlayAnimationWithRelay(AnimationHash.Idle, null);
        }
    }
}
