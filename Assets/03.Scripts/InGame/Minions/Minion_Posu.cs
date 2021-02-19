using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Posu : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public GameObject pref_Bullet;

        [Header("AudioClip")]
        public AudioClip clip_Fire;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            if (light_Fire != null) light_Fire.enabled = false;
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            SoundManager.instance.Play(clip_Fire);
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, _arrow, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireBullet(E_BulletType.POSU_BULLET, id, target.id, power, bulletMoveSpeed);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                controller.FireBullet(E_BulletType.POSU_BULLET, id, target.id, power, bulletMoveSpeed);
            }
        }

        public void FireLightOn()
        {
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }

            if (light_Fire != null)
            {
                light_Fire.enabled = true;
                Invoke("FireLightOff", 0.15f);
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