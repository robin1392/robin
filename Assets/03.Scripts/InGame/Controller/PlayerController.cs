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

        public override void ChangeLayer(bool pIsBottomPlayer)
        {
            base.ChangeLayer(pIsBottomPlayer);
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
            objCollider.layer = LayerMask.NameToLayer(layerName);
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

        public override void OnBaseStatDestroyed()
        {
            base.OnBaseStatDestroyed();
            
            image_HealthBar.transform.parent.parent.gameObject.SetActive(false);
            Transform ts = PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
            animator.SetTrigger("Death");
            SoundManager.instance.Play(clip_TowerExplosion);
            ps_Destroy.gameObject.SetActive(true);
            
            //ActorProxy가 파괴되기 전 밖으로 빼놓는다. 
            transform.SetParent(null);
        }
    }
}