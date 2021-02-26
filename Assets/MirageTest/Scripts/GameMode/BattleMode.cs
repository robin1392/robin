using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using UnityEngine;

namespace MirageTest.Scripts.GameMode
{
    public class BattleMode : GameModeBase
    {
        public BattleMode(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager) : base(gameState, playerStates, actorProxyPrefab, serverObjectManager)
        {
        }

        protected override void OnBeforeGameStart()
        {
            for (var index = 0; index < PlayerStates.Length; ++index)
            {
                var playerState = PlayerStates[index];
                // playerState.SpawnTower(ActorProxyPrefab, index);
            }
        }

        protected override void OnWave(int wave)
        {
            return;
            for (var index = 0; index < PlayerStates.Length; ++index)
            {
                var playerState = PlayerStates[index];
                SpawnMinions(playerState, team : index);
            }
        }

        void SpawnMinions(PlayerState playerState, int team)
        {
            var diceInfos = TableManager.Get().DiceInfo;
            for (var fieldIndex = 0; fieldIndex < playerState.Field.Count; ++fieldIndex)
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
                    actor.owner = playerState.userId;
                    actor.team = team;
                    
                    var deckDice = playerState.GetDeckDice(diceId);
                    
                    ServerObjectManager.Spawn(actor.NetIdentity);
                }
            }
        }
        
    }
}