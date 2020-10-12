#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Berserker : Minion
    {
        private float _skillCastedTime;
        private bool _isSkillCasting;

        public ParticleSystem ps_Wind;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -effectCooltime;
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false || _isSkillCasting) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack" , target.id);
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
            if (_spawnedTime >= _skillCastedTime + effectCooltime)
            {
                _skillCastedTime = _spawnedTime;
                StartCoroutine(SkillCoroutine());
            }
        }

        IEnumerator SkillCoroutine()
        {
            animator.SetTrigger(_animatorHashSkill);
            _isSkillCasting = true;
            SetControllEnable(false);
            
            yield return new WaitForSeconds(0.6f);
            
            ps_Wind.Play();
            var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
            //Debug.LogFormat("BerserkerSkill: {0}", cols.Length);
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;

                var m = col.GetComponent<BaseStat>();
                if (m != null && m.isAlive)
                {
                    DamageToTarget(m, 0, 0.3f);
                }
            }
            
            _isSkillCasting = false;
            SetControllEnable(true);
        }
    }
}
