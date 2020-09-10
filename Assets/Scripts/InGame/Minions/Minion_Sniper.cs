﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Sniper : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;
        public LineRenderer lr;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            
            lr.gameObject.SetActive(false);
            light_Fire.enabled = false;
        }

        public override void Attack()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.Get().IsNetwork() && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "AttackReady");
                controller.MinionAniTrigger(id, "AttackReady");
                
                StartCoroutine(AttackReadyCoroutine());
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.Get().IsNetwork() == false )
            {
                base.Attack();
                animator.SetTrigger("AttackReady");
                StartCoroutine(AttackReadyCoroutine());
            }
        }

        public override void Death()
        {
            base.Death();

            lr.gameObject.SetActive(false);
            light_Fire.enabled = false;
        }

        IEnumerator AttackReadyCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            isPushing = false;
            float t = 0;
            lr.gameObject.SetActive(true);
            controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMINIONTARGET, id, target.id);
            controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEVOID, id, "Aiming");
            
            while (t < attackSpeed - 1.5f)
            {
                if (target == null || target.isAlive == false || (target.GetType() == typeof(Minion) && ((Minion)target).isCloacking))
                {
                    target = SetTarget();

                    if (target != null)
                    {
                        controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMINIONTARGET, id, target.id);
                    }
                    else
                    {
                        controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEVOID, id, "StopAiming");
                        break;
                    }
                }

                if (target != null)
                {
                    transform.LookAt(target.transform);
                    lr.SetPositions(new Vector3[2] {ts_ShootingPos.position, target.ts_HitPos.position});
                }

                t += Time.deltaTime;
                yield return null;
            }
            
            lr.gameObject.SetActive(false);

            //if (target != null && ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false))
            if (target != null && ((InGameManager.Get().IsNetwork() && isMine ) || InGameManager.Get().IsNetwork() == false))
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
                controller.MinionAniTrigger(id, "Attack");
            }
        }

        public void Aiming()
        {
            StartCoroutine(AimingCoroutine());
        }

        public void StopAiming()
        {
            StopAllCoroutines();
            lr.gameObject.SetActive(false);
        }
        
        IEnumerator AimingCoroutine()
        {
            float t = 0;
            while (t < attackSpeed - 1.5f && target != null)
            {
                if (target != null)
                {
                    lr.gameObject.SetActive(true);
                    transform.LookAt(target.transform);
                    lr.SetPositions(new Vector3[2] {ts_ShootingPos.position, target.ts_HitPos.position});
                }
                else
                {
                    break;
                }

                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            lr.gameObject.SetActive(false);
        }

        public void FireArrow()
        {
            lr.gameObject.SetActive(false);
            
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }

            if (light_Fire != null)
            {
                light_Fire.enabled = true;
                Invoke("FireLightOff", 0.15f);
            }

            if (target != null)
            {
                if (PhotonNetwork.IsConnected && isMine)
                {
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                    controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    controller.FireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                }
            }
        }

        private void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }
    }
}
