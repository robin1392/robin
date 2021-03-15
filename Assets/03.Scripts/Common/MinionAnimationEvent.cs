#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEditor;
using UnityEngine;

namespace ED
{
    public class MinionAnimationEvent : MonoBehaviour
    {
        public delegate void Fire();

        public event Fire event_Attack;
        public event Fire event_FireArrow;
        public event Fire event_FireSpear;
        public event Fire event_FireLight;
        public event Fire event_Skill;
        
        public float delay;
        [SerializeField]
        private Minion _minion;

        private void Awake()
        {
            _minion = GetComponentInParent<Minion>();
        }

        public void Attack()
        {
            if (CanEvent() == false)
            {
                return;
            }

            if (_minion.target == null)
            {
                return;
            }
            
            _minion.target.ActorProxy.HitDamage(_minion.power);
            
            event_Attack?.Invoke();
            SoundManager.instance.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
        }

        public void FireArrow()
        {
            FireInternal();
        }

        public void FireSpear()
        {
            FireInternal();
        }

        bool CanEvent()
        {
            if (_minion == null 
                || _minion.isAlive == false
                || _minion.Destroyed 
                || _minion.ActorProxy.isPlayingAI == false)
            {
                return false;
            }

            return true;
        }

        public void FireInternal()
        {
            if (CanEvent() == false)
            {
                return;
            }
                
            event_FireLight?.Invoke();
            event_FireArrow?.Invoke();
            event_FireSpear?.Invoke();
            SoundManager.instance.PlayRandom(Global.E_SOUND.SFX_MINION_BOW_SHOT);
        }

        public void Skill()
        {
            if (CanEvent() == false)
            {
                return;
            }
            
            event_Skill?.Invoke();
        }
    }
}