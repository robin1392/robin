namespace Service.Template
{   
    public enum EGameRuleType
    {
        None,
        StartWave,
        AddSp,
        SpawnMonster,
    }
    

    public interface IGameRule
    {
        EGameRuleType CheckRuleCondition();

        bool DoRuleAction(EGameRuleType ruleType);
    }
}