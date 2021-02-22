using System;
using System.Collections;
using System.Collections.Generic;
using Mirage;
using UnityEngine;

public class ActorRegisterer : MonoBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(ActorRegisterer));

    public NetworkClient[] clients;
    public NetworkIdentity[] actorPrefabs;

    // Start is called before the first frame update
    public virtual void Start()
    {
        clients = FindObjectsOfType<NetworkClient>();
        foreach (var client in clients)
        {
            var clientObjectManager = client.GetComponent<ClientObjectManager>(); 
            foreach (var actorPrefab in actorPrefabs)
            {
                clientObjectManager.RegisterPrefab(actorPrefab);
            }
        }
    }
}
