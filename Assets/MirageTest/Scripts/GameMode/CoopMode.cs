using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;
using RandomWarsResource.Data;
using UnityEngine;

namespace MirageTest.Scripts.GameMode
{
    public class CoopMode : GameModeBase
    {
        public CoopMode(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager) : base(gameState, playerStates, actorProxyPrefab, serverObjectManager)
        {
        }

        protected override void OnBeforeGameStart()
        {
            PlayerState1.camp = GameConstants.BottomCamp;
            PlayerState2.camp = GameConstants.BottomCamp;
            
            SpawnAllyTower();
        }
        
        void SpawnAllyTower()
        {
            var playerTowerPosition = FieldManager.Get().GetPlayerPos(true);
            var tower = UnityEngine.Object.Instantiate(ActorProxyPrefab, playerTowerPosition, Quaternion.identity);
            tower.team = GameConstants.BottomCamp;
            tower.ownerTag = GameConstants.ServerTag;
            tower.actorType = ActorType.Tower;
            
            var tableManager = TableManager.Get();
            var hp = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.TowerHp].value;

            foreach (var playerState in PlayerStates)
            {
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
            }

            tower.hp = hp;
            tower.maxHp = hp;
                
            ServerObjectManager.Spawn(tower.NetIdentity);
        }
        
        void SpawnEnemyTower()
        {
            //타워는 스폰하지 않는다.
            
            var playerTowerPosition = FieldManager.Get().GetPlayerPos(true);
            var tower = UnityEngine.Object.Instantiate(ActorProxyPrefab, playerTowerPosition, Quaternion.identity);
            tower.team = GameConstants.TopCamp;
            tower.ownerTag = GameConstants.ServerTag;
            tower.actorType = ActorType.Tower;
            
            var tableManager = TableManager.Get();
            var hp = 0;

            foreach (var playerState in PlayerStates)
            {
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
            }
            
            tower.hp = hp;
            tower.maxHp = hp;
            
            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        protected override void OnWave(int wave)
        {
            //웨이브때마다 보스알을 몬스터를 스폰한다.
            //보스알 AI는 4웨이브 후에 변신한다.
            //게임모드에서 스폰한 보스알을 가지고 있다가 변신 시켜야할 듯하다.
        }
    }
}
