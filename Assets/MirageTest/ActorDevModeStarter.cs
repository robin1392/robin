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
    public string Address = "localhost";

    async void Start()
    {
        if (TableManager.Get().Loaded == false)
        {
            string targetPath = System.IO.Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "Dev");
            TableManager.Get().LoadFromFile(targetPath);
        }
        
        Server.MatchData.AddPlayerInfo(Master.LocalUserId, Master.LocalNickName, 0,0, new DeckInfo(5001, 1007, 1008, 1009, 1010, 1011), true, Master.enableAI);
        Server.MatchData.AddPlayerInfo(Other.LocalUserId, Other.LocalNickName, 0,0, new DeckInfo(5001, 1007, 1008, 1009, 1010, 1011), true, Other.enableAI);

        Master.authenticator = null;
        Other.authenticator = null;
        Server.authenticator = null;
        
        Server.ListenAsync().Forget();
        while (Server.Active == false)
        {
            await UniTask.Yield();
        }
        
        await Master.RWConnectAsync(Address);
        await Other.RWConnectAsync(Address);   
    }
}
