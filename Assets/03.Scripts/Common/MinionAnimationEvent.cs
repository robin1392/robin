#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
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
            if (_minion != null && _minion.isAlive && _minion.target != null && _minion.target.isAlive && ((InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI)) || InGameManager.IsNetwork == false))
            {
                Minion m = _minion as Minion;
                if (m != null)
                {
                    m.DamageToTarget(m.target, delay);
                    
                    event_Attack?.Invoke();
                    SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
                    //PlayerController.Get().SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                    //PlayerController.Get().ActionActivePoolObject("Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                }
            }
        }

        public void FireArrow()
        {
            //if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            if( _minion != null && (InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI) && _minion.target != null) || InGameManager.IsNetwork == false)
            {
                //_minion.SendMessage("FireLightOn", SendMessageOptions.DontRequireReceiver);
                event_FireLight?.Invoke();

                //if (PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null || PhotonNetwork.IsConnected == false)
                if( (InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI) && _minion.target != null ) || InGameManager.IsNetwork == false)
                {
                    //_minion.SendMessage("FireArrow", SendMessageOptions.DontRequireReceiver);
                    event_FireArrow?.Invoke();
                    SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_BOW_SHOT);
                }
            }
            
            
            // if ((PhotonNetwork.IsConnected && _minion != null && _minion.target != null) || PhotonNetwork.IsConnected == false)
            // {
            //     if (_minion.isMine)
            //     {
            //         _minion.SendMessage("FireArrow", SendMessageOptions.DontRequireReceiver);
            //     }
            //     else
            //     {
            //         _minion.SendMessage("FireArrowIsNotMine", SendMessageOptions.DontRequireReceiver);
            //     }
            // }
        }

        public void FireSpear()
        {
            // if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            // {
            //     _minion.SendMessage("FireSpear", SendMessageOptions.DontRequireReceiver);
            // }
            
            //if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            if( _minion != null && _minion.target != null && (InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI) && _minion.target != null) || InGameManager.IsNetwork == false)
            {
                //_minion.SendMessage("FireLightOn", SendMessageOptions.DontRequireReceiver);
                event_FireLight?.Invoke();

                //if (PhotonNetwork.IsConnected && _minion.isMine || PhotonNetwork.IsConnected == false)
                if( (InGameManager.IsNetwork && (_minion.isMine || _minion.controller.isPlayingAI)) || InGameManager.IsNetwork == false)
                {
                    //_minion.SendMessage("FireArrow", SendMessageOptions.DontRequireReceiver);
                    event_FireSpear?.Invoke();
                    SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_BOW_SHOT);
                }
            }
        }

        public void Skill()
        {
            event_Skill?.Invoke();
        }
    }
}