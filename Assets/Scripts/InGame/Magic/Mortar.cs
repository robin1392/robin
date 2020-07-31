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
        public Transform ts_ShootPoint;
        public int shootCount = 4;

        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            
            SetColor();

            if (PhotonNetwork.IsConnected && isMine)
            {
                StartCoroutine(AttackCoroutine());
            }
            else if (PhotonNetwork.IsConnected == false)
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
            Collider longTarget = null;

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
                if (PhotonNetwork.IsConnected && isMine)
                {
                    //Shoot(controller.targetPlayer);
                    controller.photonView.RPC("FireCannonBall", RpcTarget.All, ts_ShootPoint.position
                        , longTarget.transform.position, damage);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    //Shoot(longTarget.GetComponentInParent<BaseStat>());
                    controller.FireCannonBall(ts_ShootPoint.position
                        , longTarget.transform.position, damage);
                }
            }
        }

        private void Shoot(BaseStat longTarget)
        {
            //var b = PoolManager.instance.ActivateObject<Bullet>("Bullet", ts_ShootPoint.position);
            //if (b != null)
            // {
            //     b.transform.rotation = Quaternion.identity;
            //     b.controller = this;
            //     b.Initialize(targetId, damage, isMine, isBottomPlayer);
            // }

            StartCoroutine(MoveBall(longTarget));
        }

        IEnumerator MoveBall(BaseStat longTarget)
        {
            //var ts = Instantiate(pref_CannonBall, ts_ShootPoint.position, Quaternion.identity).transform;
            var ts = PoolManager.instance.ActivateObject("CannonBall", ts_ShootPoint.position);
            var distance = Vector3.Distance(longTarget.transform.position, transform.position);
            var moveTime = distance / moveSpeed;
            var startPos = ts.position;
            targetPos = longTarget.transform.position;
            var t = 0f;

            float fV_x;
            float fV_y;
            float fV_z;

            float fg;
            float fEndTime;
            float fMaxHeight = 5f;
            float fHeight;
            float fEndHeight;
            float fTime = 0f;
            float fMaxTime = 0.8f;

            fEndHeight = targetPos.y - startPos.y;
            fHeight = fMaxHeight - startPos.y;
            fg = 2 * fHeight / (fMaxTime * fMaxTime);
            fV_y = Mathf.Sqrt(2 * fg * fHeight);

            float a = fg;
            float b = -2 * fV_y;
            float c = 2 * fEndHeight;

            fEndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

            fV_x = -(startPos.x - targetPos.x) / fEndTime;
            fV_z = -(startPos.z - targetPos.z) / fEndTime;

            Vector3 currentPos = new Vector3();
            while (t < fEndTime)
            {
                t += Time.deltaTime;
                // ts.position = Vector3.Slerp(ts_ShootPoint.position, targetPos, t / moveTime);

                currentPos.x = startPos.x + fV_x * t;
                currentPos.y = startPos.y + (fV_y * t) - (0.5f * fg * t * t);
                currentPos.z = startPos.z + fV_z * t;

                ts.position = currentPos;
                
                yield return null;
            }

            var cols = Physics.OverlapSphere(targetPos, 0.5f, targetLayer);
            foreach (var col in cols)
            {
                col.GetComponentInParent<BaseStat>().HitDamage(damage);
            }

            ts.GetComponent<MeshRenderer>().enabled = false;
            ts.GetChild(1).GetComponent<ParticleSystem>().Play();
            //Destroy(ts.gameObject, 2f);
            ts.GetComponent<PoolObjectAutoDeactivate>().Deactive(2f);
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}
