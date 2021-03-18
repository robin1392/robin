namespace Template.Match.RandomwarsMatch.Common
{
    public enum ERandomwarsMatchErrorCode
    {
        Success = 0,
        Fatal,

        MatchmakingTimedOut,
    }


    public enum EGameMode
    {
        None = 0,
        DeathMatch,
        Coop
    }

}