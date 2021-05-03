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
                    CreateFieldDice(f, command.FieldIndex, command.DeckIndex, player);
                }
            }
        }

        public static void CreateFieldDice(Frame f, int selectedFieldIndex, int deckIndex, RWPlayer* player)
        {
            var diceCost = f.CreateFieldDiceCost(player->PlayerRef);
            var sp = f.Unsafe.GetPointer<Sp>(player->EntityRef);
            if (diceCost > sp->CurrentSp)
            {
                Log.Error(
                    $"주사위 생성을 위한 SP가 모자랍니다.: playerIndex:{player->PlayerRef}");
                return;
            }
            
            if (f.IsFieldFull(player->PlayerRef))
            {
                Log.Error(
                    $"필드가 가득찼습니다.: playerIndex:{player->PlayerRef}");
                return;
            }

            var field = f.Unsafe.GetPointer<Field>(player->EntityRef);

            field->Dices.GetPointer(selectedFieldIndex)->DiceScale = 0; //눈금 한개가 0, 두개 1...
            field->Dices.GetPointer(selectedFieldIndex)->DeckIndex = deckIndex;

            sp->CurrentSp -= diceCost;

            var diceCreation = f.Unsafe.GetPointer<DiceCreation>(player->EntityRef);
            diceCreation->Count += 1;

            f.Events.FieldDiceCreated(player->PlayerRef, selectedFieldIndex);
            f.Events.SpDecreased(player->PlayerRef);
            f.Events.DiceCreationCountChanged(player->PlayerRef);
        }
    }
}