using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class HasTarget : BTLeaf
    {
        public AIBlackboardValueKey Target;

        protected override BTStatus OnUpdate(BTParams p)
        {
            var bb = p.Frame.Get<AIBlackboardComponent>(p.Entity);
            var target = bb.GetEntityRef(p.Frame, Target.Key);
            
            if (target != EntityRef.None)
            {
                return BTStatus.Success;    
            }

            return BTStatus.Failure;
        }
    }
}