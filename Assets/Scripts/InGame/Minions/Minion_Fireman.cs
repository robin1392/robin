using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Fireman : Minion
    {
        public ParticleSystem ps_Fire;

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
            ps_Fire.Play();

            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                StartCoroutine(FireCoroutine());
            }
        }

        IEnumerator FireCoroutine()
        {
            var t = 0f;
            var tick = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                if (t >= tick)
                {
                    tick += 0.1f;
                    var cols = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range,
                        targetLayer);
                    foreach (var col in cols)
                    {
                        var bs = col.transform.GetComponent<BaseStat>();
                        
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
                        
                        DamageToTarget(bs, 0, 0.1f);
                    }
                }

                yield return null;
            }
        }
    }
}
