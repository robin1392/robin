using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class MoveStraight : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var movable = f.Get<Movable>(e);
            var transform = f.Unsafe.GetPointer<Transform2D>(e);
            var step = transform->Up * movable.MoveSpeed * f.DeltaTime;
            transform->Position += step;
            
            return BTStatus.Running;
        }
    }
}