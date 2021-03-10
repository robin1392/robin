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
        private BaseStat _minion;

        private void Awake()
        {
            _minion = GetComponentInParent<BaseStat>();
        }

        public void Attack()
        {
            //if (_minion != null && _minion.isAlive && _minion.target != null && ((PhotonNetwork.IsConnected && _minion.isMine) || PhotonNetwork.IsConnected == false))
            if (_minion != null && _minion.isAlive && _minion.controller.isPlayingAI)
            {
                Minion m = _minion as Minion;
                if (m != null)
                {
                    var damage = m.target.CalcDamage(m.power);
                    m.target?.ActorProxy?.HitDamage(damage);
                    event_Attack?.Invoke();
                    SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
                    
                    //PlayerController.Get().SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                    //PlayerController.Get().ActionActivePoolObject("Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                }
            }
        }

        public void FireArrow()
        {
            FireInternal();
        }

        public void FireSpear()
        {
            FireInternal();
        }

        public void FireInternal()
        {
            //TODO: 발사체서 프리팹 구분이 FireArrow, FireSpear 콜백으로 구분하는 방식에서 호출 클래스에서 Switch문으로 구분하는 방식으로 바뀜. 에니메이션 이벤트를 한가지로 통일해야 함
            if( _minion != null && _minion.target != null && (InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI) && _minion.target != null) || InGameManager.IsNetwork == false)
            {
                event_FireLight?.Invoke();
                event_FireArrow?.Invoke();
                event_FireSpear?.Invoke();
                SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_BOW_SHOT);
            }
        }

        public void Skill()
        {
            event_Skill?.Invoke();
        }
    }
}