using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum {
  public static class CommandSetup {
    public static DeterministicCommand[] CreateCommands(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
      return new DeterministicCommand[] {

        // user commands go here
        new CreateRandomFieldDiceCommand(),
        new CreateFieldDiceCommand(),
        new PowerDeckDiceUpCommand(),
        new MergeDiceCommand(),
        new CommingSpUpgradeCommand(),
      };
    }
  }
}
