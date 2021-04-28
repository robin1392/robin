namespace Quantum
{
    public unsafe class BTHelper
    {
        // Used to either initialize an entity as a bot on the beginning of the match
        // or to turn a player entity into a bot when the player gets disconnected
        public static void SetupBT(Frame f, EntityRef entityRef)
        {
            // Create the BT Agent and pick the AIConfig, if there is any
            var btAgent = new BTAgent();
            f.Set(entityRef, btAgent);

            var btRoot = f.FindAsset<BTRoot>("Resources/DB/CircuitExport/BT_Assets/Melee");
            BTManager.Init(f, entityRef, btRoot);

            // Setup the blackboard
            var blackboardComponent = new AIBlackboardComponent();
            var blackboardPath = "Resources/DB/CircuitExport/Blackboard_Assets/MeleeBlackboardInitializer";
            var bbInitializerAsset = f.FindAsset<AIBlackboardInitializer>(blackboardPath);
            AIBlackboardInitializer.InitializeBlackboard(f, &blackboardComponent, bbInitializerAsset);
            f.Set(entityRef, blackboardComponent);
        }
    }
}