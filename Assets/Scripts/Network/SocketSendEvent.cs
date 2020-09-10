﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using RWCoreNetwork;
using RWGameProtocol;
using RWGameProtocol.Msg;



public class SocketSendEvent
{

    private GamePacketSender _sender; 
    public SocketSendEvent(GamePacketSender sender)
    {
        this._sender = sender;
    }


    public void SendPacket(GameProtocol protocol , IPeer peer , params object[] param)
    {
        switch (protocol)
        {
            case GameProtocol.JOIN_GAME_REQ:
            {
                _sender.JoinGameReq(peer , (string)param[0]);
                break;
            }
            case GameProtocol.LEAVE_GAME_REQ:
            {
                _sender.LeaveGameReq(peer);
                break;
            }
            case GameProtocol.READY_GAME_REQ:
            {
                _sender.ReadyGameReq(peer);
                break;
            }
            case GameProtocol.GET_DICE_REQ:
            {
                _sender.GetDiceReq(peer);
                break;
            }
            case GameProtocol.LEVEL_UP_DICE_REQ:
            {
                _sender.LevelUpDiceReq(peer , (short)param[0] , (short)param[1]);
                break;
            }
            case GameProtocol.UPGRADE_SP_REQ:
            {
                _sender.UpgradeSpReq(peer);
                break;
            }
            case GameProtocol.INGAME_UP_DICE_REQ:
            {
                _sender.InGameUpDiceReq(peer, (int) param[0]);
                break;
            }
            case GameProtocol.HIT_DAMAGE_REQ:
            {
                float damage = (float)param[0] * Global.g_networkBaseValue;
                _sender.HitDamageReq(peer , (int)damage);
                break;
            }
            
            #region relay

            case GameProtocol.REMOVE_MINION_RELAY:
            {
                _sender.RemoveMinionRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.DESTROY_MINION_RELAY:
            {
                _sender.DestroyMinionRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.REMOVE_MAGIC_RELAY:
            {
                //RemoveMagicRelay(IPeer peer, int playerUId, int id)
                break;
            }
            case GameProtocol.HIT_DAMAGE_MINION_RELAY:
            {
                float damage = (float)param[2] * Global.g_networkBaseValue;
                float delay = (float)param[3] * Global.g_networkBaseValue;
                _sender.HitDamageMinionRelay(peer , (int)param[0] , (int)param[1] , (int)damage , (int)delay);
                break;
            }
            
            case GameProtocol.HEAL_MINION_RELAY:
            {
                float serverHeal = (float)param[2] * Global.g_networkBaseValue;
                _sender.HealMinionRelay(peer , (int)param[0] , (int)param[1] ,(int)serverHeal);
                break;
            }
            case GameProtocol.PUSH_MINION_RELAY:
            {
                //PushMinionRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int pushPower)
                break;
            }
            case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
            {
                _sender.SetMinionAnimationTriggerRelay(peer, (int) param[0], (int) param[1], (string) param[2]);
                break;
            }
            case GameProtocol.FIRE_ARROW_RELAY:
            {
                //FireArrowRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int damage)
                break;
            }
            case GameProtocol.FIREBALL_BOMB_RELAY:
            {
                //FireballBombRelay(IPeer peer, int playerUId, int id)
                break;
            }
            case GameProtocol.MINE_BOMB_RELAY:
            {
                //MineBombRelay(IPeer peer, int playerUId, int id)
                break;
            }
            
            case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
            {
                //SetMagicTargetIdRelay(IPeer peer, int playerUId, int id, int targetId)
                break;
            }
            case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
            {
                //SetMagicTargetRelay(IPeer peer, int playerUId, int id, int x, int z)
                break;
            }
            
            #endregion
            
        }
    }
    
    
    
    
    
    
}