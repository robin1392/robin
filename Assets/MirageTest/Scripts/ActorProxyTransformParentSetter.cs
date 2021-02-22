using System.Collections;
using System.Collections.Generic;
using Mirage;
using UnityEngine;

public class ActorProxyTransformParentSetter : NetworkBehaviour
{
    private void Awake()
    {
        NetIdentity.OnStartClient.AddListener(StartClient);
        NetIdentity.OnStartServer.AddListener(StartServer);
    }

    private void StartServer()
    {
        var parent = GetOrCreateActorParent(Server.transform);
        ChangeParent(parent);
    }

    private void StartClient()
    {
        var parent = GetOrCreateActorParent(Client.transform);
        ChangeParent(parent);
    }

    Transform GetOrCreateActorParent(Transform root)
    {
        var parentName = "Actors";
        
        var parent = root.Find(parentName);
        if (parent != null)
        {
            return parent;
        }

        parent = new GameObject(parentName).transform;
        parent.parent = root;
        return parent;
    }

    void ChangeParent(UnityEngine.Transform root)
    {
        gameObject.transform.SetParent(root, false);
    }
}