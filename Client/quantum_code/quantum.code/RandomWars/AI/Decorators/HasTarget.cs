using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class HasTarget : BTDecorator
    {
        public AIBlackboardValueKey TargetRef;

        public override Boolean DryRun(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var bb = p.Blackboard;

            var enemy = bb->GetEntityRef(f, TargetRef.Key);
            if (f.Exists(enemy) == false)
            {
                return false;
            }

            return true;
        }
    }
}