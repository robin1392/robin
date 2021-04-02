using System;

namespace Service.Net
{
    public enum ESessionState : short
    {
        None = 0,
        // 재연결 대기 상태
        Wait,
        // 서버에 의한 블럭 상태
        Blocked,
        // 세션 만료 상태
        Expired,
        // 중복 세션
        Duplicated,
        // 타임아웃
        TimeOut,
    }
}