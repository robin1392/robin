using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Zombie : Minion
    {
        public ParticleSystem ps_PoisonCloud;
        public Animator animator_Alive;
        public Animator animator_Dead;

        [SerializeField]
        private int _reviveCount = 1;

        public override void Initialize(DestroyCallback destroy)
        {
            animator = animator_Alive;
            animator_Alive.gameObject.SetActive(true);
            animator_Dead.gameObject.SetActive(false);
            _reviveCount = 1;

            base.Initialize(destroy);
        }

        public override void Attack()
        {
            if (target == null) return;

            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public override void Death()
        {
            if (_reviveCount > 0)
            {
                StartCoroutine(ReviveCoroutine());
            }
            else
            {
                base.Death();
            }
        }

        IEnumerator ReviveCoroutine()
        {
            _collider.enabled = false;
            _reviveCount--;
            SetControllEnable(false);
            animator.gameObject.SetActive(false);

            StartCoroutine(PoisonCoroutine(2f));
            yield return new WaitForSeconds(2f);

            currentHealth = (eyeLevel * 15) * 0.01f * maxHealth;
            RefreshHealthBar();
            animator = animator_Dead;
            animator.gameObject.SetActive(true);
            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
            SetControllEnable(true);
            _collider.enabled = true;
        }

        IEnumerator PoisonCoroutine(float duration)
        {
            PoolManager.instance.ActivateObject("Effect_Poison", transform.position);
            float t = 0;
            float tick = 0.1f;
            
            while (t < duration)
            {
                var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
                foreach (var col in cols)
                {
                    var bs = col.GetComponentInParent<BaseStat>();
                    if (bs != null && bs.id > 0 && bs.isFlying == false && bs.isAlive)
                    {
                        //controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, bs.id, effect * 10, 0f);
                        controller.AttackEnemyMinionOrMagic(bs.id, effect, 0f);
                        //controller.HitMinionDamage( true , bs.id , effect * 10, 0f);
                    }
                }
                
                t += tick;
                yield return new WaitForSeconds(tick);
            }
        }
    }
}