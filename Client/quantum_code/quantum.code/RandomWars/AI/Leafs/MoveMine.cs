using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class MoveMine : BTLeaf
    {
        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var mine = f.Get<Mine>(e);
            var transform = f.Unsafe.GetPointer<Transform2D>(e);

            var currentTime = f.Number * f.DeltaTime;
            var elapsed = currentTime - mine.SpawnTime;
            var duration = mine.ArriveTime - mine.SpawnTime;
            var t = elapsed / duration;
            transform->Position = FPVector2.Lerp(mine.StartPosition, mine.Destination, t);

            if (currentTime > mine.ArriveTime)
            {
                var minePointer = f.Unsafe.GetPointer<Mine>(e);
                minePointer->Arrived = true;
                f.Events.ActionChanged(e, ActionStateType.Idle);
                f.Events.PlaySound(e, "Sound_MineDrop");
                return BTStatus.Success;
            }
            
            return BTStatus.Running;
        }
    }
}