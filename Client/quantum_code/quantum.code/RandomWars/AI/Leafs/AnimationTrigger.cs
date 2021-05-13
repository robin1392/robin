using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class AnimationTrigger : BTLeaf
    {
        public string Trigger;
        
        protected override BTStatus OnUpdate(BTParams p)
        {
            var e = p.Entity;
            var f = p.Frame;

            f.Events.AnimationTrigger(e, Trigger);
            return BTStatus.Success;
        }
    }
}