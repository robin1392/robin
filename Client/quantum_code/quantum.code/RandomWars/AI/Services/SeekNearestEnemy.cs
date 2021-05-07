using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe class SeekNearestEnemy : BTService
    {
        public AIBlackboardValueKey EnemyTargetRef;
        public AIBlackboardValueKey IsEnemyTargetAttacked;
        protected override void OnUpdate(BTParams p)
        {
            var f = p.Frame;
            var e = p.Entity;
            var bb = p.Blackboard;
            var isEnemyTargetAttacked = bb->GetBoolean(f, IsEnemyTargetAttacked.Key);
            if (isEnemyTargetAttacked == true)
            {
                return;
            }
            
            var transform = f.Get<Transform2D>(e);
            EntityRef previousTarget = bb->GetEntityRef(f, EnemyTargetRef.Key);
            
            var actor = f.Get<Actor>(e);
            var hits = f.Physics2D.OverlapShape(transform, Shape2D.CreateCircle(actor.SearchRange));
            var nearest = EntityRef.None;
            var nearestDistanceSquared = FP.MaxValue;
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].Entity == EntityRef.None)
                {
                    continue;
                }
                
                if (p.Frame.Unsafe.TryGetPointer(hits[i].Entity, out Actor* target))
                {
                    if (actor.Team == target->Team)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                
                if (p.Frame.TryGet(hits[i].Entity, out Transform2D targetTransform))
                {
                    var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position); 
                    if (distanceSquared < nearestDistanceSquared)
                    {
                        nearestDistanceSquared = distanceSquared;
                        nearest = hits[i].Entity;
                    }
                }
            }
            
            if (nearest != previousTarget)
            {
                bb->Set(f, EnemyTargetRef.Key, nearest)->TriggerDecorators(p);
                bb->Set(f, IsEnemyTargetAttacked.Key, false);
            }
        }
    }
}
