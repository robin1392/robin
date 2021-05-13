using System.Collections.Generic;
using System.Linq;
using Quantum.Actors;
using Quantum.Combat;
using Quantum.Core;

namespace Quantum
{
    public static class SystemSetup
    {
        public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            var coreSystems = new SystemBase[]
            {
                // pre-defined core systems
                new CullingSystem2D(),
                // new CullingSystem3D(),
                
                new PhysicsSystem2D(),
                // new PhysicsSystem3D(),

                new NavigationSystem(),
                new EntityPrototypeSystem(),
            };
            
            var devSystem = new List<SystemBase>
            {
                new CreateActorCommandSystem(),
                new BotSDKDebuggerSystem(),
            };

            var commandSystems = new SystemMainThread[]
            {
                new CreateFieldDiceCommandSystem(),
                new CreateRandomFieldDiceCommandSystem(),
                new MergeDiceCommandSystem(),
                new PowerDeckDiceUpCommandSystem(),
                new CommingSpUpgradeCommandSystem(),
            };
            
            var gameStateSystems = new List<SystemMainThread>
            {
                new ReadySystem(),
                new CountDownSystem(),
                new UpdateWaveSystem(),
            };

            var creation = new List<SystemMainThread>
            {
                new ActorCreationSystem(),
                new ProjectileCreationSystem(),
            };
            
            var logicSystem = new List<SystemMainThread>
            {
                new PlayerBotSystem(),
                new ActorSystem(),
                new ProjectileSystem(),
                new UpdateFronzenSystem(),
                new UpdateBuffStateSystem(),
                new MineSystem(),
                new DamagePerSecSystem(),
                new DestroyActorByHpSystem(),
                new DestroyByComponent()
            };

            var play = new GamePlaySystemGroup("Play", commandSystems.Concat(gameStateSystems).Concat(creation).Concat(logicSystem).ToArray());
            var playGroup = new List<SystemBase>
            {
                play    
            };
            
            var logicSignal = new List<SystemBase>
            {
                new BotSDKSystem(),
                new PlayerInitSystem(),
                new StoneBallTriggerSystem(),
            };
            
            var gameStateSignal = new List<SystemBase>()
            {
                new SuddenDeathSystem(),
                new SetWaveTimeSystem(),
                new AddSpBySpWaveSystem(),
                new UpdateCommingSpByWaveSystem(),
            };
            
            if (gameConfig.Mode == 1)
            {
                gameStateSignal.Add(new CoopModeSpawnSystem());
            }
            else
            {
                gameStateSignal.Add(new BattleModeSystem());
            }

            return coreSystems
                .Concat(devSystem)
                .Concat(playGroup)
                .Concat(logicSignal)
                .Concat(gameStateSignal)
                .ToArray();
        }
    }
}