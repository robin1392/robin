using System;
using ED;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class ActorProxy : NetworkBehaviour
    {
        // public Actor actor;
        // public ActorPathfinding actorPathfinding;
        // public ActorAI actorAi;
        // public Renderer renderer;

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

        public ReferenceHolder referenceHolder;

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
            if (actorType == ActorType.Tower)
            {
                var towerPrefab = Resources.Load<ReferenceHolder>("Mirage/Tower");
                referenceHolder = Instantiate(towerPrefab, transform);
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
            if (referenceHolder == null)
            {
                return;
            }
            
            referenceHolder.image_HealthBar.fillAmount = hp / maxHp;
            referenceHolder.text_Health.text = $"{Mathf.CeilToInt(hp)}";
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