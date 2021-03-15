using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Shielder : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_ShieldMode;
        
        private float skillCastedTime;
        private bool isHalfDamage;
        private static readonly int aniHashSkill = Animator.StringToHash("Skill");

        public override void Initialize()
        {
            base.Initialize();
            skillCastedTime = -effectCooltime;
        }

        public void Skill()
        {
            if (_spawnedTime >= skillCastedTime + effectCooltime)
            {
                skillCastedTime = _spawnedTime;
                StartCoroutine(SkillCoroutine());

                SoundManager.instance.Play(clip_ShieldMode);
                animator.SetTrigger(aniHashSkill);
            }
        }

        IEnumerator SkillCoroutine()
        {
            //rb.velocity = Vector3.zero;
            //agent.velocity = Vector3.zero;
            //agent.isStopped = true;
            //agent.updatePosition = false;
            //agent.updateRotation = false;
            isHalfDamage = true;
            yield return new WaitForSeconds(effectDuration);
            isHalfDamage = false;
        }
        
        public override void HitDamage(float damage)
        {
            Skill();
            
            if (isHalfDamage) damage *= 0.5f;
            
            // base.HitDamage(damage);
        }

        public override BaseStat SetTarget()
        {
            switch (Global.PLAY_TYPE.BATTLE)
            {
                case Global.PLAY_TYPE.BATTLE:
                    return controller.targetPlayer;
                case Global.PLAY_TYPE.COOP:
                    return controller.coopPlayer;
                default:
                    return null;
            }
        }
    }
}
