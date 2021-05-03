﻿using System.Collections.Generic;
using System.Linq;
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
                new CullingSystem3D(),

                new PhysicsSystem2D(),
                new PhysicsSystem3D(),

                new NavigationSystem(),
                new EntityPrototypeSystem(),
            };

            var commandSystems = new SystemBase[]
            {
                new CreateFieldDiceCommandSystem(),
                new CreateRandomFieldDiceCommandSystem(),
                new MergeDiceCommandSystem(),
                new PowerDeckDiceUpCommandSystem(),
                new CommingSpUpgradeCommandSystem(),
            };

            var gameStateSystems = new List<SystemBase>
            {
                new ReadySystem(),
                new CountDownSystem(),
                new UpdateWaveSystem(),
            };

            if (gameConfig.Mode == 1)
            {
                gameStateSystems.Add(new CoopModeSpawnSystem());
            }
            else
            {
                gameStateSystems.Add(new BattleModeSpawnSystem());
            }

            var logicSystem = new List<SystemBase>
            {
                new BTUpdateSystem(),
                new BotSDKSystem(),
                new ActorSystem(),
                new PlayerInitSystem()
            };

            return coreSystems
                .Concat(commandSystems)
                .Concat(gameStateSystems)
                .Concat(logicSystem)
                .ToArray();
        }
    }
}