using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum
{
    public unsafe class CreateRandomFieldDiceCommandSystem : SystemMainThread
    {
        private const int EmptyFieldNotExist = -1;
        private const int NoFieldSelected = -2;
        
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                var command = f.GetPlayerCommand(playerID) as CreateRandomFieldDiceCommand;
                if (command != null)
                {
                    var player = f.Global->Players.GetPointer(playerID);
                    CreateRandomFieldDice(f, command, player);
                }
            }
        }
        
        public static void CreateRandomFieldDice(Frame f, CreateRandomFieldDiceCommand command, RWPlayer* player)
        {
            var selectedFieldIndex = GetEmptyFieldIndexByRandom(f, player);
            if (selectedFieldIndex == EmptyFieldNotExist)
            {
                Log.Error($"비어있는 필드가 없습니다.");
                return;
            }
            else if (selectedFieldIndex == NoFieldSelected)
            {
                Log.Error($"선택된 필드가 없습니다.");
                return;
            }
            
            var deck = f.Get<Deck>(player->EntityRef);
            var deckDiceSelected = f.RNG->Next(0, deck.Dices.Length);

            CreateFieldDiceCommandSystem.CreateFieldDice(f, selectedFieldIndex, deckDiceSelected, player);
        }
        
        static int GetEmptyFieldIndexByRandom(Frame f, RWPlayer* player)
        {
            var field = f.Unsafe.GetPointer<Field>(player->EntityRef);
            var emptyFieldCount = 0;
            for (var i = 0; i < field->Dices.Length; ++i)
            {
                if (field->Dices[i].IsEmpty)
                {
                    emptyFieldCount++;
                }
            }

            if (emptyFieldCount < 1)
            {
                return EmptyFieldNotExist;
            }

            var selectedIndexOnEmpty = f.RNG->Next(0, emptyFieldCount);
            var index = 0;
            for (var i = 0; i < field->Dices.Length; ++i)
            {
                if (field->Dices[i].IsEmpty)
                {
                    if (selectedIndexOnEmpty == index)
                    {
                        return i;
                    }
                    index++;
                }
            }
            
            return NoFieldSelected;
        }
    }
}