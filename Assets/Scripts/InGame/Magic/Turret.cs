#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Turret : Magic
    {
        public Animator animator;
        public Transform ts_ShootPoint;
        public float lifeTime = 20f;
        public Minion flyingTarget;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

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
            var t = 0f;
            
            while (t < lifeTime)
            {
                SetFlyingTarget();
                t += attackSpeed;
                yield return new WaitForSeconds(attackSpeed);
            }
            Destroy();
        }

        private void SetFlyingTarget()
        {
            var distance = float.MaxValue;
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            Collider colTarget = null;

            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                
                var dis = Vector3.SqrMagnitude(transform.position - col.transform.position);
                if (dis < distance)
                {
                    distance = dis;
                    colTarget = col;
                }
            }

            if (colTarget != null)
            {
                flyingTarget = colTarget.GetComponentInParent<Minion>();
                
                if (PhotonNetwork.IsConnected && isMine)
                {
                    //controller.photonView.RPC("FireArrow", RpcTarget.All, ts_ShootPoint.position, flyingTarget.id, damage);
                    controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREARROW , ts_ShootPoint.position, flyingTarget.id, power);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    controller.FireArrow(ts_ShootPoint.position, flyingTarget.id, power);
                }
            }
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}
