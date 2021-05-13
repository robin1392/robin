using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class IsSkillCool : BTDecorator
    {
        public override Boolean DryRun(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            if (f.TryGet(e, out Skill skill) == false)
            {
                return true;
            }

            var now = f.Number * f.DeltaTime;
            return skill.AvailableTime <= now;
        }
    }
}