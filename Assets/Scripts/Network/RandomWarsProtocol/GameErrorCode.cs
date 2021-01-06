using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsProtocol
{
    public enum GameErrorCode : int
    {
        SUCCESS = 0,
        ERROR_TABLE_NOT_FOUND,

        ERROR_GAMELIFT_ACCEPT_PLAYER_SESSION = 1001,            // 게임 리프트 플레이어 세션 ID를 확인 할 수 없습니다.
        ERROR_GAMELIFT_REMOVE_PLAYER_SESSION = 1002,        // 게임 리프트 플레이어 세션 ID를 제거 할 수 없습니다.
        ERROR_GAMELIFT_MATCH_PLACING = 1003,
        ERROR_GAMELIFT_MATCH_TICKET = 1004,
        ERROR_GAMELIFT_MATCH_STATE_INVALID = 1005,

        ERROR_DATABASE_UNEXPECTED = 2011,             // 데이터베이스의 예상치 못한 에러



        ERROR_USER_NOT_FOUND = 10001,               // 유저를 찾을 수 없음.
        ERROR_USER_NAME_DUPLICATED = 10001,     // 유저명 중복

        ERROR_BOX_NOT_FOUND = 10101,                // box를 찾을 수 없습니다.
        ERROR_BOX_COUNT_LACK = 10102,                // box 수량이 부족하다.

        ERROR_DICE_LEVELUP_DATA_NOT_FOUND = 10201,            // 주사위 레벨업 골드 부족
        ERROR_DICE_LEVELUP_LACK_GOLD = 10202,            // 주사위 레벨업 골드 부족
        ERROR_DICE_LEVELUP_LACK_DICE = 10203,            // 주사위 레벨업 필요 주사위 부족


        ERROR_GAME_ROOM_NOT_FOUND = 20101,          // 게임방을 찾을 수 없습니다.
        ERROR_GAME_ROOM_PLAYER_JOIN             = 20102,          // 게임방 참여에 실패했습니다.
        ERROR_GAME_ROOM_PLAYER_LEAVE            = 20103,          // 게임방 퇴장에 실패했습니다.
        ERROR_GAME_PLAYER_NOT_FOUND             = 20104,            // 플레이어를 찾을 수 없습니다.
        ERROR_GAME_PLAYER_INVALID_STATE         = 20105,            // 플레이어를 상태가 유효하지 않습니다.
        ERROR_GET_DICE_FAILED                           = 20106,            // 주사위 생성에 실패했습니다.
        ERROR_LEVELUP_DICE_FAILED                   = 20107,            // 주사위 강화에 실패했습니다.
        ERROR_INGAME_UP_DICE_FAILED                   = 20108,            // 주사위 강화에 실패했습니다.
        ERROR_HIT_DAMAGE_FAILED = 20109,            // 데미지 적용에 실패했습니다.

        ERROR_SEASON_PASS_TROPHY_LACK   = 20201,          // 시즌 트로피가 부족합니다.
        ERROR_SEASON_PASS_NEED              = 20202,          // 시즌 패스를 구입해야 합니다.
        ERROR_SEASON_PASS_INVALID_REWARD_ID = 20203,          // 시즌 패스 보상이 유효하지 않습니다.
    
        ERROR_QUEST_DATA_RESET_FAILED            = 20301,            // 퀘스트 데이터 리셋에 실패했습니다.
        ERROR_QUEST_INVALID_STATUS              = 20302,            // 퀘스트의 상태가 유효하지 않습니다.
        ERROR_QUEST_FAILED_CREATE_REWARD        = 20303,            // 퀘스트 보상 생성에 실패했습니다.


        ERROR_QUEST_DAY_REWARD_INVALID_TIME = 20401,            // 퀘스트의 일일 보상 시간이 유효화지 않습니다.
        ERROR_QUEST_DAY_REWARD_INVALID_INDEX = 20402,           // 퀘스트의 일일 보상이 이미 지급되었다.

    }
}
