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
        public static int[] pieceCount;
        public static int[] eyeTotalLevel;

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
            pieceCount[isBottomPlayer ? 0 : 1]++;
            eyeTotalLevel[isBottomPlayer ? 0 : 1] += eyeLevel;
            pieceID = pieceCount[isBottomPlayer ? 0 : 1];

            Invoke("Fusion", 1.6f);
        }

        public override void Attack()
        {
            if (target == null) return;

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

        public void Fusion()
        {
            if (pieceCount[isBottomPlayer ? 0 : 1] == 4)
            {
                transform.DOMove(controller.transform.position + controller.transform.forward * 2, 0.5f).OnComplete(Callback_MoveComplete);
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
                PoolManager.instance.ActivateObject("Effect_Death", hitPos.position);
                _poolObjectAutoDeactivate.Deactive();
            }
        }

        private void Callback_MoveComplete()
        {
            if (pieceID == 4)
            {
                PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);

                maxHealth *= eyeTotalLevel[isBottomPlayer ? 0 : 1];
                currentHealth = maxHealth;

                _collider.enabled = true;
                obj_Piece.SetActive(false);
                animator.gameObject.SetActive(true);
                SetControllEnable(true);
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