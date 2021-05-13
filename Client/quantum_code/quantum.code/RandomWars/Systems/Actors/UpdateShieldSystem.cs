using System;
using Photon.Deterministic;

namespace Quantum.Actors
{
    public unsafe class UpdateShieldSystem : SystemSignalsOnly, ISignalOnComponentAdded<Shield>, ISignalOnComponentRemoved<Shield>
    {
        public void OnAdded(Frame f, EntityRef entity, Shield* component)
        {
            var buff = f.Unsafe.GetPointer<Buff>(entity);
            buff->BuffState = buff->BuffState | (Int32)(BuffType.Shield);
        }

        public void OnRemoved(Frame f, EntityRef entity, Shield* component)
        {
            var buff = f.Unsafe.GetPointer<Buff>(entity);
            buff->BuffState = buff->BuffState & (Int32)(~BuffType.Shield);
            
        }
    }
}