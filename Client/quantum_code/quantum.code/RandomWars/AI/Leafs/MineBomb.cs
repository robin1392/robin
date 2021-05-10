using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class MineBomb : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var transform = f.Get<Transform2D>(e);
            var attackable = f.Get<Attackable>(e);
            var actor = f.Get<Actor>(e);
            BTHelper.DamageToCircleArea(f, transform, attackable.Range, p.Entity, actor, attackable.Effect);
            f.Add<Destroy>(e);

            return BTStatus.Success;
        }
    }
}