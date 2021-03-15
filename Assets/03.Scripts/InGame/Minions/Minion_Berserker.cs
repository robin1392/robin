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
        public ParticleSystem ps_Wind;

        [Header("AudioClip")]
        public AudioClip clip_Whirl;
        
        private float _skillCastedTime;

        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Skill += SkillEvent;
        }

        public override void Initialize()
        {
            base.Initialize();
            _skillCastedTime = -effectCooltime;
        }

        public override IEnumerator Attack()
        {
            if (_spawnedTime >= _skillCastedTime + effectCooltime)
            {
                _skillCastedTime = _spawnedTime;
                yield return SkillCoroutine();
            }

            yield return base.Attack();
        }

        public void SkillEvent()
        {
            SoundManager.instance.Play(clip_Whirl);
        }

        IEnumerator SkillCoroutine()
        {
            animator.SetTrigger(_animatorHashSkill);

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
        }
    }
}
