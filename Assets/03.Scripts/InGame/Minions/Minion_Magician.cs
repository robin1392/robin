#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  ED
{
    public class Minion_Magician : Minion
    {
        public float bulletMoveSpeed = 6f;
        public Transform ts_SkillParticlePosition;

        [Header("Effect prefab")] 
        public GameObject pref_Bullet;
        public GameObject pref_SkillEffect;
        public GameObject pref_Scarecrow;
        
        //[SerializeField] private readonly float _skillCooltime = 10f;
        private int _skillCastedCount;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
        }

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
            PoolManager.instance.AddPool(pref_SkillEffect, 1);
            PoolManager.instance.AddPool(pref_Scarecrow, 1);
        }

        public override void Initialize()
        {
            base.Initialize();
            _skillCastedCount = 0;
        }

        public void FireArrow()
        {

            if (target == null || target.isAlive == false || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.MAGICIAN , target, power, bulletMoveSpeed);
            }
        }

        public void Skill()
        {
            if (_spawnedTime >= effectCooltime * _skillCastedCount)
            {
                Polymorph();
            }
        }

        private void Polymorph()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var list = new List<BaseStat>();

            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null && bs.id > 0 && bs.isFlying == false)
                {
                    var m = bs as Minion;
                    if (m != null && m.isPolymorph == false)
                    {
                        list.Add(bs);
                    }
                }
            }

            if (list.Count > 0)
            {
                _skillCastedCount++;

                PoolManager.instance.ActivateObject(pref_SkillEffect.name, ts_SkillParticlePosition.position);

                var target = list[Random.Range(0, list.Count)];
                
                ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
                
                controller.ActionMinionScareCrow(true, target.id, (float) eyeLevel);

            }
        }
    }
}
