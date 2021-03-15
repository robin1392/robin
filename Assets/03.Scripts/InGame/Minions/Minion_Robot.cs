#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ED
{
    public class Minion_Robot : Minion
    {
        public Transform[] arrTs_Parts;
        public int pieceID;
        
        public override void Initialize()
        {
            base.Initialize();

            // controller.robotPieceCount++;
            // controller.robotEyeTotalLevel += eyeLevel;

            _collider.enabled = false;
            animator.gameObject.SetActive(false);
            animator.SetTrigger(_animatorHashSkill);
            pieceID = controller.robotPieceCount++;
            controller.robotEyeTotalLevel += eyeLevel;

            SetParts();
            Invoke("Fusion", 1.6f);
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

                if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
                StopAllCoroutines();
                InGameManager.Get().RemovePlayerUnit(isBottomCamp, this);

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
                if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
                StopAllCoroutines();
                InGameManager.Get().RemovePlayerUnit(isBottomCamp, this);

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
            //KZSee:
            // maxHealth *= controller.robotEyeTotalLevel;
            controller.robotEyeTotalLevel = 0;
            // currentHealth = maxHealth;

            _collider.enabled = true;
            foreach (var tsPart in arrTs_Parts)
            {
                tsPart.gameObject.SetActive(false);
            }
            animator.gameObject.SetActive(true);
            ChangeLayer(isBottomCamp);
        }
    }
}