using System;

namespace RandomWarsProtocol
{
    public enum GameProtocol : int
    {
        // ----------------------------------------------------------------------------------------------------
        // http protocols
        // ----------------------------------------------------------------------------------------------------
        BEGIN_HTTP = 100000,

        AUTH_USER_REQ,
        AUTH_USER_ACK,

        EDIT_USER_NAME_REQ,
        EDIT_USER_NAME_ACK,

        UPDATE_DECK_REQ,
        UPDATE_DECK_ACK,

        END_TUTORIAL_REQ,
        END_TUTORIAL_ACK,

        START_MATCH_REQ,
        START_MATCH_ACK,

        STATUS_MATCH_REQ,
        STATUS_MATCH_ACK,

        STOP_MATCH_REQ,
        STOP_MATCH_ACK,

        OPEN_BOX_REQ,
        OPEN_BOX_ACK,

        LEVELUP_DICE_REQ,
        LEVELUP_DICE_ACK,

        SEASON_INFO_REQ,
        SEASON_INFO_ACK,

        SEASON_RESET_REQ,
        SEASON_RESET_ACK,

        GET_RANK_REQ,
        GET_RANK_ACK,

        SEASON_PASS_INFO_REQ,
        SEASON_PASS_INFO_ACK,

        SEASON_PASS_REWARD_STEP_REQ,
        SEASON_PASS_REWARD_STEP_ACK,

        GET_SEASON_PASS_REWARD_REQ,
        GET_SEASON_PASS_REWARD_ACK,

        CLASS_REWARD_INFO_REQ,
        CLASS_REWARD_INFO_ACK,

        GET_CLASS_REWARD_REQ,
        GET_CLASS_REWARD_ACK,

        QUEST_INFO_REQ,
        QUEST_INFO_ACK,

        QUEST_REWARD_REQ,
        QUEST_REWARD_ACK,

        QUEST_DAY_REWARD_REQ,
        QUEST_DAY_REWARD_ACK,


        END_HTTP,

        // ----------------------------------------------------------------------------------------------------
        // socket protocols
        // ----------------------------------------------------------------------------------------------------
        BEGIN_SOCKET = 200000,

        // ----------------------------------------------------------------------------------------------------
        // req/ack protocols
        // ----------------------------------------------------------------------------------------------------
        JOIN_GAME_REQ,
        JOIN_GAME_ACK,
        JOIN_GAME_NOTIFY,

        LEAVE_GAME_REQ,
        LEAVE_GAME_ACK,
        LEAVE_GAME_NOTIFY,

        READY_GAME_REQ,
        READY_GAME_ACK,

        RECONNECT_GAME_REQ,
        RECONNECT_GAME_ACK,
        RECONNECT_GAME_NOTIFY,

        READY_SYNC_GAME_REQ,
        READY_SYNC_GAME_ACK,
        READY_SYNC_GAME_NOTIFY,

        START_SYNC_GAME_REQ,
        START_SYNC_GAME_ACK,
        START_SYNC_GAME_NOTIFY,

        END_SYNC_GAME_REQ,
        END_SYNC_GAME_ACK,
        END_SYNC_GAME_NOTIFY,

        GET_DICE_REQ,
        GET_DICE_ACK,
        GET_DICE_NOTIFY,

        HIT_DAMAGE_REQ,
        HIT_DAMAGE_ACK,
        HIT_DAMAGE_NOTIFY,

        MERGE_DICE_REQ,
        MERGE_DICE_ACK,
        MERGE_DICE_NOTIFY,

        INGAME_UP_DICE_REQ,
        INGAME_UP_DICE_ACK,
        INGAME_UP_DICE_NOTIFY,

        UPGRADE_SP_REQ,
        UPGRADE_SP_ACK,
        UPGRADE_SP_NOTIFY,


        JOIN_COOP_GAME_REQ,
        JOIN_COOP_GAME_ACK,
        JOIN_COOP_GAME_NOTIFY,


        // ----------------------------------------------------------------------------------------------------
        // Notify protocols
        // ----------------------------------------------------------------------------------------------------
        DEACTIVE_WAITING_OBJECT_NOTIFY = 210000,
        ADD_SP_NOTIFY,
        SPAWN_NOTIFY,
        END_GAME_NOTIFY,
        END_COOP_GAME_NOTIFY,
        DISCONNECT_GAME_NOTIFY,
        PAUSE_GAME_NOTIFY,
        RESUME_GAME_NOTIFY,
        COOP_SPAWN_NOTIFY,
        MONSTER_SPAWN_NOTIFY,


        // ----------------------------------------------------------------------------------------------------
        // Relay protocols
        // ----------------------------------------------------------------------------------------------------
        BEGIN_RELAY = 300000,

        HIT_DAMAGE_MINION_RELAY,
        DESTROY_MINION_RELAY,
        HEAL_MINION_RELAY,
        PUSH_MINION_RELAY,
        SET_MINION_ANIMATION_TRIGGER_RELAY,
        FIRE_ARROW_RELAY,
        FIRE_BALL_BOMB_RELAY,
        MINE_BOMB_RELAY,
        DESTROY_MAGIC_RELAY,
        SET_MAGIC_TARGET_ID_RELAY,
        SET_MAGIC_TARGET_POS_RELAY,
        STURN_MINION_RELAY,
        ROCKET_BOMB_RELAY,
        ICE_BOMB_RELAY,
        FIRE_CANNON_BALL_RELAY,
        FIRE_SPEAR_RELAY,
        FIRE_MAN_FIRE_RELAY,
        ACTIVATE_POOL_OBJECT_RELAY,
        MINION_CLOACKING_RELAY,
        MINION_FLAG_OF_WAR_RELAY,
        SEND_MESSAGE_VOID_RELAY,
        SEND_MESSAGE_PARAM1_RELAY,
        NECROMANCER_BULLET_RELAY,
        SET_MINION_TARGET_RELAY,
        MINION_STATUS_RELAY,
        SCARECROW_RELAY,
        LAYZER_TARGET_RELAY,
        FIRE_BULLET_RELAY,
        MINION_INVINCIBILITY_RELAY,

        END_RELAY,
        END_SOCKET,
    };
}