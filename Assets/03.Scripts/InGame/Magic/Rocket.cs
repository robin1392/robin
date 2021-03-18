using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Rocket : Magic
    {
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            transform.localScale = Vector3.one * Mathf.Lerp(1f, 3f, (ActorProxy.diceScale - 1) / 5f);

            ps_Tail.Clear();
            ps_BombEffect.Clear();
        }
        
        protected override IEnumerator Cast()
        {
            target = ActorProxy.GetEnemyTower();
            if (target == null)
            {
                yield break;
            }
            
            var rocketAction = new RocketAction();
            RunningAction = rocketAction;
            yield return rocketAction.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;
        }

        public void Bomb()
        {
            ps_Tail.Stop();
            ps_BombEffect.Play();
            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_COMMON_EXPLOSION);

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.Destroy(1.1f);
            }
        }
    }
    
    public class RocketAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var rocket = actorProxy.baseStat as Rocket;
            var actorTransform = actorProxy.transform;
            var target = targetActorProxy.baseStat;

            var startPos = actorTransform.position;
            var endPos = targetActorProxy.baseStat.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / actorProxy.baseStat.moveSpeed;
            actorProxy.transform.LookAt(target.transform);

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

            if (actorProxy.isPlayingAI)
            {
                targetActorProxy.HitDamage(actorProxy.power);
            }

            rocket.Bomb();
        }
    }
}