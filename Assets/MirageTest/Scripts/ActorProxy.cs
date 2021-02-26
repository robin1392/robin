using System;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class ActorProxy : NetworkBehaviour
    {
        public Actor actor;
        public ActorPathfinding actorPathfinding;
        public ActorAI actorAi;
        public Renderer renderer;

        [SyncVar] public string owner;
        [SyncVar(hook = nameof(SetTeam))] public int team;
        [SyncVar] public byte spawnSlot; //0 ~ 14 주사위 필드 슬롯, 15 바텀 , 16 탑, 17 보스 

        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStartServer.AddListener(StartServer);
        }

        public void SetTeam(int oldValue, int newValue)
        {
            actor.Team = newValue;
            renderer.material.color = newValue == 1? Color.blue : Color.red;
        }
        
        private void StartServer()
        {
        }

        private void StartClient()
        {
            actorPathfinding.EnableAIPathfinding(false);
            actorAi.graphOwner.StopBehaviour();
            actorAi.graphOwner.enabled = false;
            GetComponentInChildren<Collider>().enabled = false;
            // gameObject.SetActive(false);
        }

        private void Update()
        {
            if (ServerObjectManager == null)
            {
                return;
            }
            
            if (actor.IsAlive == false)
            {
                ServerObjectManager.Destroy(gameObject);
            }
        }
    }
}