﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

namespace ED
{
    public class Iceball : Magic
    {
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        private float sturnTime => effect + effectDuration * eyeLevel;

        public override void SetTarget()
        {
            StartCoroutine(Move());
        }

        protected override IEnumerator Move()
        {
            var startPos = transform.position;
            while (target == null)
            {
                yield return null;
                
                target = InGameManager.Get().GetRandomPlayerUnitHighHealth(!isBottomPlayer);
                if (target != null)
                {
                    controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMAGICTARGET, id, target.id);
                }
            }
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / moveSpeed;
            var moveDistance = 0f;

            float t = 0;
            
            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                //rb.position = Vector3.Lerp(startPos, endPos, t / moveTime);
                rb.position += (endPos - transform.position).normalized * (moveSpeed * Time.deltaTime);

                t += Time.deltaTime;
                yield return null;
            }

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
            {
                if (target != null)
                {
                    controller.AttackEnemyMinion(target.id, power, 0f);
                    controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_STURNMINION, target.id, sturnTime);
                }

                //controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.Others, target.id, damage, 0f);
                //controller.photonView.RPC("FireballBomb", RpcTarget.All, id);
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_ICEBALLBOMB , id);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                if (target != null)
                {
                    controller.AttackEnemyMinion(target.id, power, 0f);
                    controller.targetPlayer.SturnMinion(target.id, sturnTime);
                }
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
                    if (target != null)
                    {
                        controller.AttackEnemyMinion(target.id, power, 0f);
                        //controller.targetPlayer.photonView.RPC("SturnMinion", RpcTarget.All, target.id, 3f);
                        controller.targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_STURNMINION , target.id, sturnTime);
                    }
                    
                    //controller.photonView.RPC("FireballBomb", RpcTarget.All, id);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_ICEBALLBOMB , id);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinion(target.id, power, 0f);
                        controller.targetPlayer.SturnMinion(target.id, sturnTime);
                    }

                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            rb.velocity = Vector3.zero;
            ps_Tail.Stop();
            ps_BombEffect.Play();

            Destroy(1.1f);
        }
    }
}
