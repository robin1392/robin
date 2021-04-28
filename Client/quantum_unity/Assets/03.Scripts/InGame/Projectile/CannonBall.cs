#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace ED
{
    public class CannonBall : Bullet
    {
        public GameObject obj_Model;
        public ParticleSystem ps_Tail;
        public GameObject pref_BombEffect;

        [Header("Value")] 
        public float maxHeight = 1.5f;
        public float maxTime = 0.8f;

        private void Start()
        {
            if (pref_BombEffect != null)
            {
                PoolManager.instance.AddPool(pref_BombEffect, 1);
            }
        }

        public void Initialize(Vector3 pTargetPos, float pDamage, float splashRange, bool pIsMine, bool pIsBottomPlayer)
        {
            base.Initialize(pTargetPos, pDamage, splashRange, pIsMine, pIsBottomPlayer);

            obj_Model.SetActive(true);
            ps_Tail.Clear();
            ps_Tail.Play();
        }

        protected override IEnumerator Move()
        {
            var ts = transform;
            var startPos = ts.position;
            var targetPos = _targetPos;
            var t = 0f;
            //Debug.LogFormat("{0}, {1}", startPos, targetPos);

            float fV_x;
            float fV_y;
            float fV_z;

            float fg;
            float fEndTime;
            float fHeight;
            float fEndHeight;

            fEndHeight = targetPos.y - startPos.y;
            fHeight = maxHeight - startPos.y;
            fg = 2 * fHeight / (maxTime * maxTime);
            fV_y = Mathf.Sqrt(2 * fg * fHeight);

            float a = fg;
            float b = -2 * fV_y;
            float c = 2 * fEndHeight;

            fEndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            //Debug.LogFormat("{0}, {1}, {2}, {3}", a, b, c, fEndTime);

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

            ps_Tail.Stop();
            var cols = Physics.OverlapSphere(targetPos, _splashRange, targetLayer);
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseEntity>();
                if (bs != null && bs.isAlive)
                {
                    if (client.IsPlayingAI)
                    {
                        bs.ActorProxy.HitDamage(_damage);
                    }
                }
            }

            obj_Model.SetActive(false);

            if (pref_BombEffect != null)
            {
                PoolManager.instance.ActivateObject(pref_BombEffect.name, transform.position);
            }
            else
            {
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
            }

            SoundManager.instance.Play(Global.E_SOUND.SFX_COMMON_EXPLOSION);
            _poad.Deactive(3.5f);
        }
    }
}
