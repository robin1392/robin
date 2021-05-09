using System;
using Photon.Deterministic;

namespace Quantum
{
    partial struct Actor
    {
        private const Int32 EmptyIndex = -1;

        public FP GetAttackAniSpeed()
        {
            return AttackAniLength / AttackSpeed;
        }
        
        public FP GetAttackHitEvent()
        {
            return AttackSpeed / AttackAniLength * AttackHitEvent;
        }
    }
}