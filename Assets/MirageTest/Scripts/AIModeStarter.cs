using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class AIModeStarter : MonoBehaviour
    {
        public RWNetworkServer Server;
        public RWNetworkClient Client;
        
        private void Start()
        {
            Server.Listening = false;
            Server.serverGameLogic.isAIMode = true;

            Server.MatchData.AddPlayerInfo("1", "a", 0, new DeckInfo(1001, 1002, 1003, 1004, 1005));
            Server.MatchData.AddPlayerInfo("2", "b", 0, new DeckInfo(1001, 1002, 1003, 1004, 1005));
            
            Server.StartHost(Client).Forget();
        }
    }
}