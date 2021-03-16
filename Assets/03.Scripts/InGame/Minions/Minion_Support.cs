using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

namespace ED
{
    public class Minion_Support : Minion
    {
        public GameObject pref_Dust;

        [Header("AudioClip")]
        public AudioClip clip_Jump;
        public AudioClip clip_Landing;

        public override void Initialize()
        {
            base.Initialize();

            PoolManager.instance.AddPool(pref_Dust, 1);
            StartCoroutine(Jump());
        }

        private Minion GetLongDistanceFriendlyTarget()
        {
            Minion rtn = null;

            var distance = isBottomCamp ? float.MinValue : float.MaxValue;
            
            foreach (var minion in controller.listMinion)
            {
                if (minion.spawnedTime < 3f && ((isBottomCamp && minion.transform.position.z > distance) ||
                    (isBottomCamp == false && minion.transform.position.z < distance)))
                {
                    distance = minion.transform.position.z;
                    rtn = minion;
                }
            }
            
            return rtn == this ? null : rtn;
        }

        private IEnumerator Jump()
        {
            _collider.enabled = false;
            var m = GetLongDistanceFriendlyTarget();

            if (m == null)
            {
                _collider.enabled = true;
                yield break;
            }

            transform.LookAt(m.transform);
            
            yield return null;
            
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
            SoundManager.instance.Play(clip_Jump);
            
            ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);

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

            _collider.enabled = true;
            var pos = transform.position;
            pos.y = 0.1f;
            //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Support", pos, Quaternion.identity, Vector3.one * 0.8f);
            SoundManager.instance.Play(clip_Landing);
            controller.ActionActivePoolObject("Effect_Support", pos, Quaternion.identity, Vector3.one * 0.8f);
        }
    }
}
