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
                RunningAction = action;
                yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
                RunningAction = null;
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
            actorProxy.baseEntity.animator.SetTrigger(AnimationHash.Skill);

            yield return new WaitForSeconds(1.5f);

            if (actorProxy.isPlayingAI == false)
            {
                yield break;
            }
            
            var cols = Physics.OverlapSphere(actorProxy.transform.position, 1f, actorProxy.baseEntity.targetLayer);
            //Debug.LogFormat("BerserkerSkill: {0}", cols.Length);
            foreach (var col in cols)
            {
                var m = col.GetComponentInParent<Minion>();
                if (m != null && m.isAlive)
                {
                    m.ActorProxy.HitDamage(actorProxy.baseEntity.power * 0.3f);
                }
            }
        }
    }
}
