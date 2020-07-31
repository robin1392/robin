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
        private Minion _minion;

        private void Awake()
        {
            _minion = GetComponentInParent<Minion>();
        }

        public void Attack()
        {
            if (PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null)
            {
                _minion.DamageToTarget(_minion.target, delay);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                _minion.DamageToTarget(_minion.target, delay);
            }
        }

        public void FireArrow()
        {
            if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            {
                _minion.SendMessage("FireArrow");
            }
        }

        public void FireSpear()
        {
            if ((PhotonNetwork.IsConnected && _minion.isMine && _minion.target != null) || PhotonNetwork.IsConnected == false)
            {
                _minion.SendMessage("FireSpear");
            }
        }
    }
}