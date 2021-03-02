using System;
using ED;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class ActorProxy : NetworkBehaviour
    {
        [SyncVar] public byte ownerTag;
        [SyncVar(hook = nameof(SetTeam))] public byte team;
        [SyncVar] public byte spawnSlot; //0 ~ 14 필드 슬롯 
        [SyncVar] public ActorType actorType;
        [SyncVar] public int dataId;
        
        [SyncVar(hook = nameof(SetHp))] public float hp;
        [SyncVar] public float maxHp;
        [SyncVar] public float power;
        [SyncVar] public float range;
        [SyncVar] public float effect;
        [SyncVar] public float attackSpeed;
        [SyncVar] public byte diceScale;
        [SyncVar] public byte ingameUpgradeLevel;

        public BaseStat baseStat;

        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStartServer.AddListener(StartServer);
        }

        public void SetTeam(byte oldValue, byte newValue)
        {
            // renderer.material.color = newValue == 1? Color.blue : Color.red;
        }
        
        private void StartServer()
        {
        }

        private void StartClient()
        {
            var client = Client as RWNetworkClient;
            if (client.enableActor)
            {
                SpawnActor();
            }
        }

        void SpawnActor()
        {
            if (actorType == ActorType.Tower)
            {
                var towerPrefab = Resources.Load<PlayerController>("Tower/Player");
                baseStat = Instantiate(towerPrefab, transform);
            }
            
            // actorPathfinding.EnableAIPathfinding(false);
            // actorAi.graphOwner.StopBehaviour();
            // actorAi.graphOwner.enabled = false;
            // GetComponentInChildren<Collider>().enabled = false;
            // gameObject.SetActive(false);
            
            RefreshHpUI();
        }
        
        
        public void SetHp(float oldValue, float newValue)
        {
            RefreshHpUI();
        }

        void RefreshHpUI()
        {
            if (baseStat == null)
            {
                return;
            }
            
            baseStat.image_HealthBar.fillAmount = hp / maxHp;
            baseStat.text_Health.text = $"{Mathf.CeilToInt(hp)}";
        }

        private void Update()
        {
            if (ServerObjectManager == null)
            {
                return;
            }
            
            // if (actor.IsAlive == false)
            // {
            //     ServerObjectManager.Destroy(gameObject);
            // }
        }
    }
}