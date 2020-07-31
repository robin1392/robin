#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace ED
{
    public class Mine : Magic
    {
        public Animator ani;
        public ParticleSystem ps_Bomb;
        public float pushPower = 2f;
        public float lifeTime = 20f;

        private bool isTriggerOn;
        private static readonly int Set = Animator.StringToHash("Set");

        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            ani.gameObject.SetActive(true);
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
                isTriggerOn = true;
                StartCoroutine(LifetimeCoroutine());
            }

            ani.SetTrigger(Set);
        }

        private IEnumerator LifetimeCoroutine()
        {
            yield return new WaitForSeconds(lifeTime);

            if (InGameManager.Instance.isGamePlaying == false) yield break;

            isTriggerOn = false;
            if (PhotonNetwork.IsConnected)
            {
                controller.photonView.RPC("MineBomb", RpcTarget.All, id);
            }
            else
            {
                Bomb();
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (isTriggerOn &&
                ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) &&
                1 << collision.gameObject.layer == targetLayer)
            {
                StopAllCoroutines();

                // Bomb
                isTriggerOn = false;
                if (PhotonNetwork.IsConnected)
                {
                    controller.photonView.RPC("MineBomb", RpcTarget.All, id);
                }
                else
                {
                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            if (((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false))
            {
                var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
                foreach (var col in cols)
                {
                    var m = col.GetComponent<Minion>();
                    if(m)
                    {
                        if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                        {
                            controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.Others, m.id, damage, 0f);
                            //controller.targetPlayer.photonView.RPC("PushMinion", RpcTarget.Others, m.id, col.transform.position - transform.position, pushPower);
                        }
                        else if (PhotonNetwork.IsConnected == false)
                        {
                            controller.targetPlayer.HitDamageMinion(m.id, damage, 0f);
                            //controller.targetPlayer.PushMinion(m.id, col.transform.position - transform.position, pushPower);
                        }
                    }
                }
            }

            ani.gameObject.SetActive(false);
            ps_Bomb.Play();

            Destroy(2f);
        }
    }
}