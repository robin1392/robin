using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_ZombieRevived : Minion
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
    }
}