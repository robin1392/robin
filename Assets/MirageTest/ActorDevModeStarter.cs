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
        var masterAuth = Master.GetComponent<RWAthenticator>();
        var otherAuth = Other.GetComponent<RWAthenticator>();
        Server.MatchData.AddPlayerInfo(masterAuth.LocalId, masterAuth.LocalName, 0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));
        Server.MatchData.AddPlayerInfo(otherAuth.LocalId, otherAuth.LocalName, 0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));
        
        Server.ListenAsync().Forget();
        while (Server.Active == false)
        {
            await UniTask.Yield();
        }

        await Master.ConnectAsync("localhost");
        await Other.ConnectAsync("localhost");
    }
}
