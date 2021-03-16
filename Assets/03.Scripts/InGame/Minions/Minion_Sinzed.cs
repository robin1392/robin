#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;

namespace ED
{
    public class Minion_Sinzed : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_Explosion;

        private bool bombed;


        public override void Initialize()
        {
            base.Initialize();
            bombed = false;
        }

        private void OnEnable()
        {
            animator.gameObject.SetActive(true);
            if (collider == null) collider = GetComponentInChildren<Collider>();
            collider.enabled = true;
        }

        public override BaseStat SetTarget()
        {
            target = ActorProxy.GetEnemyTower();
            if (target == null)
            {
                return null;
            }

            if (isAlive)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 2f)
                {
                    Bomb();
                }
            }
            
            return target;
        }

        void Bomb()
        {
            if (bombed)
            {
                return;
            }

            bombed = true;
            ActorProxy.DestroyAfterSummonActor(SummonActorInfos.SinzedPoison, transform.position);
            StopAI();
        }

        public override bool OnBeforeHitDamage(float damage)
        {
            if (damage >= ActorProxy.currentHealth)
            {
                Bomb();
                return true;
            }

            return false;
        }

        public override void OnBaseStatDestroyed()
        {
            base.OnBaseStatDestroyed();
            SoundManager.instance.Play(clip_Explosion);
        }
    }
}
