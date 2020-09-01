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
                break;
            }
            case GameProtocol.LEAVE_GAME_REQ:
            {
                break;
            }
            case GameProtocol.READY_GAME_REQ:
            {
                break;
            }
            case GameProtocol.CHANGE_LAYER_REQ:
            {
                break;
            }
            case GameProtocol.SET_DECK_REQ:
            {
                break;
            }
            case GameProtocol.GET_DICE_REQ:
            {
                break;
            }
            case GameProtocol.HIT_DAMAGE_REQ:
            {
                break;
            }
            case GameProtocol.LEVEL_UP_DICE_REQ:
            {
                break;
            }
            case GameProtocol.UPGRADE_DICE_REQ:
            {
                break;
            }
        }
    }
    
    
    
    
    
    
}
