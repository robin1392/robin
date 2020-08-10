using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Necromancer : Minion
    {
        public Transform[] arrSpawnPos;
        public Data_Dice spawnDiceData;

        private readonly float _skillCooltime = 10f;
        private float _skillCastedTime;
        private bool _isSkillCasting;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -_skillCooltime;
        }

        public override void Attack()
        {
            if (target == null || _isSkillCasting) return;
            
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
        
        public void FireArrow()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                //controller.photonView.RPC("FireArrow", RpcTarget.All, shootingPos.position, target.id, power);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireArrow(ts_ShootingPos.position, target.id, power);
            }
        }
        
        public void Skill()
        {
            if (_spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                _skillCastedTime = _spawnedTime;
                StartCoroutine(SkillCoroutine());
            }
        }

        IEnumerator SkillCoroutine()
        {
            animator.SetTrigger("Skill");
            _isSkillCasting = true;
            
            yield return new WaitForSeconds(0.6f);

            for (int i = 0; i < arrSpawnPos.Length; i++)
            {
                if (PhotonNetwork.IsConnected && isMine)
                {
                    //controller.photonView.RPC("SpawnSkeleton", RpcTarget.All, arrSpawnPos[i].position);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_SPAWNSKELETON , arrSpawnPos[i].position);
                }
                else
                {
                    controller.SpawnSkeleton(arrSpawnPos[i].position);
                }
            }
            
            yield return new WaitForSeconds(0.3f);
            
            _isSkillCasting = false;
        }
    }
}
