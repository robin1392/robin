using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class CanAct : BTDecorator
    {
        public AIBlackboardValueKey CanActKey;
        
        public override void OnEnterRunning(BTParams p)
        {
            base.OnEnterRunning(p);
            p.Blackboard->RegisterReactiveDecorator(p.Frame, CanActKey.Key, this);
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            p.Blackboard->UnregisterReactiveDecorator(p.Frame, CanActKey.Key, this);
        }
        
        public override Boolean DryRun(BTParams p)
        {
            return p.Blackboard->GetBoolean(p.Frame, CanActKey.Key);
        }
    }
}