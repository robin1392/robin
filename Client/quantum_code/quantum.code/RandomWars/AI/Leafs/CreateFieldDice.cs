using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class CreateFieldDice : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var e = p.Entity;
            var f = p.Frame;

            var rwPlayer = f.GetRWPlayer(e);
            var playerRef = rwPlayer->PlayerRef;
            if (f.IsFieldFull(playerRef))
            {
                return BTStatus.Failure;
            }
            
            if(f.HasEnouphSpToCreateFieldDice(playerRef))
            {
                CreateRandomFieldDiceCommandSystem.CreateRandomFieldDice(f, rwPlayer);
                return BTStatus.Success;
            }

            return BTStatus.Failure;
        }
    }
}