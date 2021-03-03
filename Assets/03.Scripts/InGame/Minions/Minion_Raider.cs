#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
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

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -effectCooltime;
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                base.Attack();

                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);

            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
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
            var bs = InGameManager.Get().GetBaseStatFromId(targetId);
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
            
            //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
            if (dashTarget != null) controller.MinionAniTrigger(id, "Skill", dashTarget.GetComponentInParent<BaseStat>().id);
            
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
                var bs = dashTarget.GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    var targetID = bs.id;
                    //if (PhotonNetwork.IsConnected && isMine)
                    if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI)
                    {
                        //controller.targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_STURNMINION , targetID, 1f);
                        controller.ActionSturn(true , targetID , 1f);
                    }
                    //else if (PhotonNetwork.IsConnected == false)
                    //else if(InGameManager.IsNetwork == false)
                    //{
                        //controller.targetPlayer.SturnMinion(targetID, 1f);
                    //}
                }
            }
        }
    }
}
