using Photon.Deterministic;

namespace Quantum
{
    public unsafe class BTHelper
    {
        private readonly static string BTFormat = "Resources/DB/CircuitExport/BT_Assets/{0}";
        private readonly static string BTBlackBoardFormat = "Resources/DB/CircuitExport/Blackboard_Assets/{0}BlackboardInitializer";
        
        public static void SetupBT(Frame f, EntityRef entityRef, string btAssetName)
        {
            var btAgent = new BTAgent();
            f.Set(entityRef, btAgent);

            var btRoot = f.FindAsset<BTRoot>(string.Format(BTFormat, btAssetName));
            BTManager.Init(f, entityRef, btRoot);
            
            var blackboardComponent = new AIBlackboardComponent();
            var bbInitializerAsset = f.FindAsset<AIBlackboardInitializer>(string.Format(BTBlackBoardFormat, btAssetName));
            AIBlackboardInitializer.InitializeBlackboard(f, &blackboardComponent, bbInitializerAsset);
            f.Set(entityRef, blackboardComponent);
        }
        
        public static EntityRef GetNearestEnemy(BTParams p)
        {
            var transform = p.Frame.Get<Transform2D>(p.Entity);
            var actor = p.Frame.Get<Actor>(p.Entity);
            var hits = p.Frame.Physics2D.OverlapShape(
                transform, Shape2D.CreateCircle(actor.SearchRange));

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

            return nearest;
        }
    }
}