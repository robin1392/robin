using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Knight : Minion
    {
        [Header("Audio Clip")]
        public AudioClip clip_Blade;
        public override void Initialize()
        {
            base.Initialize();

            _animationEvent.event_Attack -= AttackSound;
            _animationEvent.event_Attack += AttackSound;
        }

        public void AttackSound()
        {
            SoundManager.instance.Play(clip_Blade);
        }
    }
}