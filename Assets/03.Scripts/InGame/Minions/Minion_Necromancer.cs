#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
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
        private bool _isSkillCasting;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_Skill += Skill;
        }

        protected override void Update()
        {
            base.Update();
            
            if (ActorProxy.isPlayingAI && _spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                _skillCastedTime = _spawnedTime;
                Summon();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _skillCastedTime = -_skillCooltime;
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
        
        public void Skill()
        {
            SoundManager.instance.Play(clip_Summon);
        }

        public void Summon()
        {
            StartCoroutine(SummonCoroutine());
        }

        IEnumerator SummonCoroutine()
        {
            animator.SetTrigger("Skill");
            _isSkillCasting = true;
            
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < arrSpawnPos.Length; i++)
            {
                arrPs_Spawn[i].Play();
                
                var m = controller.CreateMinion(pref_Skeleton, arrSpawnPos[i].position);

                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomCamp);
                // m.power = effect + (effectUpByInGameUp * ingameUpgradeLevel);
                // //KZSee:
                // // m.maxHealth = effectDuration + (effectCooltime * ingameUpgradeLevel);
                // m.attackSpeed = 0.8f;
                // m.moveSpeed = 1.2f;
                // //KZSee:
                // // m.range = 0.7f;
                // m.eyeLevel = eyeLevel;
                // m.ingameUpgradeLevel = ingameUpgradeLevel;
                m.Initialize();
            }
            
            yield return new WaitForSeconds(0.3f);

            _isSkillCasting = false;
        }
    }
}
