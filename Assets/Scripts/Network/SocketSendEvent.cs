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
                break;
            }
            case GameProtocol.READY_GAME_REQ:
            {
                //ReadyGameReq(IPeer peer)
                break;
            }
            case GameProtocol.CHANGE_LAYER_REQ:
            {
                
                break;
            }
            case GameProtocol.SET_DECK_REQ:
            {
                //SetDeckReq(IPeer peer, int[] deck)
                break;
            }
            case GameProtocol.GET_DICE_REQ:
            {
                //GetDiceReq(IPeer peer, int useSp)
                break;
            }
            case GameProtocol.HIT_DAMAGE_REQ:
            {
                //HitDamageReq(IPeer peer, float damage, float delay)
                break;
            }
            case GameProtocol.LEVEL_UP_DICE_REQ:
            {
                //LevelUpDiceReq(IPeer peer, int resetFieldNum, int leveupFieldNum)
                break;
            }
            case GameProtocol.UPGRADE_DICE_REQ:
            {
                break;
            }
        }
    }
    
    
    
    
    
    
}
