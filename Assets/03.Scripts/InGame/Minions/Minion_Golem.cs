using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ED
{
    public class Minion_Golem : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_Attack;
        public AudioClip clip_Exposion;

        protected override void Awake()
        {
            base.Awake();

            _animationEvent.event_Attack += AttackEvent;
        }

        private void AttackEvent()
        {
            SoundManager.instance.Play(clip_Attack);
        }

        public override BaseStat SetTarget()
        {
            return ActorProxy.GetEnemyTower();
        }

        public override void OnBaseStatDestroyed()
        {
            var rwClient = ActorProxy.Client as RWNetworkClient;
            // 미니골렘
            rwClient.Send(new CreateActorMessage()
            {
                diceId = 4004,
                ownerTag = ActorProxy.ownerTag,
                team = ActorProxy.team,
                inGameLevel = ActorProxy.ingameUpgradeLevel,
                outGameLevel = ActorProxy.outgameUpgradeLevel,
                positions = new Vector3[]
                {
                    ActorProxy.transform.position + Vector3.right * Random.Range(-0.5f, 0.5f) + Vector3.forward * Random.Range(-0.5f, 0.5f),
                    ActorProxy.transform.position + Vector3.right * Random.Range(-0.5f, 0.5f) + Vector3.forward * Random.Range(-0.5f, 0.5f)
                },
                delay = 0f,
            });
            
            base.OnBaseStatDestroyed();
        }
    }
}