#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
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
                var action = new BerserkerAction();
                yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
            }

            yield return base.Attack();
        }

        public void SkillEvent()
        {
            SoundManager.instance.Play(clip_Whirl);
        }
    }

    public class BerserkerAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            actorProxy.baseStat.animator.SetTrigger(AnimationHash.Skill);

            yield return new WaitForSeconds(1.5f);

            if (actorProxy.isPlayingAI == false)
            {
                yield break;
            }
            
            var cols = Physics.OverlapSphere(actorProxy.transform.position, 1f, actorProxy.baseStat.targetLayer);
            //Debug.LogFormat("BerserkerSkill: {0}", cols.Length);
            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;

                var m = col.GetComponent<BaseStat>();
                if (m != null && m.isAlive)
                {
                    m.ActorProxy.HitDamage(actorProxy.baseStat.power * 0.3f);
                }
            }
        }
    }
}
