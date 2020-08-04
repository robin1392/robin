#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class FlagOfWar : Magic
    {
        public Collider col;
        public float lifeTime = 20f;

        private bool _isTriggerOn;
        private List<int> _listAttackSpeedUp = new List<int>();
        
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            col.enabled = false;
            _listAttackSpeedUp.Clear();
            SetColor();
        }
        
        public override void SetTarget()
        {
            SetTargetPosition();
        }
        
        protected override IEnumerator Move()
        {
            var startPos = transform.position;
            var endPos = targetPos;
            var distance = Vector3.Distance(startPos, endPos);
            var max = distance / moveSpeed;

            rb.velocity = (endPos - startPos).normalized * moveSpeed;
            yield return new WaitForSeconds(max);
            rb.velocity = Vector3.zero;
            transform.position = endPos;

            EndMove();
        }

        private void EndMove()
        {
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                _isTriggerOn = true;
                col.enabled = true;
                StartCoroutine(LifetimeCoroutine());
            }
        }

        private IEnumerator LifetimeCoroutine()
        {
            yield return new WaitForSeconds(lifeTime);

            if (InGameManager.Get().isGamePlaying == false) yield break;

            _isTriggerOn = false;

            foreach (var baseStatId in _listAttackSpeedUp)
            {
                if (PhotonNetwork.IsConnected)
                {
                    //controller.photonView.RPC("SetMinionAttackSpeedFactor", RpcTarget.All, baseStatId, 1f);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONATTACKSPEEDFACTOR ,baseStatId, 1f);
                }
                else
                {
                    controller.SetMinionAttackSpeedFactor(baseStatId, 1f);
                }
            }
            
            Destroy();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (_isTriggerOn &&
                ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) &&
                IsFriendlyLayer(collision.gameObject))
            {
                
                var m = collision.GetComponentInParent<Minion>();

                if (_listAttackSpeedUp.Contains(m.id) == false)
                {
                    _listAttackSpeedUp.Add(m.id);
                    
                    if (PhotonNetwork.IsConnected)
                    {
                        //controller.photonView.RPC("SetMinionAttackSpeedFactor", RpcTarget.All, m.id, 2f);
                        controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONATTACKSPEEDFACTOR ,m.id, 2f);
                    }
                    else
                    {
                        controller.SetMinionAttackSpeedFactor(m.id, 2f);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            if (_isTriggerOn &&
                ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) &&
                IsFriendlyLayer(other.gameObject))
            {
                var m = other.GetComponentInParent<Minion>();

                if (_listAttackSpeedUp.Contains(m.id))
                {
                    _listAttackSpeedUp.Remove(m.id);
                    
                    if (PhotonNetwork.IsConnected)
                    {
                        //controller.photonView.RPC("SetMinionAttackSpeedFactor", RpcTarget.All, m.id, 1f);
                        controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONATTACKSPEEDFACTOR ,m.id, 1f);
                    }
                    else
                    {
                        controller.SetMinionAttackSpeedFactor(m.id, 1f);
                    }
                }
            }
        }
    }
}
