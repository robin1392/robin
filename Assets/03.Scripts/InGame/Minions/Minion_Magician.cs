#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
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
        private float _skillCastedTime;

        protected override void Start()
        {
            base.Start();

            _animationEvent.event_FireArrow += FireArrow;
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
            _skillCastedTime = -effectCooltime;
        }

        protected override IEnumerator Combat()
        {
            while (true)
            {
                yield return Skill();
                
                if (!IsTargetInnerRange())
                {
                    ApproachToTarget();    
                }
                else
                {
                    break;
                }

                yield return null;

                target = SetTarget();
            }

            StopApproachToTarget();
            
            if (target == null)
            {
                yield break;
            }

            yield return Attack();
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
                ActorProxy.FireBulletWithRelay(E_BulletType.MAGICIAN_BULLET , target, power, bulletMoveSpeed);
            }
        }

        public IEnumerator Skill()
        {
            if (_spawnedTime >= _skillCastedTime + effectCooltime)
            {
                yield return Polymorph();
            }
        }

        private IEnumerator Polymorph()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var list = new List<BaseEntity>();

            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseEntity>();
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
                _skillCastedTime = _spawnedTime;

                PoolManager.instance.ActivateObject(pref_SkillEffect.name, ts_SkillParticlePosition.position);

                for (int i = 0; i < ActorProxy.diceScale + 1 && i < list.Count; i++)
                {
                    var target = list[i];
                    
                    ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
                    
                    target.ActorProxy.AddBuff(BuffInfos.Scarecrow, effect);
                }
            }

            yield break;
        }
    }
}
