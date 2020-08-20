﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using DG.Tweening;

namespace ED
{
    public class Fireball : Magic
    {
        public Light light;
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        protected override IEnumerator Move()
        {
            light.enabled = true;
            var startPos = transform.position;
            while (target == null) { yield return null; }
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / moveSpeed;

            float t = 0;
            
            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                rb.position = Vector3.Lerp(startPos, endPos, t / moveTime);

                t += Time.deltaTime;
                yield return null;
            }

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
            {
                SplashDamage();
                // if (target != null)
                //     controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINION , target.id, damage, 0f);
                    //controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.Others, target.id, damage, 0f);
                //controller.photonView.RPC("FireballBomb", RpcTarget.All, id);
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREBALLBOMB ,id);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                // if (target != null)
                // {
                //     controller.targetPlayer.HitDamageMinion(target.id, damage, 0f);
                // }
                SplashDamage();
                Bomb();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InGameManager.Get().isGamePlaying == false || destroyRoutine != null) return;

            if (target != null && other.gameObject == target.gameObject || other.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                StopAllCoroutines();
                rb.velocity = Vector3.zero;

                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                {
                    SplashDamage();
                    //if (target != null)
                        //controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINION , target.id, damage, 0f);
                        //controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.Others, target.id, damage, 0f);
                    //controller.photonView.RPC("FireballBomb", RpcTarget.All, id);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREBALLBOMB ,id);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    // if (target != null)
                    // {
                    //     controller.targetPlayer.HitDamageMinion(target.id, damage, 0f);
                    // }
                    SplashDamage();
                    Bomb();
                }
            }
        }

        private void SplashDamage()
        {
            var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
            foreach (var col in cols)
            {
                controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINION , col.GetComponentInParent<BaseStat>().id, power, 0f);
            }
        }

        public void Bomb()
        {
            light.enabled = false;
            rb.velocity = Vector3.zero;
            ps_Tail.Stop();
            ps_BombEffect.Play();

            Destroy(1.1f);
        }
    }
}