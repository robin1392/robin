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
    public class Tower : BaseEntity
    {
        #region game variable

        public Collider collider;
        
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

        public override float Radius
        {
            get
            {
                if (collider is SphereCollider sphereCollider)
                {
                    return sphereCollider.radius;
                }

                return 0;
            }
        }

        protected void Awake()
        {
            collider = GetComponentInChildren<Collider>();
        }

        public override void ChangeLayer(bool pIsBottomPlayer)
        {
            base.ChangeLayer(pIsBottomPlayer);
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
            objCollider.layer = LayerMask.NameToLayer(layerName);
        }
        
        private void Update()
        {
            RefreshHealthBar();
        }

        public void RefreshHealthBar()
        {
            if (image_HealthBar == null)
            {
                return;
            }

            if (isAlive == false)
            {
                return;
            }

            image_HealthBar.fillAmount = ActorProxy.currentHealth / ActorProxy.maxHealth;
            text_Health.text = $"{Mathf.CeilToInt(ActorProxy.currentHealth)}";
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

        public override void OnBaseEntityDestroyed()
        {
            base.OnBaseEntityDestroyed();
            
            image_HealthBar?.transform.parent.gameObject.SetActive(false);
            image_HealthBar = null;
            text_Health = null;
            Transform ts = PoolManager.instance.ActivateObject("Effect_Bomb", transform.position);
            animator.SetTrigger("Death");
            SoundManager.instance.Play(clip_TowerExplosion);
            ps_Destroy.gameObject.SetActive(true);
            
            //ActorProxy가 파괴되기 전 밖으로 빼놓는다. 
            transform.SetParent(null);
        }
    }
}