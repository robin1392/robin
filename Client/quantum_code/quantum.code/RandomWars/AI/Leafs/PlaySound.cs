using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class PlaySound : BTLeaf
    {
        public string Sound;
        
        protected override BTStatus OnUpdate(BTParams p)
        {
            var e = p.Entity;
            var f = p.Frame;

            f.Events.PlaySound(e, Sound);
            return BTStatus.Success;
        }
    }
}