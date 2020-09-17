#if UNITY_EDITOR
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
        public GameObject obj_Piece;

        private int pieceID;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            currentHealth = 0;
            SetControllEnable(false);
            _collider.enabled = false;
            obj_Piece.SetActive(true);
            animator.gameObject.SetActive(false);
            controller.robotPieceCount++;
            controller.robotEyeTotalLevel += eyeLevel;
            pieceID = controller.robotPieceCount;

            Invoke("Fusion", 1.6f);
        }

        public override void Attack()
        {
            if (target == null) return;

            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Attack");
                controller.MinionAniTrigger(id, "Attack");
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
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
            if (pieceID == 4)
            {
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);

                maxHealth *= controller.robotEyeTotalLevel;
                currentHealth = maxHealth;

                _collider.enabled = true;
                obj_Piece.SetActive(false);
                animator.gameObject.SetActive(true);
                SetControllEnable(true);
                ChangeLayer(isBottomPlayer);
            }
            else
            {
                SetControllEnable(false);
                isPlayable = false;
                if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
                StopAllCoroutines();
                InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);

                destroyCallback(this);
                _poolObjectAutoDeactivate.Deactive();
            }
        }
    }
}