using System;
using Mirage;
using Mirage.Logging;
using MirageTest.Scripts.Messages;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class ClientPlayerProxySpawner : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ClientPlayerProxySpawner));

        public RWNetworkClient client;
        public NetworkIdentity playerPrefab;

        // Start is called before the first frame update
        public void Awake()
        {
            if (playerPrefab == null)
            {
                throw new InvalidOperationException("Assign a player in the PlayerSpawner");
            }

            client.Authenticated.AddListener(OnClientAuthenticated);
            client.GetComponent<ClientObjectManager>().RegisterPrefab(playerPrefab);
        }

        private void OnClientAuthenticated(INetworkPlayer connection)
        {
            connection.Send(new AddPlayerProxyMessage() {userId = client.LocalUserId});
        }
    }
}