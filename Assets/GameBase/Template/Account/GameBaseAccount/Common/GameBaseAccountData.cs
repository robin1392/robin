namespace Template.Account.GameBaseAccount.Common
{
    public class AccountInfo
    {
        // 엑세스 토큰
        public string AccessToken;
        // 플랫폼 아이디
        public string PlatformId;
        // 플랫폼 타입(GUEST, Android, IOS, Guest 등)
        public int PlatformType;
        // 계정 상태(정상, 탈퇴, 제재 등)
        public int Status;
        // 상태 남은 시간(정상이면 0)
        public int StatusRemainTime;
    }
}