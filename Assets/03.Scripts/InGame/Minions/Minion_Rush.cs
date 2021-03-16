#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ED
{
    public class Minion_Rush : Minion
    {
        //private float _skillCastedTime;
        //private Collider _col;
        public ParticleSystem ps_Rush;

        [Header("AudioClip")]
        public AudioClip clip_Rush;
        
        [SerializeField]
        private Collider dashTarget;

        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Skill += SkillEvent;
        }

        public override void Initialize()
        {
            base.Initialize();

            Skill();
        }
        
        public void Skill()
        {
            //if (_spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                //Dash();
                StartCoroutine(DashCoroutine());
            }
        }

        public void SkillEvent()
        {
            SoundManager.instance.Play(clip_Rush);
        }

        private void Dash()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = float.MaxValue;
            Collider dashTarget = null;
            var hitPoint = Vector3.zero;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                //var dis = Vector3.SqrMagnitude(col.transform.position);
                var transform1 = transform;
                Physics.Raycast(transform1.position + Vector3.up * 0.2f, transform1.forward, out var hit,
                    7f, targetLayer);

                if (hit.collider != null && !hit.collider.CompareTag("Player") && hit.distance < distance)
                {
                    distance = hit.distance;
                    dashTarget = col;
                    hitPoint = hit.point;
                }
            }

#if UNITY_EDITOR
            if (dashTarget != null)
            {
                //_skillCastedTime = _spawnedTime;
                //StartCoroutine(DashCoroutine(dashTarget));
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
#endif
        }

        private IEnumerator DashCoroutine(/*Collider dashTarget*/)
        {
            yield return new WaitForSeconds(1f);
            
            // target
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = 0f;
            dashTarget = null;
            var hitPoint = Vector3.zero;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;

                var dis = Vector3.Distance(transform.position, col.transform.position); 
                if (dis > distance)
                {
                    distance = dis;
                    dashTarget = col;
                }
            }

#if UNITY_EDITOR
            if (dashTarget != null)
            {
                //_skillCastedTime = _spawnedTime;
                //StartCoroutine(DashCoroutine(dashTarget));
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
#endif
            
            
            
            _collider.enabled = false;
            ps_Rush.Play();
            var ts = transform;
            
            ActorProxy.PlayAnimationWithRelay(AnimationHash.SkillLoop, null);

            float tick = 0.1f;
            while (dashTarget != null)
            {
                ts.LookAt(dashTarget.transform);
                ts.position += (dashTarget.transform.position - transform.position).normalized * (moveSpeed * 2.5f) * Time.deltaTime;

                if (tick < 0)
                {
                    tick = 0.1f;
                    
                    var hits = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range, targetLayer);
                    foreach (var raycastHit in hits)
                    {
                        //if (list.Contains(raycastHit.collider) == false)
                        {
                            var bs = raycastHit.collider.GetComponentInParent<BaseStat>();
                            if (bs != null && bs.isAlive)
                            {
                                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, bs.id, effect, 0f);
                                controller.AttackEnemyMinionOrMagic(bs.UID, bs.id, effect, 0f);
                                //controller.HitMinionDamage( true , bs.id , effect);
                                
                                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Stone", raycastHit.point, Quaternion.identity, Vector3.one);
                                controller.ActionActivePoolObject("Effect_Stone", raycastHit.point, Quaternion.identity, Vector3.one);
                            }
                        }
                    } 
                }

                if (Vector3.Distance(dashTarget.transform.position, transform.position) <= range)
                    break;

                tick -= Time.deltaTime;
                yield return null;
            }
            
            
            dashTarget = null;
            _collider.enabled = true;
            ps_Rush.Stop();
            
            ActorProxy.PlayAnimationWithRelay(AnimationHash.Idle, null);
        }

        public override void Sturn(float duration)
        {
            base.Sturn(duration);

            _collider.enabled = true;
            ps_Rush.Stop();
        }

        public override void EndGameUnit()
        {
            base.EndGameUnit();
            
            ps_Rush.Stop();
        }
    }
}
