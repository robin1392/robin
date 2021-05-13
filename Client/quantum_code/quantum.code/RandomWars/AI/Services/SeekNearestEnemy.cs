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
            EntityRef previousTarget = bb->GetEntityRef(f, EnemyTargetRef.Key);
            if (isEnemyTargetAttacked == true && f.Has<Tower>(previousTarget) == false)
            {
                return;
            }
            
            var transform = f.Get<Transform2D>(e);
            var attackable = f.Get<Attackable>(e);
            var actor = f.Get<Actor>(e);
            var hits = f.Physics2D.OverlapShape(transform, Shape2D.CreateCircle(attackable.SearchRange));
            var nearest = EntityRef.None;
            var nearestDistanceSquared = FP.MaxValue;
            EntityRef tower = EntityRef.None;
            for (int i = 0; i < hits.Count; i++)
            {
                var hitEntity = hits[i].Entity;
                if (hitEntity == EntityRef.None)
                {
                    continue;
                }

                if (f.Has<Hittable>(hitEntity) == false)
                {
                    continue;
                }

                if (f.TryGet(hitEntity, out Actor target))
                {
                    if (actor.Team == target.Team)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                if (f.Has<Tower>(hitEntity))
                {
                    tower = hitEntity;
                    continue;
                }
                
                if (f.TryGet(hits[i].Entity, out Transform2D targetTransform))
                {
                    var distanceSquared = FPVector2.DistanceSquared(targetTransform.Position, transform.Position); 
                    if (distanceSquared < nearestDistanceSquared)
                    {
                        nearestDistanceSquared = distanceSquared;
                        nearest = hits[i].Entity;
                    }
                }
            }

            var selected = nearest != EntityRef.None ? nearest : tower; 
            if (selected != previousTarget)
            {
                bb->Set(f, EnemyTargetRef.Key, selected)->TriggerDecorators(p);
                bb->Set(f, IsEnemyTargetAttacked.Key, false);
            }
        }
    }
}
