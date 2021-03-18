using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;
using UnityEngine.Rendering;

namespace ED
{
    public class Iceball : Magic
    {
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        [Header("AudioClip")]
        public AudioClip clip_Shoot;
        public AudioClip clip_Explosion;
        
        private float sturnTime => effect + (effectUpByUpgrade * ActorProxy.outgameUpgradeLevel) + (effectUpByInGameUp * ActorProxy.ingameUpgradeLevel);
        private bool isBombed = false;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            isBombed = false;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);

            ps_Tail.Clear();
            ps_BombEffect.Clear();
        }

        protected override IEnumerator Cast()
        {
            target = ActorProxy.GetRandomEnemyCanBeAttacked();
            
            var iceBallAction = new IceBallAction();
            RunningAction = iceBallAction;
            yield return iceBallAction.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (isBombed)
            {
                return;
            }

            if (ActorProxy.isPlayingAI == false)
            {
                return;
            }

            if (target != null && other.gameObject == target.gameObject ||
                other.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                Bomb();
            }
        }

        public void Bomb()
        {
            SoundManager.instance.Play(clip_Explosion);
            ps_Tail.Stop();
            ps_BombEffect.Play();
            
            if (ActorProxy.isPlayingAI == false) return;

            var targetActorProxy = ActorProxy.baseStat.target.ActorProxy;
            if (targetActorProxy != null && targetActorProxy.baseStat.CanBeTarget())
            {
                targetActorProxy.AddBuff(BuffInfos.Freeze, sturnTime);
                targetActorProxy.HitDamage(ActorProxy.power);
            }
            
            ActorProxy.Destroy(1.1f);
        }
    }
    
    public class IceBallAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var iceBall = actorProxy.baseStat as Iceball;
            var actorTransform = actorProxy.transform;
            var target = targetActorProxy.baseStat;

            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_ICEBALL_MISSILE);
            
            var startPos = actorTransform.position;

            //?
            while (target == null)
            {
                yield return null;
            }
            
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / iceBall.moveSpeed;

            float t = 0;
            
            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                actorTransform.position = Vector3.Lerp(startPos, endPos, t / moveTime);

                t += Time.deltaTime;
                yield return null;
            }

            iceBall.Bomb();
        }
    }
}
