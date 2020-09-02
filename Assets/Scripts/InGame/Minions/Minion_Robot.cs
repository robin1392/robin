﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

namespace ED
{
    public class Minion_Robot : Minion
    {
        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            // currentHealth = 0;
            // SetControllEnable(false);
            // _collider.enabled = false;
            // animator.gameObject.SetActive(false);
            // controller.robotPieceCount++;
            // controller.robotEyeTotalLevel += eyeLevel;

            //Invoke("Fusion", 1.6f);
            
            animator.SetTrigger(_animatorHashSkill);
        }

        public override void Attack()
        {
            if (target == null) return;

            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void Fusion()
        {
            if (controller.robotPieceCount == 4)
            {
                Vector3 fusionPosition = controller.transform.position;
                fusionPosition.z += fusionPosition.z > 0 ? -2f : 2f;
                transform.DOMove(fusionPosition, 0.5f).OnComplete(Callback_MoveComplete);
            }
            else
            {
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);

                SetControllEnable(false);
                isPlayable = false;
                if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
                StopAllCoroutines();
                InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);

                destroyCallback(this);
                PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
                _poolObjectAutoDeactivate.Deactive();
            }
        }

        private void Callback_MoveComplete()
        {
            // if (pieceID == 4)
            // {
            //     PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
            //
            //     maxHealth *= controller.robotEyeTotalLevel;
            //     currentHealth = maxHealth;
            //
            //     _collider.enabled = true;
            //     obj_Piece.SetActive(false);
            //     animator.gameObject.SetActive(true);
            //     SetControllEnable(true);
            //     ChangeLayer(isBottomPlayer);
            // }
            // else
            // {
            //     SetControllEnable(false);
            //     isPlayable = false;
            //     if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
            //     StopAllCoroutines();
            //     InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);
            //
            //     destroyCallback(this);
            //     _poolObjectAutoDeactivate.Deactive();
            // }
        }
    }
}