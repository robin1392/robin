using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MirageTest.Scripts;
using UnityEngine;

public class ActorDevModeStarter : MonoBehaviour
{
    public RWNetworkServer Server;
    public RWNetworkClient Master;
    public bool EnableMasterAI;
    public string MasterName;
    public RWNetworkClient Other;
    public bool EnableOtherAI;
    public string OtherName;
    public string Address = "localhost";

    async void Start()
    {
        if (TableManager.Get().Loaded == false)
        {
            TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
        }
        
        if (Server != null && Server.gameObject.activeInHierarchy)
        {
            Server.authenticator = null;
            Server.MatchData.AddPlayerInfo(Master?.LocalUserId ?? MasterName, Master?.LocalNickName ?? MasterName, 0,0, new DeckInfo(5001, 1007, 1008, 1009, 1010, 1011), true, EnableMasterAI);
            Server.MatchData.AddPlayerInfo(Other?.LocalUserId ?? OtherName, Other?.LocalNickName ?? OtherName, 0,0, new DeckInfo(5001, 1007, 1008, 1009, 1010, 1011), true, EnableOtherAI);
            Server.ListenAsync().Forget();
            while (Server.Active == false)
            {
                await UniTask.Yield();
            }
        }

        if (Master != null && Master.gameObject.activeInHierarchy)
        {
            Master.authenticator = null;
            await Master.RWConnectAsync(Address);    
        }

        if (Other != null && Other.gameObject.activeInHierarchy)
        {
            Other.authenticator = null;    
            await Other.RWConnectAsync(Address);
        }
    }
}
