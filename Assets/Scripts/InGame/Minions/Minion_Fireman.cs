﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Fireman : Minion
    {
        public ParticleSystem ps_Fire;
        public Light light;

        public bool isFire;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            ps_Fire.Stop();
            light.enabled = false;
            isFire = false;
        }

        public override void Death()
        {
            base.Death();

            isFire = false;
            light.enabled = false;
            ps_Fire.Stop();
        }

        public override void Sturn(float duration)
        {
            base.Sturn(duration);
            
            isFire = false;
            light.enabled = false;
            ps_Fire.Stop();
        }

        public override void Attack()
        {
            if (target != null)
            {
                //if (PhotonNetwork.IsConnected && isMine)
                if( InGameManager.IsNetwork && isMine )
                {
                    base.Attack();
                    
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                    controller.MinionAniTrigger(id, "Attack");
                    
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREMANFIRE , id);
                    controller.ActionFireManFire(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false )
                {
                    base.Attack();
                    animator.SetTrigger(_animatorHashAttack);
                    controller.FiremanFire(id);
                }
            }
        }

        public void Fire()
        {
            if (isFire == false)
            {
                StartCoroutine(FireCoroutine());
            }

        }

        IEnumerator FireCoroutine()
        {
            isFire = true;
            ps_Fire.Play();
            light.enabled = true;
            var t = 0f;
            var tick = 0f;
            while (t < 0.95f)
            {
                t += Time.deltaTime;
                
                if (t >= tick)
                {
                    tick += attackSpeed;
                    
                    //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
                    if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false )
                    {
                        var cols = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range,
                            targetLayer);
                        foreach (var col in cols)
                        {
                            var bs = col.transform.GetComponentInParent<BaseStat>();

                            if (bs.id == id) continue;

                            DamageToTarget(bs);
                        }
                    }
                    
                }
                
                yield return null;
            }

            ps_Fire.Stop();
            light.enabled = false;
            isFire = false;
        }

        public override void EndGameUnit()
        {
            base.EndGameUnit();
            
            ps_Fire.Stop();
        }
    }
}
