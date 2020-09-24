﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Shielder : Minion
    {
        private float skillCastedTime;
        private bool isHalfDamage;
        private static readonly int aniHashAttack = Animator.StringToHash("Skill");

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            skillCastedTime = -effectCooltime;
        }

        public override void Attack()
        {
            if (target == null || isHalfDamage) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack" , target.id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }
        
        public void Skill()
        {
            if (_spawnedTime >= skillCastedTime + effectCooltime)
            {
                skillCastedTime = _spawnedTime;
                StartCoroutine(SkillCoroutine());

                animator.SetTrigger(aniHashAttack);
            }
        }

        IEnumerator SkillCoroutine()
        {
            //rb.velocity = Vector3.zero;
            //agent.velocity = Vector3.zero;
            //agent.isStopped = true;
            //agent.updatePosition = false;
            //agent.updateRotation = false;
            SetControllEnable(false);
            isHalfDamage = true;
            yield return new WaitForSeconds(effectDuration);
            isHalfDamage = false;
            SetControllEnable(true);
            
        }
        
        public override void HitDamage(float damage)
        {
            Skill();
            
            if (isHalfDamage) damage *= 0.5f;
            
            base.HitDamage(damage);
        }

        public override BaseStat SetTarget()
        {
            return controller.targetPlayer;
        }
    }
}
