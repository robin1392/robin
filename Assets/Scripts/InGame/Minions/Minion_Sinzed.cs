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
    public class Minion_Sinzed : Minion
    {
        public ParticleSystem ps;

        private void OnEnable()
        {
            animator.gameObject.SetActive(true);
            if (_collider == null) _collider = GetComponentInChildren<Collider>();
            _collider.enabled = true;
        }

        public override BaseStat SetTarget()
        {
            target = controller.targetPlayer;
            return target;
        }
        
        public override void Attack()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                //controller.photonView.RPC("HitDamageMinion", RpcTarget.All, id, float.MaxValue, 0f);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINION,id, float.MaxValue, 0f);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                //animator.SetTrigger(AnimatorHashAttack);
                controller.HitDamageMinion(id, float.MaxValue, 0f);
            }
        }
        
        // public void FireArrow()
        // {
        //     if (PhotonNetwork.IsConnected && isMine)
        //     {
        //         controller.photonView.RPC("FireSpear", RpcTarget.All, shootingPos.position, target.id, power);
        //     }
        //     else if (PhotonNetwork.IsConnected == false)
        //     {
        //         controller.FireSpear(shootingPos.position, target.id, power);
        //     }
        // }

        public override void Death()
        {
            //rb.velocity = Vector3.zero;
            //rb.isKinematic = true;
            //_agent.velocity = Vector3.zero;
            //_agent.isStopped = true;
            //_agent.updatePosition = false;
            //_agent.updateRotation = false;
            //_agent.enabled = false;
            SetControllEnable(false);
            _collider.enabled = false;
            animator.SetFloat(_animatorHashMoveSpeed, 0);
            isPlayable = false;
            StopAllCoroutines();
            InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);

            destroyCallback(this);
            PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            animator.gameObject.SetActive(false);
            
            // var cols = Physics.OverlapSphere(transform.position, range * 2f, targetLayer);
            // foreach (var col in cols)
            // {
            //     if (col.CompareTag("Player")) continue;
            //
            //     var m = col.GetComponent<Minion>();
            //     if (m.isAlive)
            //     {
            //         DamageToTarget(m);
            //     }
            // }
            ps.Play();
            StartCoroutine(DeathCoroutine());
        }

        IEnumerator DeathCoroutine()
        {
            var t = 0f;
            var tick = 0f;
            while (t < 5f)
            {
                t += Time.deltaTime;
                if (t >= tick)
                {
                    tick += 0.1f;
                    var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
                    //Debug.LogFormat("Poison: {0}", cols.Length);
                    foreach (var col in cols)
                    {
                        if (col.CompareTag("Player")) continue;

                        var bs = col.transform.GetComponent<BaseStat>();

                        if (bs.id == id) continue;

                        // if (PhotonNetwork.IsConnected && isMine)
                        // {
                        //     bs.controller.photonView.RPC("HitDamageMinion", RpcTarget.All,
                        //         bs.id, power * 0.05f, 0f);
                        // }
                        // else if (PhotonNetwork.IsConnected == false)
                        // {
                        //     bs.controller.HitDamageMinion(bs.id, power * 0.05f, 0f);
                        // }
                        DamageToTarget(bs, 0, 1f);
                    }
                }

                yield return null;
            }
            
            ps.Stop();
            yield return new WaitForSeconds(2f);
            _poolObjectAutoDeactivate.Deactive();
        }
    }
}
