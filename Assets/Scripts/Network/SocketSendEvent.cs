using System.Collections;
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
                //JoinGameReq(IPeer peer, string playerSessionId)
                break;
            }
            case GameProtocol.LEAVE_GAME_REQ:
            {
                //LeaveGameReq(IPeer peer, string playerSessionId)
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
            case GameProtocol.HIT_DAMAGE_REQ:
            {
                //HitDamageReq(IPeer peer, float damage, float delay)
                break;
            }
            case GameProtocol.LEVEL_UP_DICE_REQ:
            {
                _sender.LevelUpDiceReq(peer , (short)param[0] , (short)param[1]);
                //LevelUpDiceReq(IPeer peer, int resetFieldNum, int leveupFieldNum)
                break;
            }
            case GameProtocol.INGAME_UP_DICE_REQ:
            {
                break;
            }
            case GameProtocol.UPGRADE_SP_REQ:
            {
                break;
            }
        }
    }
    
    
    
    
    
    
}
