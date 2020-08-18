using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class LightningRod : Magic
    {
        public Transform ts_ShootPoint;
        public float lifeTime = 20f;
        public float tick = 0.1f;
        
        private List<Collider> list = new List<Collider>();
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            list.Clear();
            StartCoroutine(AttackCoroutine());
            
            SetColor();
        }

        private IEnumerator AttackCoroutine()
        {
            float t = 0;
            float shootTime = tick;
            while (t < lifeTime)
            {
                if (t >= shootTime)
                {
                    shootTime += tick;
                    Shoot();
                }
                
                t += Time.deltaTime;
                yield return null;
            }
            Destroy();
        }

        private void Shoot()
        {
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            foreach (var col in cols)
            {
                if (list.Contains(col) == false)
                {
                    list.Add(col);
                    controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Lightning",
                        col.transform.position, Quaternion.identity, Vector3.one);
                    
                    // Damage and sturn
                    var cols2 = Physics.OverlapSphere(col.transform.position, 0.5f, targetLayer);
                    foreach (var col2 in cols2)
                    {
                        BaseStat bs = col2.GetComponentInParent<BaseStat>();
                        DamageToTarget(bs);
                        controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_STURNMINION, bs.id, 1f);
                    }
                }
            }
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
        }
    }
}
