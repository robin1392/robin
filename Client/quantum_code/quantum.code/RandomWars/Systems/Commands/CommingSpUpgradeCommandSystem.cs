using Quantum.Commands;

namespace Quantum
{
    public unsafe class CommingSpUpgradeCommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerID = 0; playerID < f.Global->Players.Length; playerID++)
            {
                var command = f.GetPlayerCommand(playerID) as CommingSpUpgradeCommand;
                if (command != null)
                {
                    var player = f.Global->Players.GetPointer(playerID);
                    CommingSpUpgrade(f, player);
                }
            }
        }

        public static void CommingSpUpgrade(Frame f, RWPlayer* player)
        {
            var sp = f.Unsafe.GetPointer<Sp>(player->EntityRef);
            var spUpgradeCost = f.SpUpgradeCost(sp->CommingSpGrade);
            
            if (sp->CurrentSp < spUpgradeCost)
            {
                Log.Error($"SP 업그레이드를 위한 SP가 모자랍니다.: player:{player->PlayerRef}");
                return;
            }
            
            if (sp->CommingSpGrade >= GameConstants.MaxSpUpgradeLevel)
            {
                Log.Error($"SP 업그레이드 레벨이 최대치입니다.: player:{player->PlayerRef}");
                return;
            }

            sp->CurrentSp -= spUpgradeCost;
            sp->CommingSpGrade += 1;

            sp->CommingSp = f.CalculateCommingSp(sp->CommingSpGrade);

            f.Events.CommingSpGradeUpgraded(player->PlayerRef);
            f.Events.SpDecreased(player->PlayerRef);
            f.Events.CommingSpChanged(player->PlayerRef);
            f.Events.CommingSpGradeChanged(player->PlayerRef);
        }
    }
}