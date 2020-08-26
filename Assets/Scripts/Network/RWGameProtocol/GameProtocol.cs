using System;

namespace RWGameProtocol
{
    public enum GameProtocol : short
    {
        BEGIN = 0,

        JOIN_GAME_REQ,
        JOIN_GAME_ACK,

        LEAVE_GAME_REQ,
        LEAVE_GAME_ACK,

        READY_GAME_REQ,
        READY_GAME_ACK,

        CHANGE_LAYER_REQ,
        CHANGE_LAYER_ACK,

        SET_DECK_REQ,
        SET_DECK_ACK,

        GET_DICE_REQ,
        GET_DICE_ACK,

        HIT_DAMAGE_REQ,
        HIT_DAMAGE_ACK,

        LEVEL_UP_DICE_REQ,
        LEVEL_UP_DICE_ACK,

        UPGRADE_DICE_REQ,
        UPGRADE_DICE_ACK,


        // ----------------------------------------------------------------------------------------------------
        // �˸� ��Ŷ ��ȣ �뿪
        // ----------------------------------------------------------------------------------------------------
        
        // �÷��̾� ���� ���� �˸�
        JOIN_GAME_NOTIFY = 10001,

        // �÷��̾� ���� ���� �˸�
        LEAVE_GAME_NOTIFY,

        // ��� ���� ���� �˸�
        DEACTIVE_WAITING_OBJECT_NOTIFY,

        GET_DICE_NOTIFY,
        
        ADD_SP_NOTIFY,
        
        SPAWN_NOTIFY,
        
        LEVEL_UP_DICE_NOTIFY,

        UPGRADE_DICE_NOTIFY,


        // ----------------------------------------------------------------------------------------------------
        // ProtocolId 20001 ���ʹ� relay ��Ŷ�� �����Ѵ�.
        // relay ��Ŷ�� ��Ŷ �����͸� �м����� �ʰ� ��󿡰� �ٷ� �����Ѵ�.
        // ----------------------------------------------------------------------------------------------------
        REMOVE_MINION_RELAY = 20001,

        HIT_DAMAGE_MINION_RELAY,

        DESTROY_MINION_RELAY,

        HEAL_MINION_RELAY,

        PUSH_MINION_RELAY,

        SET_MINION_ANIMATION_TRIGGER_RELAY,

        FIRE_ARROW_RELAY,

        FIREBALL_BOMB_RELAY,

        MINE_BOMB_RELAY,

        REMOVE_MAGIC_RELAY,

        SET_MAGIC_TARGET_ID_RELAY,

        SET_MAGIC_TARGET_POS_RELAY,



        END,
    };
}