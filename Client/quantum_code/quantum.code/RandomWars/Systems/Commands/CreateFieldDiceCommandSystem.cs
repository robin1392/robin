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
                    var player = f.Global->Players.GetPointer(playerID);
                    CreateFieldDice(f, command, player);
                }
            }
        }
        
        public static void CreateFieldDice(Frame f, CreateFieldDiceCommand command, RWPlayer* player)
        { 
            var deck = f.Get<Deck>(player->EntityRef);

            CreateFieldDice(f, command.FieldIndex, command.DeckIndex, player);
        }

        public static void CreateFieldDice(Frame f, int selectedFieldIndex, int deckIndex, RWPlayer* player)
        {
            var field = f.Unsafe.GetPointer<Field>(player->EntityRef);
            field->Dices.GetPointer(selectedFieldIndex)->DiceScale = 0; //눈금 한개가 0, 두개 1...
            field->Dices.GetPointer(selectedFieldIndex)->DeckIndex = deckIndex;

            f.Events.FieldDiceCreated(player->PlayerRef, selectedFieldIndex);
        }
    }
}