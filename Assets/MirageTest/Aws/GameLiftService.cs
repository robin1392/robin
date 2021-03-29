#if UNITY_EDITOR || UNITY_STANDALONE_LINUX
using System;
using System.Collections.Generic;
using System.Linq;
using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;
using Cysharp.Threading.Tasks;
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
        string matchmakingData = "{\"matchId\":\"9cf2f612-403e-4aa1-aa7a-8c5afd7e4e31\",\"matchmakingConfigurationArn\":\"arn:aws:gamelift:ap-northeast-2:153269277707:matchmakingconfiguration/randomwars-config-dev\",\"teams\":[{\"name\":\"red\",\"players\":[{\"playerId\":\"8ab64c62-8843-4c6f-b940-8f6eb736b423\",\"attributes\":{\"addTowerHp\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":0.0},\"class\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":1.0},\"diceInfo\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"{\\\"DiceId\\\":1000,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1001,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1002,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1003,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1004,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":5001,\\\"Level\\\":1,\\\"Count\\\":0}\"]},\"gameMode\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"coop\"]},\"questData\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"{\\\"QuestId\\\":14,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":6,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":3,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":9,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":17,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":1000,\\\"Value\\\":0,\\\"Status\\\":1}\"]},\"score\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":5.0},\"trophy\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":0.0},\"userName\":{\"attributeType\":\"STRING\",\"valueAttribute\":\"GUEST79515\"},\"winStreak\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":0.0}}}]},{\"name\":\"blue\",\"players\":[{\"playerId\":\"a2400798-6df1-4261-beb1-3f2a4e1cc60f\",\"attributes\":{\"addTowerHp\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":0.0},\"class\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":3.0},\"diceInfo\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"{\\\"DiceId\\\":1000,\\\"Level\\\":2,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1001,\\\"Level\\\":2,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":1002,\\\"Level\\\":3,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":2004,\\\"Level\\\":1,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":3005,\\\"Level\\\":3,\\\"Count\\\":0}\",\"{\\\"DiceId\\\":5001,\\\"Level\\\":1,\\\"Count\\\":0}\"]},\"gameMode\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"deathmatch\"]},\"questData\":{\"attributeType\":\"STRING_LIST\",\"valueAttribute\":[\"{\\\"QuestId\\\":11,\\\"Value\\\":3,\\\"Status\\\":2}\",\"{\\\"QuestId\\\":2,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":6,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":16,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":8,\\\"Value\\\":0,\\\"Status\\\":1}\",\"{\\\"QuestId\\\":1000,\\\"Value\\\":1,\\\"Status\\\":1}\"]},\"score\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":173.0},\"trophy\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":155.0},\"userName\":{\"attributeType\":\"STRING\",\"valueAttribute\":\"GUEST78045\"},\"winStreak\":{\"attributeType\":\"DOUBLE\",\"valueAttribute\":0.0}}}]}],\"autoBackfillMode\":null,\"autoBackfillTicketId\":null}";
        public bool EnableTestData;
        
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

            //TODO: 서버 익셉션 시 클라이언트에서 서버 익셉션을 확인할 수 있도록 하자.
            // Application.logMessageReceived += (string logString, string stackTrace, LogType type) =>
            // {
            //     if (type == LogType.Exception)
            //     {
            //         foreach (var player in _server.Players)
            //         {
            //             if (player == null)
            //             {
            //                 continue;
            //             }
            //
            //             if (player.Identity == null)
            //             {
            //                 continue;
            //             }
            //             
            //             player.Send(new ServerExceptionMessage()
            //             {
            //                 message = logString,  
            //             });
            //         }
            //         
            //         ProcessEnd(5).Forget();
            //     }
            // };

            var hasArg = CommandLineArgs.HasArg("table_test");
            if (hasArg)
            {
                TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
            }
            
            var hasMatchDataArg = CommandLineArgs.HasArg("matchdata_test");
            if (hasMatchDataArg)
            {
                EnableTestData = true;
            }

            var logFilePath = CommandLineArgs.GetString("logFile");
            if (Init(transport.Port, logFilePath) == false)
            {
                Debug.LogError("GameLiftService Init 실패.");    
            }
            
            if (EnableTestData)
            {
                ApplyMatchmakerData(matchmakingData);
            }
            
            _server.ListenAsync().Forget();
        }

        async UniTask ProcessEnd(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5)); 
            GameLiftServerAPI.ProcessEnding();
            Application.Quit();
        }
        
        public bool Init(int port, string logFilePath)
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
                   logFilePath,
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
                    var winStreak = attribute.dictAttributeNumber["winStreak"];
                    AddMatchPlayer(playerId, userName, trophy, (short)winStreak, listDiceInfo);

                    // 모드 설정
                    string gameMode = attribute.dictAttributeList["gameMode"][0];
                    _server.SetGameMode(gameMode);
                    _server.serverGameLogic.isAIMode = true;
                    _server.serverGameLogic.attachPlayer2AI = true;
                }
            }
            else if (string.IsNullOrEmpty(gameSession.MatchmakerData) == false)
            {
                ApplyMatchmakerData(gameSession.MatchmakerData);
            }
        }

        void ApplyMatchmakerData(string matchmakerData)
        {
            JObject jObject = JObject.Parse(matchmakerData);
            foreach (var team in (JArray)jObject["teams"])
            {
                foreach (var player in (JArray)team["players"])
                {
                    // 캐릭터 추가
                    string playerId = player["playerId"].ToString();
                    string userName = player["attributes"]["userName"]["valueAttribute"].ToString();
                    var trophy = int.Parse(player["attributes"]["trophy"]["valueAttribute"].ToString());
                    var winStreak = int.Parse(player["attributes"]["winStreak"]["valueAttribute"].ToString());
                    var listDiceInfo = player["attributes"]["diceInfo"]["valueAttribute"].ToObject<List<string>>();
                    AddMatchPlayer(playerId, userName, trophy, winStreak, listDiceInfo);

                    // 모드 설정
                    string gameMode = player["attributes"]["gameMode"]["valueAttribute"].ToObject<List<string>>()[0];
                    _server.SetGameMode(gameMode);
                    _server.serverGameLogic.isAIMode = false;
                    _server.serverGameLogic.attachPlayer2AI = false;
                }
            }
        }
        
        void AddMatchPlayer(string playerId, string userName, int trophy, int winStreak, List<string> listDiceInfo)
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
            deckInfo.GuardianId = matchGuadianInfo.DiceId;

            // 플레이어 추가
            _server.MatchData.AddPlayerInfo(playerId, userName, trophy, winStreak, deckInfo);
            Debug.Log($"플레이어 추가:{playerId} name:{userName} gudrdian:{deckInfo.GuardianId} deck:{String.Join(", ", deckInfo.DiceInfos.ToArray())}");
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
