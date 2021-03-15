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

        public override void Death()
        {
            StartCoroutine(DeathCoroutine());
        }

        IEnumerator DeathCoroutine()
        {
            SoundManager.instance.Play(clip_Explosion);
            
            var t = 0f;
            var tick = 0f;
            while (t < 5f)
            {
                if (t >= tick)
                {
                    tick += 0.1f;
                    var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
                    
                    foreach (var col in cols)
                    {
                        if (col.CompareTag("Player")) continue;

                        var bs = col.transform.GetComponentInParent<BaseStat>();

                        if (bs == null || bs.id == id) continue;

                        DamageToTarget(bs, 0, 1f);
                    }
                }

                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            _poolObjectAutoDeactivate.Deactive();
        }
    }
}
