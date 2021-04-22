using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;

namespace ED
{
    public class Minion_Zombie : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_Blade;
        public AudioClip clip_Poison;
        
        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Attack += AttackEvent;
        }

        public void AttackEvent()
        {
            SoundManager.instance.Play(clip_Blade);
        }

        public override void OnBaseEntityDestroyed()
        {
            var rwClient = ActorProxy.Client as RWNetworkClient;
            // 독구름
            rwClient.Send(new CreateActorMessage()
            {
                diceId = 3014,
                ownerTag = ActorProxy.ownerTag,
                team = ActorProxy.team,
                inGameLevel = ActorProxy.ingameUpgradeLevel,
                outGameLevel = ActorProxy.outgameUpgradeLevel,
                positions = new Vector3[] { transform.position },
                delay = 0f,
            });
            
            // 부활
            rwClient.Send(new CreateActorMessage()
            {
                diceId = 3015,
                ownerTag = ActorProxy.ownerTag,
                team = ActorProxy.team,
                inGameLevel = ActorProxy.ingameUpgradeLevel,
                outGameLevel = ActorProxy.outgameUpgradeLevel,
                positions = new Vector3[] { transform.position },
                delay = 2f,
            });
            
            base.OnBaseEntityDestroyed();
        }
    }
}