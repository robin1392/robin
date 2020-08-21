#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace ED
{
    public class Mine : Magic
    {
        public ParticleSystem ps_Bomb;
        public float pushPower = 2f;
        public float lifeTime = 20f;

        private bool isTriggerOn;
        private float _originRange;
        private static readonly int Set = Animator.StringToHash("Set");

        protected override void Awake()
        {
            base.Awake();
            _originRange = range;
        }

        private void Update()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            //
            // if (maxHealth > 0)
            // {
            //     var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}"; 
            //     _collider.gameObject.layer = LayerMask.NameToLayer(layerName);
            // }

            _collider.enabled = true;
            animator.gameObject.SetActive(true);
            SetColor();
            image_HealthBar.enabled = false;
            image_HealthBar.fillAmount = 1f;
            ps_Bomb.transform.localScale = Vector3.one * Mathf.Pow(1.5f, eyeLevel - 1);
            StartCoroutine(LifetimeCoroutine());
        }

        public override void SetTarget()
        {
            SetTargetPosition();
        }

        public override void Destroy(float delay = 0)
        {
            StopAllCoroutines();

            // Bomb
            isTriggerOn = false;
            if (PhotonNetwork.IsConnected && isMine)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINEBOMB, id);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                Bomb();
            }
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
                image_HealthBar.enabled = true;
            }

            animator.SetTrigger(Set);
        }

        private IEnumerator LifetimeCoroutine()
        {
            float t = 0;
            while (t < lifeTime)
            {
                yield return null;
                t += Time.deltaTime;
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, id, (maxHealth / lifeTime) * Time.deltaTime, 0f);
            }
            
            if (InGameManager.Get().isGamePlaying == false) yield break;

            isTriggerOn = false;
            if (PhotonNetwork.IsConnected)
            {
                //controller.photonView.RPC("MineBomb", RpcTarget.All, id);
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINEBOMB ,  id);
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
                    //controller.photonView.RPC("MineBomb", RpcTarget.All, id);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINEBOMB ,  id);
                }
                else
                {
                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            _collider.enabled = false;
            image_HealthBar.enabled = false;
            
            if (((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false))
            {
                var cols = Physics.OverlapSphere(transform.position, range * Mathf.Pow(1.5f, eyeLevel - 1), targetLayer);
                foreach (var col in cols)
                {
                    var m = col.GetComponent<Minion>();
                    if(m)
                    {
                        if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                        {
                            //controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.Others, m.id, damage, 0f);
                            controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINIONANDMAGIC , m.id, power, 0f);
                            //controller.targetPlayer.photonView.RPC("PushMinion", RpcTarget.Others, m.id, col.transform.position - transform.position, pushPower);
                        }
                        else if (PhotonNetwork.IsConnected == false)
                        {
                            controller.targetPlayer.HitDamageMinionAndMagic(m.id, power, 0f);
                            //controller.targetPlayer.PushMinion(m.id, col.transform.position - transform.position, pushPower);
                        }
                    }
                }
            }

            animator.gameObject.SetActive(false);
            ps_Bomb.Play();

            base.Destroy(2f);
        }
    }
}