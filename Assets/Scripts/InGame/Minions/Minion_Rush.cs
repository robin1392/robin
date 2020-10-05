#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Rush : Minion
    {
        //private float _skillCastedTime;
        //private Collider _col;
        public ParticleSystem ps_Rush;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            SetControllEnable(false);
            Skill();
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            ps_Rush.Stop();
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void Skill()
        {
            //if (_spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                //Dash();
                StartCoroutine(DashCoroutine());
            }
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

            if (dashTarget != null)
            {
                //_skillCastedTime = _spawnedTime;
                //StartCoroutine(DashCoroutine(dashTarget));
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
        }

        private IEnumerator DashCoroutine(/*Collider dashTarget*/)
        {
            yield return new WaitForSeconds(1f);
            
            // target
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = 0f;
            Collider dashTarget = null;
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

            if (dashTarget != null)
            {
                //_skillCastedTime = _spawnedTime;
                //StartCoroutine(DashCoroutine(dashTarget));
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
            
            
            // dash
            //isPushing = true;
            _collider.enabled = false;
            ps_Rush.Play();
            var ts = transform;
            
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
            controller.MinionAniTrigger(id, "Skill", target.id);
            
            float tick = 0.1f;
            while (dashTarget != null)
            {
                ts.LookAt(dashTarget.transform);
                ts.position += (dashTarget.transform.position - transform.position).normalized * (moveSpeed * 2.5f) * Time.deltaTime;

                if (tick < 0)
                {
                    tick = 0.1f;
                    
                    RaycastHit hit;
                    var hits = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range, targetLayer);
                    foreach (var raycastHit in hits)
                    {
                        //if (list.Contains(raycastHit.collider) == false)
                        {
                            var bs = raycastHit.collider.GetComponentInParent<BaseStat>();
                            if (bs != null && bs.isAlive)
                            {
                                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, bs.id, effect, 0f);
                                controller.AttackEnemyMinionOrMagic(bs.id, effect, 0f);
                                //controller.HitMinionDamage( true , bs.id , effect);
                                
                                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Stone", raycastHit.point, Quaternion.identity, Vector3.one);
                                controller.ActionActivePoolObject("Effect_Stone", raycastHit.point, Quaternion.identity, Vector3.one);
                            }
                        }
                    } 
                }

                if (Vector3.Distance(dashTarget.transform.position, transform.position) < range)
                    break;

                tick -= Time.deltaTime;
                yield return null;
            }
            
            //rb.velocity = Vector3.zero;
            //isPushing = false;
            SetControllEnable(true);
            _collider.enabled = true;
            ps_Rush.Stop();
            
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Idle");
            controller.MinionAniTrigger(id, "Idle" , 0);
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
