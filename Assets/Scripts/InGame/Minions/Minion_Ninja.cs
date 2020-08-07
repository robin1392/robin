#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Ninja : Minion
    {
        [SerializeField] private readonly float _skillCooltime = 6f;
        private int _skillCastedCount;
        [SerializeField] private readonly float _skillDuration = 2f;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedCount = 0;
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
            if (_spawnedTime >= _skillCooltime * _skillCastedCount)
            {            
                MoveBack();
                Invincibility(_skillDuration);
            }
        }

        private void MoveBack()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var distance = 0f;
            Collider longTarget = null;
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;

                var dis = Vector3.SqrMagnitude(col.transform.position - transform.position);
                if (dis > distance)
                {
                    distance = dis;
                    longTarget = col;
                }
            }

            if (longTarget != null)
            {
                var targetPos = longTarget.transform.position + (-longTarget.transform.forward * 0.4f);

                if (PhotonNetwork.IsConnected && isMine)
                {
                    //controller.photonView.RPC("TeleportMinion", RpcTarget.All, id, targetPos.x, targetPos.z);
                    controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_TELEPORTMINION , id, targetPos.x, targetPos.z);
                    controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONINVINCIBILITY, id, 2f);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    controller.TeleportMinion(id, targetPos.x, targetPos.z);
                    Invincibility(2f);
                }
                transform.LookAt(longTarget.transform);
                
                _skillCastedCount++;
            }
        }
    }
}
