using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using RandomWarsResource.Data;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts.GameMode
{
    public class BattleMode : GameModeBase
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BattleMode));
        
        public BattleMode(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager) : base(gameState, playerStates, actorProxyPrefab, serverObjectManager)
        {
        }

        protected override void OnBeforeGameStart()
        {
            PlayerState1.camp = GameConstants.BottomCamp;
            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            SpawnTower(PlayerState1, player1TowerPosition);
            
            PlayerState2.camp = GameConstants.TopCamp;
            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            SpawnTower(PlayerState2, player2TowerPosition);
        }
        
        void SpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = UnityEngine.Object.Instantiate(ActorProxyPrefab, position, Quaternion.identity);
            tower.team = playerState.camp;
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

            tower.health = hp;
            tower.maxHealth = hp;
                
            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        protected override void OnWave(int wave)
        {
            Spawn();
        }

        public override void Spawn()
        {
            for (var index = 0; index < PlayerStates.Length; ++index)
            {
                var playerState = PlayerStates[index];
                SpawnMinions(playerState, playerState.camp, playerState.ownerTag);
            }
        }

        void SpawnMinions(PlayerState playerState, byte team, byte ownerTag)
        {
            var diceInfos = TableManager.Get().DiceInfo;
            for (byte fieldIndex = 0; fieldIndex < playerState.Field.Count; ++fieldIndex)
            {
                var fieldDice = playerState.Field[fieldIndex];
                if (fieldDice.IsEmpty)
                {
                    continue;
                }

                var diceId = fieldDice.diceId;
                if (diceInfos.GetData(diceId, out var diceInfo) == false)
                {
                    ED.Debug.LogError($"다이스정보 {fieldDice.diceId}가 없습니다. UserId : {playerState.userId} 필드 슬롯 : {fieldIndex}");
                    continue;
                }
                
                var spawnCount = diceInfo.spawnMultiply;
                if (diceInfo.castType == (int)DICE_CAST_TYPE.MINION)
                {
                    spawnCount *= (fieldDice.diceScale + 1);
                }
                
                var deckDice = playerState.GetDeckDice(diceId);

                var power = diceInfo.power 
                            + (diceInfo.powerUpgrade * deckDice.outGameLevel) 
                            + (diceInfo.powerInGameUp * deckDice.inGameLevel);

                //KZSee: 서든데스 로직으로 묶어내기
                if (GameState.wave > 10)
                {
                    power *= Mathf.Pow(2f, GameState.wave - 10);
                }
                
                var maxHealth =  diceInfo.maxHealth + (diceInfo.maxHpUpgrade * deckDice.outGameLevel) + (diceInfo.maxHpInGameUp * deckDice.inGameLevel);
                var effect = diceInfo.effect + (diceInfo.effectUpgrade * deckDice.outGameLevel) + (diceInfo.effectInGameUp * deckDice.inGameLevel);
                var attackSpeed = diceInfo.attackSpeed;
                if (GameState.wave > 10)
                {
                    attackSpeed *= Mathf.Pow(0.9f, GameState.wave - 10);
                    attackSpeed = Mathf.Max(diceInfo.attackSpeed * 0.5f, attackSpeed);
                }
                
                if ((DICE_CAST_TYPE)diceInfo.castType == DICE_CAST_TYPE.HERO)
                {
                    power *= fieldDice.diceScale + 1;
                    maxHealth *= fieldDice.diceScale + 1;
                    effect *= fieldDice.diceScale + 1;
                }
                
                for (int i = 0; i < spawnCount; ++i)
                {
                    var actor = Object.Instantiate(ActorProxyPrefab);
                    actor.ownerTag = playerState.ownerTag;
                    actor.actorType = ActorType.MinionFromDice;
                    actor.team = team;
                    actor.ownerTag = ownerTag;
                    actor.spawnSlot = fieldIndex;
                    actor.power = power;
                    actor.maxHealth = maxHealth;
                    actor.health = maxHealth;
                    actor.effect = effect;
                    actor.attackSpeed = attackSpeed;
                    actor.diceScale = fieldDice.diceScale;
                    actor.ingameUpgradeLevel = deckDice.inGameLevel;
                    actor.dataId = diceId;
                    
                    ServerObjectManager.Spawn(actor.NetIdentity);
                }
            }
        }
    }
}