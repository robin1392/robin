using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Knight : Minion
    {
        [Header("Audio Clip")]
        public AudioClip clip_Blade;
        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            _animationEvent.event_Attack += AttackSound;
        }

        public override void Death()
        {
            base.Death();

            _animationEvent.event_Attack -= AttackSound;
        }

        public void AttackSound()
        {
            SoundManager.instance.Play(clip_Blade);
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
    }
}