using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

namespace ED
{
    public class CannonBall : Bullet
    {
        public GameObject obj_Model;
        public ParticleSystem ps_Bomb;

        public override void Initialize(int pTargetId, float pDamage, bool pIsMine, bool pIsBottomPlayer, UnityAction pCallback = null)
        {
            base.Initialize(pTargetId, pDamage, pIsMine, pIsBottomPlayer, pCallback);
            
            obj_Model.SetActive(true);
        }

        protected override IEnumerator Move()
        {
            var ts = transform;
            var startPos = ts.position;
            var targetPos = _targetPos;
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

            var currentPos = new Vector3();
            while (t < fEndTime)
            {
                t += Time.deltaTime;

                currentPos.x = startPos.x + fV_x * t;
                currentPos.y = startPos.y + (fV_y * t) - (0.5f * fg * t * t);
                currentPos.z = startPos.z + fV_z * t;

                ts.position = currentPos;
                
                yield return null;
            }

            var cols = Physics.OverlapSphere(targetPos, 0.5f, targetLayer);
            foreach (var col in cols)
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && _isMine)
                {
                    controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.All, col.GetComponentInParent<BaseStat>().id, _damage, 0f);
                }
                else if (PhotonNetwork.IsConnected == false)
                {
                    controller.targetPlayer.HitDamageMinion(col.GetComponentInParent<BaseStat>().id, _damage, 0f);
                }
            }

            obj_Model.SetActive(false);
            ps_Bomb.Play();
            
            _poad.Deactive(2f);
        }
    }
}
