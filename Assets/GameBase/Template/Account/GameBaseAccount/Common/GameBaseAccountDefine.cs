namespace Template.Account.GameBaseAccount.Common
{
    public enum EGameBaseAccountErrorCode
    {
        Success = 0,
        Fatal,
    }


    public enum EPlatformType
    {
        None,
        Guest,
        Android,
        IOS,
        Facebook,

        Max,
    }


    // 계정 상태
    public enum EAccountStatus
    {
        // 정상
        None,
        // 제재
        Block,
        // 탈퇴
        Withdrawal,
    }


    // 계정 레벨
    public enum EAccountLevel
    {
        None,
        // 일반 유저
        User,
        // 운영자
        Operator,
        // 개발자
        Developer,
        // 관리자
        Administrator,
    }
}