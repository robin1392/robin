using System;

namespace RWGameProtocol
{
    public enum GameProtocol : int
    {
        BEGIN = 100000,

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

        PAUSE_GAME_REQ,
        PAUSE_GAME_ACK,
        PAUSE_GAME_NOTIFY,

        RESUME_GAME_REQ,
        RESUME_GAME_ACK,
        RESUME_GAME_NOTIFY,

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

        LEVEL_UP_DICE_REQ,
        LEVEL_UP_DICE_ACK,
        LEVEL_UP_DICE_NOTIFY,

        INGAME_UP_DICE_REQ,
        INGAME_UP_DICE_ACK,
        INGAME_UP_DICE_NOTIFY,

        UPGRADE_SP_REQ,
        UPGRADE_SP_ACK,
        UPGRADE_SP_NOTIFY,


        // ----------------------------------------------------------------------------------------------------
        // Notify protocols
        // ----------------------------------------------------------------------------------------------------
        DEACTIVE_WAITING_OBJECT_NOTIFY = 200001,
        ADD_SP_NOTIFY,
        SPAWN_NOTIFY,
        END_GAME_NOTIFY,
        DISCONNECT_GAME_NOTIFY,


        // ----------------------------------------------------------------------------------------------------
        // Relay protocols
        // ----------------------------------------------------------------------------------------------------
        BEGIN_PROTOCOL_RELAY = 300000,

        REMOVE_MINION_RELAY,
        HIT_DAMAGE_MINION_RELAY,
        DESTROY_MINION_RELAY,
        HEAL_MINION_RELAY,
        PUSH_MINION_RELAY,
        SET_MINION_ANIMATION_TRIGGER_RELAY,
        FIRE_ARROW_RELAY,
        FIRE_BALL_BOMB_RELAY,
        MINE_BOMB_RELAY,
        REMOVE_MAGIC_RELAY,
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
        MINION_FOG_OF_WAR_RELAY,
        SEND_MESSAGE_VOID_RELAY,
        SEND_MESSAGE_PARAM1_RELAY,
        NECROMANCER_BULLET_RELAY,
        SET_MINION_TARGET_RELAY,
        MINION_STATUS_RELAY,
        SCARECROW_RELAY,
        LAYZER_TARGET_RELAY,
        FIRE_BULLET_RELAY,
        MINION_INVINCIBILITY_RELAY,


        END_PROTOCOL_RELAY,

        END,
    };
}