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
            tower.ownerTag = playerState.tag;
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

            tower.hp = hp;
            tower.maxHp = hp;
                
            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        protected override void OnWave(int wave)
        {
            return;
            for (var index = 0; index < PlayerStates.Length; ++index)
            {
                var playerState = PlayerStates[index];
                // SpawnMinions(playerState, team : index);
            }
        }

       

        void SpawnMinions(PlayerState playerState, byte team, byte ownerTag)
        {
            var diceInfos = TableManager.Get().DiceInfo;
            for (byte fieldIndex = 0; fieldIndex < playerState.Field.Count; ++fieldIndex)
            {
                var fieldSlot = playerState.Field[fieldIndex];
                if (fieldSlot.IsEmpty)
                {
                    continue;
                }

                var diceId = fieldSlot.diceId;
                if (diceInfos.GetData(diceId, out var diceInfo) == false)
                {
                    ED.Debug.LogError($"다이스정보 {fieldSlot.diceId}가 없습니다. UserId : {playerState.userId} 필드 슬롯 : {fieldIndex}");
                    continue;
                }
                
                var spawnCount = diceInfo.spawnMultiply;
                if (diceInfo.castType == (int)DICE_CAST_TYPE.MINION)
                {
                    spawnCount *= fieldSlot.diceScale;
                }

                for (int i = 0; i < spawnCount; ++i)
                {
                    var actor = Object.Instantiate(ActorProxyPrefab);
                    actor.ownerTag = playerState.tag;
                    actor.team = team;
                    actor.ownerTag = ownerTag;
                    actor.spawnSlot = fieldIndex;

                    var deckDice = playerState.GetDeckDice(diceId);
                    
                    ServerObjectManager.Spawn(actor.NetIdentity);
                }
            }
        }
        
    }
}