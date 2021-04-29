using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum
{
    public unsafe class CreateFieldDiceCommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                var command = f.GetPlayerCommand(playerID) as CreateFieldDiceCommand;

                if (command != null)
                {
                }
            }
        }
    }
}