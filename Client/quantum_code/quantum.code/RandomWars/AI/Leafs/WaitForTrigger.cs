using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class WaitForTrigger : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            if (p.Frame.Has<Trigger>(p.Entity))
            {
                return BTStatus.Success;
            }
            
            return BTStatus.Running;
        }
    }
}