using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;
using UnityEngine.AI;

namespace ED
{
    public class Minion_BabyDragon : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_BabyAttack;

        private int spawnedWave;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();

            ae.event_Attack += AttackEvent;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            var rwClient = ActorProxy.Client as RWNetworkClient;
            spawnedWave = rwClient.GameState.wave;
            rwClient.GameState.WaveEvent.AddListener(OnWave);
        }

        private void OnWave(int wave)
        {
            if (wave > spawnedWave)
            {
                // 변신
                var rwClient = ActorProxy.Client as RWNetworkClient;
                rwClient.Send(new CreateActorMessage()
                {
                    diceId = 4011,
                    ownerTag = ActorProxy.ownerTag,
                    team = ActorProxy.team,
                    inGameLevel = ActorProxy.ingameUpgradeLevel,
                    outGameLevel = ActorProxy.outgameUpgradeLevel,
                    positions = new Vector3[] { transform.position },
                    delay = 0f,
                });
                ActorProxy.Destroy();
            }
        }

        public void AttackEvent()
        {
            SoundManager.instance.Play(clip_BabyAttack);
        }

        public override void OnBaseEntityDestroyed()
        {
            var rwClient = ActorProxy.Client as RWNetworkClient;
            rwClient.GameState.WaveEvent.RemoveListener(OnWave);

            StopAllAction();
            _poolObjectAutoDeactivate?.Deactive();
            if (animator != null) animator.SetFloat(AnimationHash.MoveSpeed, 0);
            _destroyed = true;

            if (ActorProxy.currentHealth <= 0)
            {
                SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_DEATH);
                PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            }

            foreach (var autoDeactivate in _dicEffectPool)
            {
                autoDeactivate.Value.Deactive();
            }
        }
    }
}