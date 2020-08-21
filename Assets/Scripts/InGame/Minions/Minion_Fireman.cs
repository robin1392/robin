using System.Collections;
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
        }

        public override void Sturn(float duration)
        {
            base.Sturn(duration);
            
            ps_Fire.Stop();
        }

        public override void Attack()
        {
            if (target != null)
            {
                if (PhotonNetwork.IsConnected && isMine)
                {
                    base.Attack();
                    //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                    //controller.photonView.RPC("FiremanFire", RpcTarget.All, id);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREMANFIRE , id);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    base.Attack();
                    animator.SetTrigger(_animatorHashAttack);
                    controller.FiremanFire(id);
                }
            }
        }

        public void Fire()
        {
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
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
                    var cols = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range,
                        targetLayer);
                    foreach (var col in cols)
                    {
                        var bs = col.transform.GetComponentInParent<BaseStat>();
                        
                        if (bs.id == id) continue;

                        // if (PhotonNetwork.IsConnected && isMine)
                        // {
                        //     controller.targetPlayer.photonView.RPC("HitDamageMinion", 
                        //         RpcTarget.All, bs.id, power * 0.1f, 0f);
                        // }
                        // else if (PhotonNetwork.IsConnected == false)
                        // {
                        //     controller.targetPlayer.HitDamageMinion(bs.id, power * 0.1f, 0f);
                        // }
                        
                        DamageToTarget(bs);
                    }
                }

                yield return null;
            }

            ps_Fire.Stop();
            light.enabled = false;
            isFire = false;
        }
    }
}
