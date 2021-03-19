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
        if (TableManager.Get().Loaded == false)
        {
            string targetPath = System.IO.Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "Dev");
            TableManager.Get().LoadFromFile(targetPath);
        }
        
        var masterAuth = Master.GetComponent<RWAthenticator>();
        Master.localPlayerId = masterAuth.LocalUserId;
        var otherAuth = Other.GetComponent<RWAthenticator>();
        Other.localPlayerId = otherAuth.LocalUserId;
        Server.MatchData.AddPlayerInfo(masterAuth.LocalUserId, masterAuth.LocalNickName, 0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));
        Server.MatchData.AddPlayerInfo(otherAuth.LocalUserId, otherAuth.LocalNickName, 0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));

        Master.authenticator = null;
        Other.authenticator = null;
        
        Server.ListenAsync().Forget();
        while (Server.Active == false)
        {
            await UniTask.Yield();
        }

        await Master.ConnectAsync("localhost");
        await Other.ConnectAsync("localhost");
    }
}
