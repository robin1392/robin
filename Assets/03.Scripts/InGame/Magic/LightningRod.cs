#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class LightningRod : Magic
    {
        public Transform ts_ShootPoint;
        public readonly float tick = 0.1f;

        public Transform[] arrTs_Parts;

        [Header("Prefab")]
        public GameObject pref_FireEffect;
        
        private List<Collider> list = new List<Collider>();

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_FireEffect, 1);
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            list.Clear();
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
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

        private IEnumerator AttackCoroutine()
        {
            float t = 0;
            float shootTime = tick;
            float lifeTime = InGameManager.Get().spawnTime;
            while (t < lifeTime)
            {
                if (t >= shootTime)
                {
                    shootTime += tick;
                    Shoot();
                }
                
                t += Time.deltaTime;
                //Debug.LogFormat("AttackCoroutine t:{0}", t);
                yield return null;
            }
            //Debug.Log(gameObject.name + " Destroy !!");
            //Destroy();
        }

        private void Shoot()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            foreach (var col in cols)
            {
                if (list.Contains(col) == false)
                {
                    list.Add(col);
                    BaseStat bs = col.GetComponentInParent<BaseStat>();
                    if (bs != null)
                    {
                        //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, pref_FireEffect.name, ts_ShootPoint.position, Quaternion.identity, Vector3.one);
                        //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Lightning", bs.ts_HitPos.position, Quaternion.identity, Vector3.one);
                        
                        controller.ActionActivePoolObject(pref_FireEffect.name, col.transform.position, Quaternion.identity, Vector3.one);
                        controller.ActionActivePoolObject("Effect_Lightning", col.transform.position, Quaternion.identity, Vector3.one);

                        // Damage and sturn
                        var cols2 = Physics.OverlapSphere(col.transform.position, 0.5f, targetLayer);
                        foreach (var col2 in cols2)
                        {
                            bs = col2.GetComponentInParent<BaseStat>();
                            DamageToTarget(bs);
                            //controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_STURNMINION, bs.id, 1f);
                            controller.ActionSturn(true , bs.id , 1f);
                        }
                    }
                }
            }
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }

        public override void SetTarget()
        {
            
        }
    }
}
