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
            if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            {
                ((Minion)_minion).DamageToTarget(_minion.target, delay);
            }
        }

        public void FireArrow()
        {
            if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            {
                _minion.SendMessage("FireArrow", SendMessageOptions.DontRequireReceiver);
            }
        }

        public void FireSpear()
        {
            if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            {
                _minion.SendMessage("FireSpear", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}