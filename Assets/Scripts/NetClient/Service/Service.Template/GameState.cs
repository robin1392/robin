namespace Service.Template
{
    public enum EGameState
    {
        None,
        // 게임 활성화 상태
        Activate,
        // 플레이어 입장 대기 상태
        Wait,
        // 준비 상태
        Ready,
        // 플레이 중인 상태
        Playing,
        // 게임 종료(승패 결정) 상태
        End,
        // 플레이어가 모두 퇴장한 상태
        Leave,
        // 일시 정지 중인 상태
        Pause,
    }
}