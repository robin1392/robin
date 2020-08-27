#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class Mortar : Magic
    {
        public ParticleSystem ps_Fire;
        public Light light_Fire;
        public float shootTime = 0;

        private Transform longTarget;
        private static readonly int animatorHashShoot = Animator.StringToHash("Shoot");

        public Transform[] arrTs_Parts;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            shootTime = 0;
            SetColor();

            if (pIsBottomPlayer == false)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                StartCoroutine(AttackCoroutine());
            }
            
            SetParts();
        }

        private void SetParts()
        {
            for (int i = 0; i < arrTs_Parts.Length; i++)
            {
                arrTs_Parts[i].localScale = i + 1 < eyeLevel ? Vector3.one : Vector3.zero;
            }

            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
        }

        public override void Destroy(float delay = 0)
        {
            base.Destroy(delay);
            
            CancelInvoke("Shoot");
        }

        private IEnumerator AttackCoroutine()
        {
            // for (var i = 0; i < shootCount; i++)
            // {
            //     SetLongTarget();
            //     yield return new WaitForSeconds(5f);
            // }
            //
            var t = 0f;
            var lifeTime = InGameManager.Get().spawnTime;

            while (t < lifeTime)
            {
                yield return null;
                
                t += Time.deltaTime;
                if (shootTime + attackSpeed <= Time.time)
                {
                    SetLongTarget();
                }
            }
        }

        private void SetLongTarget()
        {
            var distance = 0f;
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            longTarget = null;

            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                
                var dis = Vector3.SqrMagnitude(transform.position - col.transform.position);
                if (dis > distance)
                {
                    distance = dis;
                    longTarget = col.transform;
                }
            }

            if (longTarget != null)
            {
                shootTime = Time.time;
                Invoke("Shoot", 0.5f);
                // StartCoroutine(LookAtTargetCoroutine());
                // animator.SetTrigger(animatorHashShoot);
                
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_SENDMESSAGEPARAM1, id, "LookAndAniTrigger", longTarget.GetComponentInParent<BaseStat>().id);
            }
        }

        public void LookAndAniTrigger(int targetID)
        {
            longTarget = controller.targetPlayer.GetBaseStatFromId(targetID)?.transform;
            if (longTarget != null)
            {
                StartCoroutine(LookAtTargetCoroutine());
                animator.SetTrigger(animatorHashShoot);
            }
        }
        
        IEnumerator LookAtTargetCoroutine()
        {
            float t = 0f;
            Quaternion q = transform.rotation;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(q,
                    Quaternion.LookRotation((longTarget.position - transform.position).normalized),
                    t / 0.5f);
                yield return null;
            }
        }

        public void Shoot()
        {
            ps_Fire.Play();
            light_Fire.enabled = true;
            Invoke("FireLightOff", 0.15f);
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIRECANNONBALL , ts_ShootingPos.position, longTarget.position, power, range);
            }
        }
        
        public void EndGameUnit()
        {
            StopAllCoroutines();
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
