namespace Template.Stage.RandomWarsMatch.Common
{
    public enum ERandomWarsMatchErrorCode
    {
        SUCCESS = 0,
        // 알수 없는 심각한 에러가 발생했습니다.
        FATAL = 20000000,
        // 매치 플레이어를 찾을 수 없습니다.
        MATCH_PLAYER_NOT_FOUND,
        // 매치 플레이어 게임 상태가 유효하지 않습니다.
        MATCH_PLAYER_INVALID_STATE,
    }
}