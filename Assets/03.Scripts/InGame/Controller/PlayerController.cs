#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.AI;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Table;

//
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TDataDiceInfo = RandomWarsResource.Data.TDataDiceInfo;


#region photon

//using Photon.Pun;

#endregion

namespace ED
{
    public class PlayerController : BaseStat
    {
        #region game variable

        public UI_DiceField uiDiceField;
        public GameObject objCollider;
        public ParticleSystem ps_ShieldOff;
        public ParticleSystem ps_Destroy;
        public GameObject pref_Guardian;

        public int robotPieceCount;

        [Header("AudioClip")] public AudioClip clip_TowerFalldown;
        public AudioClip clip_TowerExplosion;

        #endregion

        public bool isPlayingAI => ActorProxy.isPlayingAI;

        protected void Awake()
        {
        }

        protected override void Start()
        {
            StartPlayerControll();
        }

        private void Update()
        {
            RefreshHealthBar();
        }
        
        public void RefreshHealthBar()
        {
            if (isAlive)
            {
                image_HealthBar.fillAmount = ActorProxy.currentHealth / ActorProxy.maxHealth;
                text_Health.text = $"{Mathf.CeilToInt(ActorProxy.currentHealth)}";
            }
        }
        
        protected virtual void StartPlayerControll()
        {
        }

        public override void SetColor(E_MaterialType type, bool isAlly)
        {
            var mat = arrMaterial[isAlly ? 0 : 1];
            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                if (m.gameObject.CompareTag("Finish")) continue;

                m.material = mat;

                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.3f;
                        m.material.color = c;
                        break;
                }
            }
        }

        // 네트워크 변환하면서 필요없어지긴 했으나 
        // 싱글 모드 테스트 일때는 게임 끝내는걸 호출해준다
        // 네트워크 모드에선 게임의 끝은 서버가 판단해준다
        private void Death()
        {
            //KZSee:
            // if (InGameManager.Get().isGamePlaying)
            {
                image_HealthBar.transform.parent.parent.gameObject.SetActive(false);
                //KZSee:ActionActivePoolObject("Effect_Bomb", transform.position, Quaternion.identity, Vector3.one);
                animator.SetTrigger("Death");
                SoundManager.instance.Play(clip_TowerExplosion);
                ps_Destroy.gameObject.SetActive(true);

                // 연결은 안되었으나 == 싱글모드 일때 && 내 타워라면
                if (InGameManager.IsNetwork == false)
                {
                    InGameManager.Get().EndGame(!isMine, 1, null, null, null);
                }
            }
        }

        public void SendEventLog_BatCheck()
        {
            //KZSee: 게임 종료 시에 덱 정보를 아날리틱스로 보낸다  최대데미지와 최대 눈금을 주사위 아이디별로 저장해서 보낸
            // var param = new Firebase.Analytics.Parameter[25];
            // for(int i = 0; i < arrDiceDeck.Length; i++)
            // {
            //     int diceID = arrDiceDeck[i].id;
            //     param[i * 5] = new Firebase.Analytics.Parameter($"dice{i + 1}_id", diceID);
            //     param[i * 5 + 1] = new Firebase.Analytics.Parameter($"dice{i + 1}_level", UserInfoManager.Get().GetUserInfo().dicGettedDice[diceID][0]);
            //     param[i * 5 + 2] = new Firebase.Analytics.Parameter($"dice{i + 1}_max_eye", _dicMaxEye[diceID]);
            //     param[i * 5 + 3] = new Firebase.Analytics.Parameter($"dice{i + 1}_max_upgrade", arrUpgradeLevel[i]);
            //     param[i * 5 + 4] = new Firebase.Analytics.Parameter($"dice{i + 1}_max_damage", _dicMaxDamage[diceID]);
            // }
            //
            // FirebaseManager.Get().LogEvent("bat_check", param);
        }
        
        public void GetDice()
        {
            //Mirage => PlayerProxy GetDice
            // var emptyCount = GetDiceFieldEmptySlotCount();
            //
            // if (emptyCount == 0)
            // {
            //     Debug.Log("DiceField is Full !!");
            //     return;
            // }
            // else
            // {
            //     var emptySlotNum = 0;
            //     do
            //     {
            //         emptySlotNum = Random.Range(0, arrDice.Length);
            //     } while (arrDice[emptySlotNum].id >= 0);
            //
            //     int randomDeckNum = Random.Range(0, arrDiceDeck.Length);
            //     arrDice[emptySlotNum].Set(arrDiceDeck[randomDeckNum]);
            //     
            //     if (uiDiceField != null)
            //     {
            //         uiDiceField.arrSlot[emptySlotNum].ani.SetTrigger("BBoing");
            //         uiDiceField.SetField(arrDice);
            //     }
            // }
        }

        
        public void GetDice(int diceId , int slotNum , int level = 0)
        {
            // arrDice[slotNum].Set(GetArrayDeckDice(diceId));
            //
            // if (uiDiceField != null)
            // {
            //     uiDiceField.arrSlot[slotNum].ani.SetTrigger("BBoing");
            //     uiDiceField.SetField(arrDice);
            // }
            //
            // //
            // uiDiceField.RefreshField();
        }
    }
}