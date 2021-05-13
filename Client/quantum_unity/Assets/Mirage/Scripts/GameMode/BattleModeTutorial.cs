using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using Mirage.Logging;
using MirageTest.Aws;
using MirageTest.Scripts.Entities;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Service.Core;
using Service.Template;
using UnityEngine;
using Debug = ED.Debug;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts.GameMode
{
    public class BattleModeTutorial : BattleMode
    {
        public BattleModeTutorial(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder,
            serverObjectManager)
        {
        }

        public override async UniTask OnBeforeGameStart()
        {
            var gameState = CreateGameState();
            Debug.Log($"gameState:{gameState != null} {gameState.state}");
            var playerStates = CreatePlayerStates();
            Debug.Log($"playerStates1:{playerStates[0] != null} playerStates2:{playerStates[1] != null}");
            gameState.masterOwnerTag = playerStates[0].ownerTag;
            GameState = gameState;
            PlayerStates = playerStates;

            PlayerState1.team = GameConstants_Mirage.BottomCamp;
            PlayerState2.team = GameConstants_Mirage.TopCamp;
            
            Debug.Log($"ServerObjectManager:{ServerObjectManager != null}");
            ServerObjectManager.Spawn(gameState.NetIdentity);
            foreach (var playerState in playerStates)
            {
                ServerObjectManager.Spawn(playerState.NetIdentity);
            }

            //액터보다 플레이어 스테이트가 먼저 생성되야해서 한 틱 양보
            await UniTask.Yield();

            Debug.Log($"fieldManager:{FieldManager.Get() != null}");
            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            SpawnTower(PlayerState1, player1TowerPosition);

            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            SpawnTower(PlayerState2, player2TowerPosition);
        }

        void SpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = UnityEngine.Object.Instantiate(_prefabHolder.TowerActorProxyPrefab, position, Quaternion.identity);
            tower.team = playerState.team;
            tower.ownerTag = playerState.ownerTag;

            var tableManager = TableManager.Get();
            var hp = 30000;
            if (playerState == PlayerState2)
            {
                hp = 10000;
            }

            tower.currentHealth = hp;
            tower.maxHealth = hp;

            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        
        public override void OnHitDamageTower(ActorProxy actorProxy)
        {
        }

        private void EndGame(PlayerState winner, PlayerState loser, bool winByDefault, bool perfect)
        {
            GameState.state = EGameState.End;
            var victoryReport = new MatchReport();
            victoryReport.GameResult = winByDefault ?GAME_RESULT.VICTORY_BY_DEFAULT : GAME_RESULT.VICTORY;
            victoryReport.Nick = winner.userId;
            victoryReport.IsPerfect = perfect;

            // 패자 레포트 작성
            var defeatReport = new MatchReport();
            defeatReport.GameResult = GAME_RESULT.DEFEAT;
            defeatReport.Nick = loser.userId;

            Server.OnGameEnd(new List<MatchReport>() {victoryReport, defeatReport});
        }

        public override void OnGiveUp(PlayerState playerState)
        {
            var winner = PlayerStates.First(p => p.ownerTag != playerState.ownerTag);
            int getDefenderTowerHp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
            var winnerTower = Server.Towers.Find(t => t.ownerTag == winner.ownerTag);
            var loser = playerState;
            EndGame(winner, loser, true, winnerTower.currentHealth > getDefenderTowerHp * 100);
        }
    }
}