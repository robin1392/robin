using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

namespace ED
{
    public class Minion_BabyDragon : Minion
    {
        public Animator ani_Baby;
        public Animator ani_Dragon;
        public Transform ts_HitPosBaby;
        public Transform ts_HitPosDragon;
        public ParticleSystem ps_Smoke;
        public float polymophCooltime = 20f;

        private float originRange;
        private readonly string strTagGround = "Minion_Ground"; 
        private readonly string strTagFlying = "Minion_Flying"; 

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            gameObject.tag = strTagGround;
            ani_Baby.gameObject.SetActive(true);
            ani_Dragon.gameObject.SetActive(false);
            animator = ani_Baby;
            ts_HitPos = ts_HitPosBaby;
            agent.baseOffset = 0;
            originRange = range;
            range = 0.7f;
            StartCoroutine(PolymorphCoroutine());
        }

        public override void Attack()
        {
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

        public void FireSpear()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                //controller.photonView.RPC("FireSpear", RpcTarget.All, shootingPos.position, target.id, power);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRESPEAR, ts_ShootingPos.position, target.id, power);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller?.FireSpear(ts_ShootingPos.position, target.id, power);
            }
        }

        IEnumerator PolymorphCoroutine()
        {
            yield return new WaitForSeconds(polymophCooltime);

            gameObject.tag = strTagFlying;
            ani_Baby.gameObject.SetActive(false);
            ani_Dragon.gameObject.SetActive(true);
            animator = ani_Dragon;
            ts_HitPos = ts_HitPosDragon;
            agent.baseOffset = -2f;
            range = originRange;
            
            ps_Smoke.Play();
            power *= 10;
            maxHealth *= 10;
            currentHealth = maxHealth;
        }
    }
}