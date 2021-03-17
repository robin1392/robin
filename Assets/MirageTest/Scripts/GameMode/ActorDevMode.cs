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
        
        public ActorDevMode(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder, serverObjectManager)
        {
        }

        protected override async UniTask OnBeforeGameStart()
        {
            var gameState = CreateGameState();
            var playerStates = CreatePlayerStates();
            gameState.masterOwnerTag = playerStates[0].ownerTag;
            GameState = gameState;
            PlayerStates = playerStates;
            
            PlayerState1.team = GameConstants.BottomCamp;
            PlayerState2.team = GameConstants.TopCamp;
            
            ServerObjectManager.Spawn(gameState.NetIdentity);
            foreach (var playerState in playerStates)
            {
                ServerObjectManager.Spawn(playerState.NetIdentity);
            }

            await UniTask.Yield();
            
            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            CreateAndSpawnTower(PlayerState1, player1TowerPosition);
            
            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            CreateAndSpawnTower(PlayerState2, player2TowerPosition);
        }

        void CreateAndSpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = UnityEngine.Object.Instantiate(_prefabHolder.ActorProxy, position, Quaternion.identity);
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

        public void SpawnMyMinion(int diceId, byte ingameLevel, byte outGameLevel, byte diceScale)
        {
            SpawnAtCenterField(PlayerState1, diceId, ingameLevel, outGameLevel, diceScale);
        }
        
        public void SpawnEnemyMinion(int diceId, byte ingameLevel, byte outGameLevel, byte diceScale)
        {
            SpawnAtCenterField(PlayerState2, diceId, ingameLevel, outGameLevel, diceScale);
        }

        void SpawnAtCenterField(PlayerState playerState, int diceId, byte ingameLevel, byte outGameLevel, byte diceScale)
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

            var actorProxies = CreateActorByPlayerFieldDice(playerState);
            foreach (var actorProxy in actorProxies)
            {
                ServerObjectManager.Spawn(actorProxy.NetIdentity);
            }
        }
    }
}
