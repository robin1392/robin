using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;

namespace MirageTest.Scripts.GameMode
{
    public class CoopMode : GameModeBase
    {
        public CoopMode(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager) : base(gameState, playerStates, actorProxyPrefab, serverObjectManager)
        {
        }

        protected override void OnBeforeGameStart()
        {
            
        }

        protected override void OnWave(int wave)
        {
            
        }
    }
}
