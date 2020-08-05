#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace  ED
{
    public class StoneBall : Magic
    {
        public Transform ts_Model;
        public ParticleSystem ps_Bomb;
        
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            if (isBottomPlayer == false) transform.rotation = Quaternion.Euler(0, 180, 0);
            ts_Model.gameObject.SetActive(true);
            
            SetColor();
            
            
            StartCoroutine(AttackCoroutine());
        }
        
        private IEnumerator AttackCoroutine()
        {
            float angle = 0f;
            Vector3 forward = transform.forward;
            while (true)
            {
                transform.position += forward * moveSpeed * Time.deltaTime;
                angle += (isBottomPlayer ? 45f : -45f) * Time.deltaTime;
                ts_Model.rotation = Quaternion.AngleAxis(angle, Vector3.right);
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTargetLayer(other.gameObject) && other.CompareTag("Player") == false)
            {
                //ps_Bomb.Play();
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);

                if (PhotonNetwork.IsConnected && isMine)
                {
                    var bs = other.GetComponentInParent<BaseStat>();
                    controller.AttackEnemyMinion(bs.id, damage, 0f);
                }
            }
            else if (other.CompareTag("Wall"))
            {
                //ps_Bomb.Play();
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
                StopAllCoroutines();
                ts_Model.gameObject.SetActive(false);
                Destroy(2f);
            }
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}
