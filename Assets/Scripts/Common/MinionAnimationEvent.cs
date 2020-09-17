using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace ED
{
    public class MinionAnimationEvent : MonoBehaviour
    {
        public float delay;
        private BaseStat _minion;

        private void Awake()
        {
            _minion = GetComponentInParent<BaseStat>();
        }

        public void Attack()
        {
            //if (_minion != null && _minion.isAlive && _minion.target != null && ((PhotonNetwork.IsConnected && _minion.isMine) || PhotonNetwork.IsConnected == false))
            if (_minion != null && _minion.isAlive && _minion.target != null && ((InGameManager.IsNetwork && _minion.isMine) || InGameManager.IsNetwork == false))
            {
                Debug.Log("AnimationEventAttack: " + gameObject.name);
                Minion m = _minion as Minion;
                if (m != null)
                {
                    m.DamageToTarget(m.target, delay);
                    //PlayerController.Get().SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                    PlayerController.Get().ActionActivePoolObject("Effect_ArrowHit", m.target.ts_HitPos.position, Quaternion.identity, Vector3.one * 0.6f);
                }
            }
        }

        public void FireArrow()
        {
            //if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && _minion.isMine && _minion.target != null) || InGameManager.IsNetwork == false)
            {
                _minion.SendMessage("FireArrow", SendMessageOptions.DontRequireReceiver);
            }
        }

        public void FireSpear()
        {
            //if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && _minion.isMine && _minion.target != null) || InGameManager.IsNetwork == false)
            {
                _minion.SendMessage("FireSpear", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}