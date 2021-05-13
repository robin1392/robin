using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class TauntSkill : BTDecorator
    {
        public override void OnEnterRunning(BTParams p)
        {
            base.OnEnterRunning(p);
            p.Frame.Add<Shield>(p.Entity);
        }

        public override void OnExit(BTParams p)
        {
            base.OnExit(p);
            p.Frame.Remove<Shield>(p.Entity);
        }
        
        public override Boolean DryRun(BTParams p)
        {
            return true;
        }
    }
}