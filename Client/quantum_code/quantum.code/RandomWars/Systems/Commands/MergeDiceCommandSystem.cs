using Photon.Deterministic;
using Quantum.Commands;

namespace Quantum
{
    public unsafe class MergeDiceCommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                var command = f.GetPlayerCommand(playerID) as MergeDiceCommand;
                if (command != null)
                {
                    var player = f.Global->Players.GetPointer(playerID);
                    MergeFieldDice(f, command.SourceFieldIndex, command.TargetFieldIndex, player);
                }
            }
        }

        public static void MergeFieldDice(Frame f, int sourceFieldIndex, int targetFieldIndex, RWPlayer* player)
        {
            var field = f.Unsafe.GetPointer<Field>(player->EntityRef);

            var sourceFieldDice = field->Dices.GetPointer(sourceFieldIndex);
            if (sourceFieldDice->IsEmpty)
            {
                Log.Error($"필드에 주사위가 존재하지 않습니다.: playerIndex:{player->PlayerRef}, fieldSourceIndex:{sourceFieldIndex}");
                return;
            }

            var targetFieldDice = field->Dices.GetPointer(targetFieldIndex);
            if (targetFieldDice->IsEmpty)
            {
                Log.Error($"필드에 주사위가 존재하지 않습니다.: playerIndex:{player->PlayerRef}, targetFieldIndex:{targetFieldIndex}");
                return;
            }
            
            if (targetFieldDice->DiceScale >= GameConstants.MaxIngameDiceScale)
            {
                Log.Error($"주사위 눈금이 최대치입니다.: playerIndex:{player->PlayerRef}, fieldIndex:{targetFieldIndex}");
                return;
            }
            
            if (sourceFieldDice->DeckIndex != targetFieldDice->DeckIndex)
            {
                Log.Error(
                    $"병합하려는 주사위의 덱인덱스가 다릅니다.: playerIndex:{player->PlayerRef}, source:{sourceFieldDice->DeckIndex} target:{targetFieldDice->DeckIndex}");
                return;
            }

            if (sourceFieldDice->DiceScale != targetFieldDice->DiceScale)
            {
                Log.Error(
                    $"병합하려는 주사위의 눈금이 다릅니다.: playerIndex:{player->PlayerRef}, source:{sourceFieldDice->DiceScale} target:{targetFieldDice->DiceScale}");
                return;
            }

            var deck = f.Get<Deck>(player->EntityRef);

            var deckIndex = f.RNG->Next(0, deck.Dices.Length);
            
            targetFieldDice->DeckIndex = deckIndex;
            targetFieldDice->DiceScale = sourceFieldDice->DiceScale + 1;

            sourceFieldDice->SetEmpty();
            
            f.Events.FieldDiceMerged(player->PlayerRef, sourceFieldIndex, targetFieldIndex);
        }
    }
}