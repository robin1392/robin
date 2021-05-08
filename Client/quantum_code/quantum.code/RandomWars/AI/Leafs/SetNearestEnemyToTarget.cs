using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    public unsafe partial class SetNearestEnemyToTarget : BTLeaf
    {
        public AIBlackboardValueKey EnemyTargetRef;
        public AIBlackboardValueKey IsEnemyTargetAttacked;

        protected override BTStatus OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var bb = p.Blackboard;
            
            var target = bb->GetEntityRef(f, EnemyTargetRef.Key);
            if (f.Exists(target) == false)
            {
                bb->Set(f, IsEnemyTargetAttacked.Key, false);
            }
            
            if (target == EntityRef.None)
            {
                bb->Set(f, IsEnemyTargetAttacked.Key, false);
            }
            
            var isEnemyTargetAttacked = bb->GetBoolean(f, IsEnemyTargetAttacked.Key);
            if (isEnemyTargetAttacked == true)
            {
                return BTStatus.Success;
            }
            
            var nearest = BTHelper.GetNearestEnemy(p);
            bb->Set(p.Frame, EnemyTargetRef.Key, nearest);

            if (nearest != EntityRef.None)
            {
                return BTStatus.Success;    
            }

            return BTStatus.Failure;
        }
    }
}