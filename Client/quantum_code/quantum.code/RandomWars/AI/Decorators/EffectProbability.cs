using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class EffectProbability : BTDecorator
    {
        public override Boolean DryRun(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var actor = f.Get<Actor>(e);
            var random = f.RNG->Next(0, 100);
            return actor.EffectProbability > random;
        }
    }
}