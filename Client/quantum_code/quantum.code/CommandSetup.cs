using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum
{
    public static class CommandSetup
    {
        public static DeterministicCommand[] CreateCommands(RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            var playerCommnds = new DeterministicCommand[]
            {
                // user commands go here
                new CreateRandomFieldDiceCommand(),
                new CreateFieldDiceCommand(),
                new PowerDeckDiceUpCommand(),
                new MergeDiceCommand(),
                new CommingSpUpgradeCommand(),
            };

            var devCommands = new List<DeterministicCommand>()
            {
                new CreateActorCommand(),
            };
            
            return playerCommnds.Concat(devCommands).ToArray();
        }
    }
}