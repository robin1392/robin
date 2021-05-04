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
    }
}