using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using RandomWarsResource.Data;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts.GameMode
{
    public class ActorDevMode : GameModeBase
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ActorDevMode));
        
        public ActorDevMode(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager) : base(gameState, playerStates, actorProxyPrefab, serverObjectManager)
        {
        }

        protected override void OnBeforeGameStart()
        {
            PlayerState1.team = GameConstants.BottomCamp;
            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            SpawnTower(PlayerState1, player1TowerPosition);
            
            PlayerState2.team = GameConstants.TopCamp;
            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            SpawnTower(PlayerState2, player2TowerPosition);
        }

        void SpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = UnityEngine.Object.Instantiate(ActorProxyPrefab, position, Quaternion.identity);
            tower.team = playerState.team;
            tower.ownerTag = playerState.ownerTag;
            tower.actorType = ActorType.Tower;
            
            var tableManager = TableManager.Get();
            var hp = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.TowerHp].value;
            foreach (var deckDice in playerState.Deck)
            {
                if (tableManager.DiceInfo.GetData(deckDice.diceId, out var diceInfo) == false)
                {
                    Debug.LogError($"존재하지않는 다이스 아이디입니다. {deckDice.diceId}");
                    continue;
                }

                if (tableManager.DiceUpgrade.GetData(
                    x => x.diceLv == deckDice.outGameLevel && x.diceGrade == diceInfo.grade, out TDataDiceUpgrade diceLevelUpInfo))
                {
                    hp += diceLevelUpInfo.getTowerHp;
                }
            }

            tower.currentHealth = hp;
            tower.maxHealth = hp;
                
            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        protected override void OnWave(int wave)
        {
        }

        public void SpawnMyMinion(int diceId, byte ingameLevel, short outGameLevel, byte diceScale)
        {
            SpawnAtCenterField(PlayerState1, diceId, ingameLevel, outGameLevel, diceScale);
        }
        
        public void SpawnEnemyMinion(int diceId, byte ingameLevel, short outGameLevel, byte diceScale)
        {
            SpawnAtCenterField(PlayerState2, diceId, ingameLevel, outGameLevel, diceScale);
        }

        void SpawnAtCenterField(PlayerState playerState, int diceId, byte ingameLevel, short outGameLevel, byte diceScale)
        {
            playerState.Deck[0] = new DeckDice()
            {
                diceId = diceId,
                inGameLevel = ingameLevel,
                outGameLevel = outGameLevel,
            };

            playerState.Field[7] = new FieldDice()
            {
                diceId = diceId,
                diceScale = diceScale,
            };

            var actorProxies = CreateMinionsByPlayerField(playerState);
            foreach (var actorProxy in actorProxies)
            {
                ServerObjectManager.Spawn(actorProxy.NetIdentity);
            }
        }
    }
}
