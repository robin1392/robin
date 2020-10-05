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

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            //
            // if (maxHealth > 0)
            // {
            //     var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}"; 
            //     _collider.gameObject.layer = LayerMask.NameToLayer(layerName);
            // }

            image_HealthBar.transform.parent.gameObject.SetActive(false);
            _collider.enabled = true;
            animator.gameObject.SetActive(true);
            SetColor();
            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
            ps_Bomb.transform.localScale = Vector3.one * Mathf.Pow(1.5f, eyeLevel - 1);
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
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && isMine)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINEBOMB, id);
                controller.ActionMineBomb(id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
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
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                isTriggerOn = true;
                image_HealthBar.transform.parent.gameObject.SetActive(true);
            }

            animator.SetTrigger(Set);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (isTriggerOn && 
                //((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) &&
                ( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI ) && 
                1 << collision.gameObject.layer == targetLayer)
            {
                StopAllCoroutines();

                // Bomb
                isTriggerOn = false;
                //if (PhotonNetwork.IsConnected)
                if(InGameManager.IsNetwork && isMine)
                {
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINEBOMB ,  id);
                    controller.ActionMineBomb(id);
                }
                else
                {
                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            StopAllCoroutines();
            _collider.enabled = false;
            image_HealthBar.transform.parent.gameObject.SetActive(false);
            
            //if (((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false))
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                var cols = Physics.OverlapSphere(transform.position, range * Mathf.Pow(1.5f, eyeLevel - 1), targetLayer);
                foreach (var col in cols)
                {
                    var m = col.GetComponentInParent<Minion>();
                    if(m != null)
                    {
                        controller.AttackEnemyMinionOrMagic(m.id, power, 0f);

                        //if(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                        //controller.HitMinionDamage( true , m.id , power, 0f);
                        /*if(InGameManager.IsNetwork == true)
                        {
                            controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINIONANDMAGIC , m.id, power, 0f);
                        }
                        //else if (PhotonNetwork.IsConnected == false)
                        else if(InGameManager.IsNetwork == false)
                        {
                            controller.targetPlayer.HitDamageMinionAndMagic(m.id, power, 0f);
                        }*/

                    }
                }
            }

            animator.gameObject.SetActive(false);
            ps_Bomb.Play();

            base.Destroy(2f);
        }

        protected override void EndLifetime()
        {
            isTriggerOn = false;
            //if (PhotonNetwork.IsConnected)
            if(InGameManager.IsNetwork && isMine)
            {
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINEBOMB ,  id);
                controller.ActionMineBomb(id);
            }
            else
            {
                Bomb();
            }
        }
    }
}