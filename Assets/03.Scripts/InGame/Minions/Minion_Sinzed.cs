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
        
        private void OnEnable()
        {
            animator.gameObject.SetActive(true);
            if (_collider == null) _collider = GetComponentInChildren<Collider>();
            _collider.enabled = true;
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
                    ActorProxy.SummonActor(SummonActorInfos.SinzedPoison, transform.position);
                    ActorProxy.Destroy();
                }
            }
            
            return target;
        }

        public override void OnBaseStatDestroyed()
        {
            base.OnBaseStatDestroyed();
            SoundManager.instance.Play(clip_Explosion);
        }
    }
}
