
namespace Service.Net 
{
    public enum ENetState : byte
    {
        None,

        // 접속 중
        Connecting,

        // 접속 완료
        Connected,

        // 재접속 중
        Reconnecting,

        // 재접속 완료
        Reconnected,

        // 연결 해제 중
        Disconnecting,

        // 연결 해제 완료
        Disconnected,

        // 종료
        End,
    }
}