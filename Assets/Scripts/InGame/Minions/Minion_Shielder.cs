using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Shielder : Minion
    {
        [SerializeField] private readonly float _skillCooltime = 5f;
        private int _skillCastedCount;
        [SerializeField] private readonly float _skillDuration = 3f;
        private bool isHalfDamage;
        private static readonly int aniHashAttack = Animator.StringToHash("Skill");

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedCount = 0;
        }

        public override void Attack()
        {
            if (target == null || isHalfDamage) return;
            
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
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
                StartCoroutine(SkillCoroutine());

                animator.SetTrigger(aniHashAttack);
                _skillCastedCount++;
            }
        }

        IEnumerator SkillCoroutine()
        {
            rb.velocity = Vector3.zero;
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            isHalfDamage = true;
            isPushing = true;
            yield return new WaitForSeconds(_skillDuration);
            isHalfDamage = false;
            isPushing = false;
        }
        
        public override void HitDamage(float damage, float delay = 0)
        {
            Skill();
            
            if (isHalfDamage) damage *= 0.5f;
            
            base.HitDamage(damage, delay);
        }

        public override BaseStat SetTarget()
        {
            return controller.targetPlayer;
        }
    }
}
