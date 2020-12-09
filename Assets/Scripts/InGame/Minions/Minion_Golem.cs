using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ED
{
    public class Minion_Golem : Minion
    {
        public GameObject pref_MiniGolem;

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
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

            /*base.Attack();
            //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            controller.MinionAniTrigger(id, "Attack");*/
        }

        public override BaseStat SetTarget()
        {
            switch (NetworkManager.Get().playType)
            {
                case Global.PLAY_TYPE.BATTLE:
                    return controller.targetPlayer;
                case Global.PLAY_TYPE.COOP:
                    return controller.coopPlayer;
                default:
                    return null;
            }
        }

        public override void Death()
        {
            for (int i = 0; i < 2; i++)
            {
                // Spawn
                var m = controller.CreateMinion(pref_MiniGolem,
                    transform.position + Vector3.right * Random.Range(-0.5f, 0.5f) + Vector3.forward * Random.Range(-0.5f, 0.5f));

                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomPlayer);
                m.power = effect;
                m.maxHealth = maxHealth * eyeLevel * 0.1f;
                m.attackSpeed = attackSpeed;
                m.moveSpeed = moveSpeed;
                m.range = range;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(destroyCallback);
            }

            base.Death();
        }
    }
}