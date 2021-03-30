#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace  ED
{
    public class StoneBall : Magic
    {
        [Header("Prefab")]
        public GameObject pref_StoneHitEffect;
        
        [Space]
        public Transform ts_Model;

        [Header("AudioClip")]
        public AudioClip clip_Rolling;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_StoneHitEffect, 2);
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            ts_Model.gameObject.SetActive(true);
            
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
        }

        protected override IEnumerator Cast()
        {
            var stoneBallAction = new StoneBallAction();
            RunningAction = stoneBallAction;
            yield return stoneBallAction.ActionWithSync(ActorProxy);
            RunningAction = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTargetLayer(other.gameObject) && other.CompareTag("Player") == false)
            {
                var bs = other.GetComponentInParent<BaseStat>();
                PoolManager.instance.ActivateObject("Effect_Stone", bs.ts_HitPos.position);
                
                if (ActorProxy.isPlayingAI)
                {
                    if (bs != null && bs.isAlive)
                    {
                        bs.ActorProxy.HitDamage(power);
                    }
                }
            }
            else if (other.CompareTag("Wall"))
            {
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
                ts_Model.gameObject.SetActive(false);
                SoundManager.instance.Play(Global.E_SOUND.SFX_COMMON_EXPLOSION);
                if (ActorProxy.isPlayingAI)
                {
                    ActorProxy.Destroy();
                }
            }
        }
    }
    
    public class StoneBallAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actor)
        {
            var stoneBall = actor.baseStat as StoneBall;
            var actorTransform = actor.transform;
            var isBottomCamp = stoneBall.isBottomCamp;
            var modelTransform = stoneBall.ts_Model;
            SoundManager.instance.Play(stoneBall.clip_Rolling);
            float angle = 0f;
            Vector3 forward = actorTransform.forward;
            while (true)
            {
                actorTransform.position += forward * (stoneBall.moveSpeed * Time.deltaTime);
                angle += (isBottomCamp? 45f : -45f) * Time.deltaTime;
                modelTransform.rotation = Quaternion.AngleAxis(angle, Vector3.right);
                yield return null;
            }
        }
    }
}
