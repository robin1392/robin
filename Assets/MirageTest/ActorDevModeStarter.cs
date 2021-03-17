using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MirageTest.Scripts;
using UnityEngine;

public class ActorDevModeStarter : MonoBehaviour
{
    public RWNetworkServer Server;
    public RWNetworkClient Master;
    public RWNetworkClient Other;
    
    async void Start()
    {
        Server.ListenAsync().Forget();
        while (Server.Active == false)
        {
            await UniTask.Yield();
        }

        await Master.ConnectAsync("localhost");
        await Other.ConnectAsync("localhost");
    }
}
