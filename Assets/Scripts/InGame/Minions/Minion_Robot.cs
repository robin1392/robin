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
        public Transform[] arrTs_Parts;
        public int pieceID;
        
        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            // controller.robotPieceCount++;
            // controller.robotEyeTotalLevel += eyeLevel;

            currentHealth = 0;
            SetControllEnable(false);
            _collider.enabled = false;
            animator.gameObject.SetActive(false);
            animator.SetTrigger(_animatorHashSkill);
            pieceID = controller.robotPieceCount++;
            controller.robotEyeTotalLevel += eyeLevel;

            SetParts();
            Invoke("Fusion", 1.6f);
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
            if (pieceID == 3)
            {
                PoolManager.instance.ActivateObject("Effect_Robot_Summon", transform.position);
                
                Transform();
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

        public void SetParts()
        {
            animator.gameObject.SetActive(false);
            if (pieceID < 4)
            {
                for (int i = 0; i < arrTs_Parts.Length; i++)
                {
                    arrTs_Parts[i].gameObject.SetActive(i == pieceID);
                }
            }
            else
            {
                int rnd = Random.Range(0, arrTs_Parts.Length);
                for (int i = 0; i < arrTs_Parts.Length; i++)
                {
                    arrTs_Parts[i].gameObject.SetActive(i == rnd);
                }
            }
        }

        public void Transform()
        {
            maxHealth *= controller.robotEyeTotalLevel;
            controller.robotEyeTotalLevel = 0;
            currentHealth = maxHealth;

            _collider.enabled = true;
            foreach (var tsPart in arrTs_Parts)
            {
                tsPart.gameObject.SetActive(false);
            }
            animator.gameObject.SetActive(true);
            SetControllEnable(true);
            ChangeLayer(isBottomPlayer);
        }
    }
}