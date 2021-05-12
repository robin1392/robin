using Photon.Deterministic;
using Quantum.Commands;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class PowerDeckDiceUpCommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                var command = f.GetPlayerCommand(playerID) as PowerDeckDiceUpCommand;
                if (command != null)
                {
                    var player = f.Global->Players.GetPointer(playerID);
                    PowerDeckDiceUp(f, command.DeckIndex, player);
                }
            }
        }

        public static void PowerDeckDiceUp(Frame f, int deckIndex, RWPlayer* player)
        {
            var deck = f.Unsafe.GetPointer<Deck>(player->EntityRef);
            var deckDice = deck->Dices.GetPointer(deckIndex);

            var sp = f.Unsafe.GetPointer<Sp>(player->EntityRef);
            var powerUpCost = f.GetPowerUpCost(deckDice->InGameLevel);
            if (sp->CurrentSp < powerUpCost)
            {
                Log.Error($"덱 주사위 업그레이드를 위한 SP가 모자랍니다.: player:{player->PlayerRef}, deckIndex:{deckIndex}");
                return;
            }
            
            if (deckDice->InGameLevel >= GameConstants.MaxIngameUpgradeLevel)
            {
                Log.Error($"덱 주사위 레벨이 최대치입니다.: player:{player->PlayerRef}, deckIndex:{deckIndex}");
                return;
            }

            sp->CurrentSp -= powerUpCost;
            deckDice->InGameLevel += 1;

            f.Events.PoweredDeckDiceUp(player->PlayerRef, deckIndex);
            f.Events.SpDecreased(player->PlayerRef);
        }
    }
}