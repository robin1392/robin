using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Ranger : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            if (light_Fire != null) light_Fire.enabled = false;
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

        public void FireArrow()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }
            
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }

            if (light_Fire != null)
            {
                light_Fire.enabled = true;
                Invoke("FireLightOff", 0.15f);
            }
            
            if (PhotonNetwork.IsConnected && isMine)
            {
                //controller.photonView.RPC("FireArrow", RpcTarget.All, shootingPos.position, target.id, power);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, _arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireBullet(_arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
        }

        private void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }
    }
}