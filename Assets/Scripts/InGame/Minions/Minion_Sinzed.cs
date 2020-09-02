﻿#if UNITY_EDITOR
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
        private void OnEnable()
        {
            animator.gameObject.SetActive(true);
            if (_collider == null) _collider = GetComponentInChildren<Collider>();
            _collider.enabled = true;
        }

        public override BaseStat SetTarget()
        {
            target = controller.targetPlayer;

            if (isAlive)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 2f)
                {
                    controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, id, float.MaxValue, 0f);
                }
            }
            
            return target;
        }

        public override void Death()
        {
            SetControllEnable(false);
            _collider.enabled = false;
            animator.SetFloat(_animatorHashMoveSpeed, 0);
            isPlayable = false;
            StopAllCoroutines();
            InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);

            destroyCallback(this);
            PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            animator.gameObject.SetActive(false);
            
            controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Poison", ts_HitPos.position,
                Quaternion.identity, Vector3.one);
            StartCoroutine(DeathCoroutine());
        }

        IEnumerator DeathCoroutine()
        {
            var t = 0f;
            var tick = 0f;
            while (t < 5f)
            {
                if (t >= tick)
                {
                    tick += 0.1f;
                    var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
                    
                    foreach (var col in cols)
                    {
                        if (col.CompareTag("Player")) continue;

                        var bs = col.transform.GetComponentInParent<BaseStat>();

                        if (bs == null || bs.id == id) continue;

                        DamageToTarget(bs, 0, 1f);
                    }
                }

                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            _poolObjectAutoDeactivate.Deactive();
        }
    }
}
