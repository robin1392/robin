using System.Collections;
using System.Collections.Generic;
using Mirage;
using UnityEngine;

public class PlayerProxyTransformParentSetter : NetworkBehaviour
{
    private void Awake()
    {
        NetIdentity.OnStartClient.AddListener(StartClient);
        NetIdentity.OnStartServer.AddListener(StartServer);
    }

    private void StartServer()
    {
        ChangeParent(Server.transform);
    }

    private void StartClient()
    {
        ChangeParent(Client.transform);
    }

    void ChangeParent(UnityEngine.Transform root)
    {
        gameObject.transform.SetParent(root, false);
    }
}
