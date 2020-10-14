#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Ninja : Minion
    {
        //[Header("Effect")] 
        //public GameObject pref_Effect;

        protected override void Awake()
        {
            base.Awake();
            
            //PoolManager.instance.AddPool(pref_Effect, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) ||  InGameManager.IsNetwork == false )
            {
                Skill();
            }
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
            MoveForward();
        }

        private void MoveForward()
        {
            // var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            // var distance = 0f;
            // Collider longTarget = null;
            // foreach (var col in cols)
            // {
            //     if (col.CompareTag("Player")) continue;
            //
            //     var dis = Vector3.SqrMagnitude(col.transform.position - transform.position);
            //     if (dis > distance)
            //     {
            //         distance = dis;
            //         longTarget = col;
            //     }
            // }
            //
            // if (longTarget != null)
            // {
            //     var targetPos = longTarget.transform.position + (-longTarget.transform.forward * 0.4f);
            //
            //     if (PhotonNetwork.IsConnected && isMine)
            //     {
            //         //controller.photonView.RPC("TeleportMinion", RpcTarget.All, id, targetPos.x, targetPos.z);
            //         controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_TELEPORTMINION , id, targetPos.x, targetPos.z);
            //         controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONINVINCIBILITY, id, 2f);
            //     }
            //     else if (PhotonNetwork.IsConnected == false)
            //     {
            //         controller.TeleportMinion(id, targetPos.x, targetPos.z);
            //         Invincibility(2f);
            //     }
            //     transform.LookAt(longTarget.transform);
            // }

            StartCoroutine(MoveForwardCoroutine());
        }

        IEnumerator MoveForwardCoroutine()
        {
            SetControllEnable(false);
            //PoolManager.instance.ActivateObject("Effect_Cloaking", transform.position);
            transform.LookAt(transform.position + (isBottomPlayer ? Vector3.forward : Vector3.back));
            
            yield return null;
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, id, true);
            controller.ActionCloacking(id, true);
            
            animator.SetFloat("MoveSpeed", 1f);
            
            //agent.SetDestination(transform.position + (isBottomPlayer ? Vector3.forward : Vector3.back) * 5f);
            float t = 0;
            while (t < effectDuration)
            {
                transform.position += (isBottomPlayer ? Vector3.forward : Vector3.back) * moveSpeed * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
            
            //yield return new WaitForSeconds(effectCooltime);

            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, id, false);
            controller.ActionCloacking(id, false);
            
            SetControllEnable(true);
        }
    }
}
