#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using RandomWarsProtocol;

namespace ED
{
    public class Fireball : Magic
    {
        public Light light;
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        [Header("AudioClip")] public AudioClip clip_Fire;
        public AudioClip clip_Explosion;

        private bool isBombed = false;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            isBombed = false;
            ps_Tail.transform.localScale = Vector3.one * Mathf.Lerp(1f, 3f, (eyeLevel - 1) / 5f);
            ps_BombEffect.transform.localScale = Vector3.one * Mathf.Lerp(0.7f, 1f, (eyeLevel - 1) / 5f);

            ps_Tail.Clear();
            ps_BombEffect.Clear();
        }

        protected override IEnumerator Cast()
        {
            target = ActorProxy.GetRandomEnemyCanBeAttacked();
            
            var fireBallAction = new FireBallAction();
            RunningAction = fireBallAction;
            yield return fireBallAction.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isBombed)
            {
                return;
            }

            if (target != null && other.gameObject == target.gameObject ||
                other.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                Bomb();
            }
        }

        private void SplashDamage()
        {
            Vector3 pos = transform.position;
            pos.y = 0;
            var cols = Physics.OverlapSphere(pos, range * Mathf.Pow(1.5f, eyeLevel - 1), targetLayer);
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    ActorProxy.DamageTo(bs);
                }
            }
        }

        public void Bomb()
        {
            if (isBombed)
            {
                return;
            }
            
            isBombed = true;
            light.enabled = false;
            ps_Tail.Stop();
            ps_BombEffect.Play();

            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_COMMON_EXPLOSION);

            if (ActorProxy.isPlayingAI)
            {
                SplashDamage();
                ActorProxy.Destroy(1.1f);
            }
        }
    }
}