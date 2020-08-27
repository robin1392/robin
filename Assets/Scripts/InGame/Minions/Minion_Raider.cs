#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Raider : Minion
    {
        [Header("Effect")]
        public GameObject pref_EffectDash;
        
        private float _skillCastedTime;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_EffectDash, 1);
        }
        
        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -effectCooltime;
        }

        public override void Attack()
        {
            if (target == null) return;
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void Skill()
        {
            if (_spawnedTime >= _skillCastedTime + effectCooltime)
            {
                Dash();
            }
        }

        protected void Dash()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = float.MaxValue;
            Collider dashTarget = null;
            Vector3 hitPoint = Vector3.zero;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                //var dis = Vector3.SqrMagnitude(col.transform.position);
                var transform1 = transform;
                Physics.Raycast(transform1.position + Vector3.up * 0.2f, transform1.forward, out var hit,
                    7f, targetLayer);

                if (hit.collider != null && !hit.collider.CompareTag("Player") 
                    /*&& hit.collider.gameObject != target.gameObject */&& hit.distance < distance)
                {
                    distance = hit.distance;
                    dashTarget = col;
                    hitPoint = hit.point;
                }
            }

            if (dashTarget != null)
            {
                _skillCastedTime = _spawnedTime;
                StartCoroutine(DashCoroutine(dashTarget.transform));
                controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEPARAM1, id, "DashMessage", dashTarget.GetComponentInParent<BaseStat>().id);
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
        }

        public void DashMessage(int targetID)
        {
            Transform ts = controller.targetPlayer.GetBaseStatFromId(targetID).transform;
            StartCoroutine(DashCoroutine(ts));
        }

        private IEnumerator DashCoroutine(Transform dashTarget)
        {
            PoolManager.instance.ActivateObject("Effect_Dash", ts_HitPos.position);
            isPushing = true;
            animator.SetTrigger(_animatorHashSkill);
            controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
            var ts = transform;
            
            while (dashTarget != null)
            {
                ts.LookAt(dashTarget);
                //rb.MovePosition(transform.position + transform.forward * moveSpeed * 3f);
                ts.position += (dashTarget.position - transform.position).normalized * (moveSpeed * 5f) * Time.deltaTime;

                if (Vector3.Distance(dashTarget.position, transform.position) < range)
                    break;
                
                //var vel = (dashTarget.transform.position - transform.position).normalized * moveSpeed * 3f;
                //vel.y = 0;
                //rb.velocity = vel;
                yield return null;
                //yield return new WaitForSeconds(moveTime);
            }
            rb.velocity = Vector3.zero;

            isPushing = false;
            //dashTarget.GetComponent<Minion>()?.Sturn(1f);

            if (dashTarget != null && dashTarget.gameObject.activeSelf)
            {
                var targetID = dashTarget.GetComponentInParent<BaseStat>().id;
                if (PhotonNetwork.IsConnected && isMine)
                {
                    //controller.targetPlayer.photonView.RPC("SturnMinion", RpcTarget.All, targetID, 1f);
                    controller.targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_STURNMINION , targetID, 1f); 
                        
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    controller.targetPlayer.SturnMinion(targetID, 1f);
                }
            }
        }
    }
}
