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
    public bool AttatchOtherAI;
    public string Address = "localhost";
    
    async void Start()
    {
        if (TableManager.Get().Loaded == false)
        {
            string targetPath = System.IO.Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "Dev");
            TableManager.Get().LoadFromFile(targetPath);
        }
        
        Server.MatchData.AddPlayerInfo(Master.LocalUserId, Master.LocalNickName, 0,0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));
        Server.MatchData.AddPlayerInfo(Other.LocalUserId, Other.LocalNickName, 0,0, new DeckInfo(5001, 1001, 1002, 1003, 1004, 1005));

        Master.authenticator = null;
        Other.authenticator = null;
        Server.authenticator = null;

        Server.serverGameLogic.attachPlayer2AI = true;
        Server.ListenAsync().Forget();
        while (Server.Active == false)
        {
            await UniTask.Yield();
        }
        
        await Master.RWConnectAsync(Address);
        await Other.RWConnectAsync(Address);   
    }
}
