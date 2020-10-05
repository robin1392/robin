using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Bomber : Minion
    {
        public GameObject pref_Bullet;
        public ParticleSystem ps_Fire;
        
        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        protected override void Start()
        {
            base.Start();
            
            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLight;
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;

            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
        }

        public void FireArrow()
        {
            if (target != null)
            {
                //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
                if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
                {
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRECANNONBALL, E_CannonType.BOMBER, ts_ShootingPos.position, target.transform.position, power, 2f);
                    controller.ActionFireCannonBall(E_CannonType.BOMBER , ts_ShootingPos.position, target.transform.position, power, 2f);
                }
            }
        }

        public void FireLight()
        {
            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }
        }
    }
}
