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

        [SyncVar] public int owner;
        [SyncVar(hook = nameof(SetTeam))] public int team;
        
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
            SetTeamInternal(newValue);
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

        public void SetTeamInternal(int team)
        {
            actor.Team = team;
            renderer.material.color =  team == 1? Color.blue : Color.red;
        }
    }
}