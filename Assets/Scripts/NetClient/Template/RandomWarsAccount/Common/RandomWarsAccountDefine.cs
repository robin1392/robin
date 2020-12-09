namespace Template.Account.RandomWarsAccount.Common
{
    public enum ERandomWarsAccountErrorCode
    {
        SUCCESS = 0,
        FATAL = ERandomWarsAccountProtocol.BEGIN,
        NOT_FOUND_ACCOUNT,
    }


    public enum EPlatformType
    {
        NONE,
        GUEST,
        GOOGLE,
        IOS,
        FACEBOOK,

        END
    }
}