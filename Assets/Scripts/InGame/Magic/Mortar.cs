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
        public Animator animator;
        public Transform ts_ShootPoint;
        public int shootCount = 4;

        private Collider longTarget;
        private static readonly int animatorHashShoot = Animator.StringToHash("Shoot");

        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            
            SetColor();

            if (pIsBottomPlayer == false)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                StartCoroutine(AttackCoroutine());
            }
        }

        private IEnumerator AttackCoroutine()
        {
            for (var i = 0; i < shootCount; i++)
            {
                SetLongTarget();
                yield return new WaitForSeconds(5f);
            }
            Destroy();
        }

        private void SetLongTarget()
        {
            var distance = 0f;
            var cols = Physics.OverlapSphere(transform.position, 15f, targetLayer);
            longTarget = null;

            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                
                var dis = Vector3.SqrMagnitude(transform.position - col.transform.position);
                if (dis > distance)
                {
                    distance = dis;
                    longTarget = col;
                }
            }

            if (longTarget != null)
            {
                animator.SetTrigger(animatorHashShoot);
                StartCoroutine(LookAtTargetCoroutine(longTarget.transform));
                Invoke("Shoot", 0.5f);
            }
        }

        IEnumerator LookAtTargetCoroutine(Transform lookTarget)
        {
            float t = 0f;
            Quaternion q = transform.rotation;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(q,
                    Quaternion.LookRotation((lookTarget.transform.position - transform.position).normalized),
                    t / 0.5f);
                yield return null;
            }
        }

        public void Shoot()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                //Shoot(controller.targetPlayer);
                //controller.photonView.RPC("FireCannonBall", RpcTarget.All, ts_ShootPoint.position, longTarget.transform.position, damage);
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIRECANNONBALL , ts_ShootPoint.position, longTarget.transform.position, damage);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                //Shoot(longTarget.GetComponentInParent<BaseStat>());
                controller.FireCannonBall(ts_ShootPoint.position,longTarget.transform.position, damage);
            }
        }
        
        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}
