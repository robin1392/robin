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
        
        public static void CreateFieldDice(Frame f, CreateFieldDiceCommand command, int playerID)
        {
            
            // f.AddAsset()
            // f.FindAsset<VsMode>()
            // CellData availableCellData;
            // bool hasAvailableCell = GridHelper.GetAvailableBenchCell(f, playerID, out availableCellData);
            // if (!hasAvailableCell)
            // {
            //     return;
            // }
            //
            // var player = f.Global->Players.GetPointer(playerID);
            // var targetSpecRef = player->StoreSlots[command.SlotIndex].Spec;
            // var unitSpec = f.FindAsset<UnitSpec>(targetSpecRef.Id);
            //
            // if (unitSpec != null && player->BuyUnit(unitSpec.BuyPrice))
            // {
            //     UnitSpec.CreateUnit(f, player, command.SlotIndex, availableCellData, unitSpec, targetSpecRef);
            // }
            // else {
            //     f.Events.PlayerHasNoMoneyAlert(playerID);
            // }
        }
    }
}