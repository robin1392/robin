﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RWGameProtocol
{
    public enum GameErrorCode : short
    {
        SUCCESS = 0,

        ERROR_ACCEPT_PLAYER_SESSION                         = 10001,            // 플레이어 세션 ID를 확인 할 수 없습니다.

        ERROR_GAME_ROOM_NOT_FOUND           = 10101,          // 게임방을 찾을 수 없습니다.
        ERROR_GAME_ROOM_PLAYER_JOIN             = 10102,          // 게임방 참여에 실패했습니다.
        ERROR_GAME_PLAYER_NOT_FOUND             = 10103,            // 플레이어를 찾을 수 없습니다.
        ERROR_GAME_PLAYER_INVALID_STATE         = 10104,            // 플레이어를 상태가 유효하지 않습니다.
        ERROR_GET_DICE_FAILED                           = 10105,            // 주사위 생성에 실패했습니다.
    }
}