using Quantum.Core;

namespace Quantum
{
    public static class SystemSetup
    {
        public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            return new SystemBase[]
            {
                // pre-defined core systems
                new CullingSystem2D(),
                new CullingSystem3D(),

                new PhysicsSystem2D(),
                new PhysicsSystem3D(),

                new NavigationSystem(),
                new EntityPrototypeSystem(),

                // user systems go here
                new CreateFieldDiceCommandSystem(),
                new CreateRandomFieldDiceCommandSystem(),
                new MergeDiceCommandSystem(),
                new PowerDeckDiceUpCommandSystem(),
                new CommingSpUpgradeCommandSystem(),

                new BTUpdateSystem(),
                new BotSDKSystem(),
                new ActorSystem(),
                new PlayerInitSystem()
            };
        }
    }
}