#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using UnityEngine;

namespace ED
{
    public class Minion_Necromancer : Minion
    {
        [Header("Prefab")]
        public GameObject pref_Bullet;
        public GameObject pref_Skeleton;
        [Space]
        public float bulletMoveSpeed = 6f;
        public Transform[] arrSpawnPos;
        //public Data_Dice spawnDiceData;
        public ParticleSystem[] arrPs_Spawn;

        [Header("AudioClip")]
        public AudioClip clip_Summon;
        public AudioClip clip_Attack;

        private readonly float _skillCooltime = 10f;
        private float _skillCastedTime;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
            PoolManager.instance.AddPool(pref_Skeleton, 2);
        }

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow -= FireArrow;
            ae.event_FireArrow += FireArrow;
            // ae.event_Skill -= Skill;
            // ae.event_Skill += Skill;
        }

        protected override void Update()
        {
            base.Update();
            
            
        }

        public override void Initialize()
        {
            base.Initialize();
            _skillCastedTime = -_skillCooltime;
        }

        protected override IEnumerator Root()
        {
            while (isAlive)
            {
                yield return Skill();
                
                target = SetTarget();
                if (target != null)
                {
                    yield return Combat();
                }

                yield return _waitForSeconds0_1;
            }
        }

        public void FireArrow()
        {
            if (target == null)
            {
                return;
            }
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(AnimationHash.Idle);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.NECROMANCER, target, power, bulletMoveSpeed);;
            }
            
            SoundManager.instance.Play(clip_Attack);
        }
        
        public IEnumerator Skill()
        {
            if (ActorProxy.isPlayingAI && _spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                _skillCastedTime = _spawnedTime;
                SoundManager.instance.Play(clip_Summon);
                yield return SummonCoroutine();
            }
        }

        public void Summon()
        {
            StartCoroutine(SummonCoroutine());
        }

        IEnumerator SummonCoroutine()
        {
            ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, null);

            yield return new WaitForSeconds(0.4f);

            if (ActorProxy.isPlayingAI)
            {
                var positions = arrSpawnPos.Select(t => t.position).ToArray();
                ActorProxy.CreateActorBy(3012, ActorProxy.ingameUpgradeLevel, ActorProxy.outgameUpgradeLevel,
                    positions);
            }

            for (int i = 0; i < arrSpawnPos.Length; i++)
            {
                arrPs_Spawn[i].Play();
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }
}
