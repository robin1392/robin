using System;
using System.Collections.Generic;
using System.Text;

namespace RWGameProtocol
{
    public enum GameErrorCode : short
    {
        SUCCESS = 0,

        ERROR_GAMELIFT_ACCEPT_PLAYER_SESSION                         = 10001,            // 게임 리프트 플레이어 세션 ID를 확인 할 수 없습니다.
        ERROR_GAMELIFT_REMOVE_PLAYER_SESSION                        = 10002,        // 게임 리프트 플레이어 세션 ID를 제거 할 수 없습니다.

        ERROR_DATABASE_UNEXPECTED                          = 10011,             // 데이터베이스의 예상치 못한 에러

        ERROR_GAME_ROOM_NOT_FOUND           = 10101,          // 게임방을 찾을 수 없습니다.
        ERROR_GAME_ROOM_PLAYER_JOIN             = 10102,          // 게임방 참여에 실패했습니다.
        ERROR_GAME_ROOM_PLAYER_LEAVE            = 10103,          // 게임방 퇴장에 실패했습니다.
        ERROR_GAME_PLAYER_NOT_FOUND             = 10104,            // 플레이어를 찾을 수 없습니다.
        ERROR_GAME_PLAYER_INVALID_STATE         = 10105,            // 플레이어를 상태가 유효하지 않습니다.
        ERROR_GET_DICE_FAILED                           = 10106,            // 주사위 생성에 실패했습니다.
        ERROR_LEVELUP_DICE_FAILED                   = 10107,            // 주사위 강화에 실패했습니다.
        ERROR_INGAME_UP_DICE_FAILED                   = 10108,            // 주사위 강화에 실패했습니다.
    }
}
