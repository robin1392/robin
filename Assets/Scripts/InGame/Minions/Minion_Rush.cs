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
        [SerializeField] private readonly float _skillCooltime = 6f;
        private float _skillCastedTime;
        private Collider _col;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -_skillCooltime;
            if (_col == null) _col = GetComponent<Collider>();
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
            if (_spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                Dash();
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
                _skillCastedTime = _spawnedTime;
                StartCoroutine(DashCoroutine(dashTarget));
                Debug.DrawLine(transform.position + Vector3.up * 0.2f, hitPoint, Color.red, 2f);
            }
        }

        private IEnumerator DashCoroutine(Collider dashTarget)
        {
            isPushing = true;
            _col.enabled = false;
            var ts = transform;
            animator.SetTrigger(_animatorHashSkill);
            
            while (true)
            {
                ts.LookAt(dashTarget.transform);
                //rb.MovePosition(transform.position + transform.forward * moveSpeed * 3f);
                ts.position += (dashTarget.transform.position - transform.position).normalized * (moveSpeed * 5f) * Time.deltaTime;

                if (Vector3.Distance(dashTarget.transform.position, transform.position) < 0.7f)
                    break;
                
                //var vel = (dashTarget.transform.position - transform.position).normalized * moveSpeed * 3f;
                //vel.y = 0;
                //rb.velocity = vel;
                yield return null;
                //yield return new WaitForSeconds(moveTime);
            }
            rb.velocity = Vector3.zero;

            isPushing = false;
            _col.enabled = true;
            //dashTarget.GetComponent<Minion>()?.Sturn(1f);

            if (dashTarget != null && dashTarget.gameObject.activeSelf)
            {
                // var targetID = dashTarget.GetComponentInParent<Minion>().id;
                // if (PhotonNetwork.IsConnected && isMine)
                // {
                //     controller.targetPlayer.photonView.RPC("SturnMinion", RpcTarget.All, targetID, 1f);
                // }
                // else if (PhotonNetwork.IsConnected == false)
                // {
                //     controller.targetPlayer.SturnMinion(targetID, 1f);
                // }
                
                DamageToTarget(dashTarget.GetComponentInParent<BaseStat>(), 0, 5f);
            }
        }
    }
}
