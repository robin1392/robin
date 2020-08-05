using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_BabyDragon : Minion
    {
        public ParticleSystem ps_Smoke;
        public float polymophCooltime = 20f;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            StartCoroutine(PolymophCoroutine());
        }

        public override void Attack()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireSpear()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                controller.photonView.RPC("FireSpear", RpcTarget.All, shootingPos.position, target.id, power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller?.FireSpear(shootingPos.position, target.id, power);
            }
        }

        IEnumerator PolymophCoroutine()
        {
            animator.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(polymophCooltime);

            ps_Smoke.Play();
            animator.transform.localScale = Vector3.one * 2f;
            power *= 10;
            maxHealth *= 10;
            currentHealth = maxHealth;
        }
    }
}