using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Support : Minion
    {
        public ParticleSystem ps_Dust;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            StartCoroutine(Jump());
        }

        public override void Attack()
        {
            if (target == null) return;
            if (_collider.enabled == false) _collider.enabled = true;
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        private Minion GetLongDistanceFriendlyTarget()
        {
            Minion rtn = null;

            var distance = isBottomPlayer ? float.MinValue : float.MaxValue;
            
            foreach (var minion in controller.listMinion)
            {
                if ((isBottomPlayer && minion.transform.position.z > distance) ||
                    (isBottomPlayer == false && minion.transform.position.z < distance))
                {
                    distance = minion.transform.position.z;
                    rtn = minion;
                }
            }
            
            return rtn == this ? null : rtn;
        }

        private IEnumerator Jump()
        {
            SetControllEnable(false);
            _collider.enabled = false;
            var m = GetLongDistanceFriendlyTarget();
            controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");

            if (m == null)
            {
                SetControllEnable(true);
                _collider.enabled = false;
                yield break;
            }

            transform.LookAt(m.transform);
            var ts = transform;
            var startPos = ts.position;
            var targetPos = m.transform.position;
            var t = 0f;

            float fV_x;
            float fV_y;
            float fV_z;

            float fg;
            float fEndTime;
            float fMaxHeight = 2f;
            float fHeight;
            float fEndHeight;
            float fTime = 0f;
            float fMaxTime = 0.75f;

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

            SetControllEnable(true);
            _collider.enabled = true;
            ps_Dust.Play();
        }
    }
}
