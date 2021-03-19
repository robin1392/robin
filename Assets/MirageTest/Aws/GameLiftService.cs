#if UNITY_EDITOR || UNITY_STANDALONE
using System;
using System.Collections.Generic;
using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;
using Mirage.KCP;
using MirageTest.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MirageTest.Aws
{
    public class GameLiftService : MonoBehaviour
    {
        RWNetworkServer _server;

        private void Start()
        {
            _server = FindObjectOfType<RWNetworkServer>();
            var fps = CommandLineArgs.GetInt("fps"); 
            if (fps.HasValue)
            {
                Application.targetFrameRate = fps.Value; 
                Debug.Log($"fps from CommandLineArgs - {fps.Value}");
            }
            else
            {
                Application.targetFrameRate = 30;
            }
            
            QualitySettings.vSyncCount = 0;

            var transport = _server.GetComponent<KcpTransport>();
            var portFromArgs = CommandLineArgs.GetInt("port"); 
            if (portFromArgs.HasValue)
            {
                transport.Port = (ushort)portFromArgs.Value;
                Debug.Log($"port from CommandLineArgs - {portFromArgs.Value}");
            }

            if (Init(transport.Port) == false)
            {
                Debug.LogError("GameLiftService Init 실패.");    
            }
        }
        
        public bool Init(int port)
        {
            var initSDKOutcome = GameLiftServerAPI.InitSDK();
            if (initSDKOutcome.Success == false)
            {
                return false;
            }

            // Set parameters and call ProcessReady
            var processParams = new ProcessParameters(
               OnGameSession,
               OnUpdateGameSession,
               OnProcessTerminate,
               OnHealthCheck,
               port,
               new LogParameters(new List<string>()
               {
                  $"/local/game/game_{port - 7800}/log_{port}.txt",
               })
            );

            // Amazon GameLift 서비스에 서버 프로세스가 게임 세션을 호스팅할 준비가 되었음을 알립니다.
            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParams);
            if (processReadyOutcome.Success == false)
            {
                return false;
            }

            return true;
        }
        
        public void Destroy()
        {
            GameLiftServerAPI.Destroy();
        }
        
        public void TerminateGameSession()
        {
            GameLiftServerAPI.ProcessEnding();
        }
        
        void OnGameSession(GameSession gameSession)
        {
            TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
            
            var outcome = GameLiftServerAPI.ActivateGameSession();

            if (string.IsNullOrEmpty(gameSession.GameSessionData) == false)
            {
                var listPlayerAttribute = JsonConvert.DeserializeObject<List<MatchPlayerAttribute>>(gameSession.GameSessionData);
                foreach (var attribute in listPlayerAttribute)
                {
                    // 캐릭터 추가
                    string playerId = attribute.PlayerId;
                    string userName = attribute.dictAttributeString["userName"];
                    int trophy = (int)attribute.dictAttributeNumber["trophy"];
                    var listDiceInfo = attribute.dictAttributeList["diceInfo"];
                    AddMatchPlayer(playerId, userName, trophy, listDiceInfo);

                    // 모드 설정
                    string gameMode = attribute.dictAttributeList["gameMode"][0];
                    _server.MatchData.SetGameMode(gameMode);
                }
            }
            else if (string.IsNullOrEmpty(gameSession.MatchmakerData) == false)
            {
                JObject jObject = JObject.Parse(gameSession.MatchmakerData);
                foreach (var team in (JArray)jObject["teams"])
                {
                    foreach (var player in (JArray)team["players"])
                    {
                        // 캐릭터 추가
                        string playerId = player["playerId"].ToString();
                        string userName = player["attributes"]["userName"]["valueAttribute"].ToString();
                        int trophy = int.Parse(player["attributes"]["trophy"]["valueAttribute"].ToString());
                        var listDiceInfo = player["attributes"]["diceInfo"]["valueAttribute"].ToObject<List<string>>();
                        AddMatchPlayer(playerId, userName, trophy, listDiceInfo);

                        // 모드 설정
                        string gameMode = player["attributes"]["gameMode"]["valueAttribute"].ToObject<List<string>>()[0];
                        _server.MatchData.SetGameMode(gameMode);
                    }
                }
            }
        }
        
        void AddMatchPlayer(string playerId, string userName, int trophy, List<string> listDiceInfo)
        {
            if (_server == null)
            {
                return;
            }

            // 주사위
            DeckInfo deckInfo = new DeckInfo();
            deckInfo.DiceInfos = new DiceInfo[listDiceInfo.Count - 1];
            for (int i = 0; i < deckInfo.DiceInfos.Length; i++)
            {
                var matchDiceInfo = JsonConvert.DeserializeObject<MatchDiceInfo>(listDiceInfo[i]);
                deckInfo.DiceInfos[i] = new DiceInfo
                {
                    DiceId = matchDiceInfo.DiceId,
                    OutGameLevel = matchDiceInfo.Level,
                };
            }

            // 수호자
            var matchGuadianInfo = JsonConvert.DeserializeObject<MatchDiceInfo>(listDiceInfo[listDiceInfo.Count - 1]);
            deckInfo.GuadianId = matchGuadianInfo.DiceId;

            // 플레이어 추가
            _server.MatchData.AddPlayerInfo(playerId, userName, trophy, deckInfo);
        }
        
        void OnProcessTerminate()
        {
            // game-specific tasks required to gracefully shut down a game session, 
            // such as notifying players, preserving game state data, and other cleanup
            var ProcessEndingOutcome = GameLiftServerAPI.ProcessEnding();
            GameLiftServerAPI.Destroy();
        }

        bool OnHealthCheck()
        {
            bool isHealthy = true;
            // complete health evaluation within 60 seconds and set health
            return isHealthy;
        }


        void OnUpdateGameSession(UpdateGameSession updateGameSession)
        {
            
        }
        
        public bool AcceptPlayerSession(string playerSessionId)
        {
            try
            {
                var outCome = GameLiftServerAPI.AcceptPlayerSession(playerSessionId);
                if (outCome.Success == false)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public bool RemovePlayerSession(string playerSessionId)
        {
            try
            {
                var outCome = GameLiftServerAPI.RemovePlayerSession(playerSessionId);
                if (outCome.Success == false)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
#endif
