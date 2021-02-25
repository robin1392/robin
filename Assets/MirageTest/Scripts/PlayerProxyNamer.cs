//TODO : 개발 편의 스크립트이다. 배포 시에 제거해서 내보낸다.

using Mirage;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerProxyNamer : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetNameClientHook))]
    public string Name;

    private void Awake()
    {
        NetIdentity.OnStartLocalPlayer.AddListener(StartLocalPlayer);
    }

    private void StartLocalPlayer()
    {
        SetNameOnServer(Client.transform.name);
    }

    [ServerRpc]
    public void SetNameOnServer(string name)
    {
        gameObject.name = name;
        Name = name;
    }

    void SetNameClientHook(string oldName, string newName)
    {
        gameObject.name = newName;
    }
}