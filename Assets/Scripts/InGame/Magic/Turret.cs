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
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public Transform ts_Head;
        public Transform ts_ShootPoint;
        //public float lifeTime = 20f;
        public Minion flyingTarget;
        public float bulletMoveSpeed = 6f;
        public float shootTime = 0;

        public Transform[] arrTs_Parts;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            shootTime = 0;
            SetColor();

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
            ts_Head.rotation = isBottomPlayer ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        }

        private IEnumerator AttackCoroutine()
        {
            var t = 0f;
            var lifeTime = InGameManager.Get().spawnTime;

            while (t < lifeTime)
            {
                t += Time.deltaTime;
                if (shootTime + attackSpeed <= Time.time)
                {
                    SetFlyingTarget();
                }
                
                yield return null;
            }
            
            //Destroy();
        }

        public void FireArrow()
        {
            ps_Fire.Play();
            light_Fire.enabled = true;
            Invoke("FireLightOff", 0.15f);
            
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.Get().IsNetwork() && isMine)
            {
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREARROW , ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
                controller.ActionFireArrow(ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.Get().IsNetwork() == false)
            {
                controller.FireArrow(ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
            }
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
                shootTime = Time.time;
                flyingTarget = colTarget.GetComponentInParent<Minion>();
                
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_SENDMESSAGEPARAM1, id, "LookAndAniTrigger", flyingTarget.id);
                controller.ActionSendMsg(id, "LookAndAniTrigger", flyingTarget.id);
            }
        }

        public void LookAndAniTrigger(int targetID)
        {
            flyingTarget = (Minion)controller.targetPlayer.GetBaseStatFromId(targetID);
            StartCoroutine(LookAtTargetCoroutine());
            animator.SetTrigger("Attack");
        }

        private IEnumerator LookAtTargetCoroutine()
        {
            float t = 0;
            while (t < 0.5f)
            {
                ts_Head.rotation = Quaternion.RotateTowards(ts_Head.rotation,
                    Quaternion.LookRotation((flyingTarget.transform.position - ts_Head.position).normalized),
                    Time.deltaTime * 540f);
                t += Time.deltaTime;
                yield return null;
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
