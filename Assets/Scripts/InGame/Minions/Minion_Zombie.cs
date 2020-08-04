using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Zombie : Minion
    {
        public ParticleSystem ps_PoisonCloud;

        [SerializeField]
        private int _reviveCount = 1;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            _reviveCount = 1;
        }

        public override void Attack()
        {
            if (target == null) return;

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
            _reviveCount--;
            PoolManager.instance.ActivateObject("Effect_Poison", transform.position);
            SetControllEnable(false);

            float t = 0;
            while (t < 2f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            currentHealth = (eyeLevel + upgradeLevel) * maxHealth * 0.1f;

            SetControllEnable(true);
        }
    }
}