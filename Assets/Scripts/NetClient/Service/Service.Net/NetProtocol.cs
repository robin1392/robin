namespace Service.Net 
{
    public enum ENetProtocol
    {
        // 세션 인증
        AUTH_SESSION_REQ = 1,
        AUTH_SESSION_ACK,

        // 세션 접속 종료 알림.
        DISCONNECT_SESSION_NOTIFY,

        // 세션 일시 정지
        PAUSE_SESSION_REQ,
        PAUSE_SESSION_ACK,

        // 세션 재개
        RESUME_SESSION_REQ,
        RESUME_SESSION_ACK,
    }   
}