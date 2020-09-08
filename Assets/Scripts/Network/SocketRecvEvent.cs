﻿using System.Collections;
using System.Collections.Generic;
using RWCoreNetwork;
using RWGameProtocol.Msg;
using UnityEngine;


public class SocketRecvEvent 
{

    #region variable
    #endregion
    
    
    #region init 
    // 
    public SocketRecvEvent()
    {
    }
    
    
    #endregion
    
    
    
    
    #region join room


    // <summary>
    /// 게임 참가 응답 처리부
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="msg"></param>
    public void OnJoinGameAck(IPeer peer, MsgJoinGameAck msg)
    {
        // something to do...

        //NetworkManager.Get().SendSocket.ReadyGameReq(peer);
        //SendSocket.ReadyGameReq(peer);
    }
    #endregion
    
    #region leave
    public void OnLeaveGameAck(IPeer peer, MsgLeaveGameAck msg)
    {
        
    }
    #endregion
    
    #region ready game

    public void OnReadyGameAck(IPeer peer, MsgReadyGameAck msg)
    {
        
    }
    #endregion
    
    #region deck

    public void OnSetDeckAck(IPeer peer, MsgSetDeckAck msg)
    {
        
    }

    #endregion
    
    
    #region dice

    public void OnGetDiceAck(IPeer peer, MsgGetDiceAck msg)
    {
        
    }

    public void OnLevelUpDiceAck(IPeer peer, MsgLevelUpDiceAck msg)
    {
        
    }
    #endregion
    
    
    #region damage

    public void OnHitDamageAck(IPeer peer, MsgHitDamageAck msg)
    {
        
    }
    #endregion
    
    
    #region notify

    public void OnJoinGameNotify(IPeer peer, MsgJoinGameNotify msg)
    {
        
    }

    public void OnLeaveGameNotify(IPeer peer, MsgLeaveGameNotify msg)
    {
        
    }

    public void OnGetDiceNotify(IPeer peer, MsgGetDiceNotify msg)
    {
        
    }

    public void OnDeactiveWaitingObjectNotify(IPeer peer, MsgDeactiveWaitingObjectNotify msg)
    {
        
    }
    #endregion
    
    #region relay

    public void OnRemoveMinionRelay(IPeer peer, MsgRemoveMinionRelay msg)
    {
        
    }

    public void OnHitDamageMinionRelay(IPeer peer, MsgHitDamageMinionRelay msg)
    {
        
    }

    public void OnDestroyMinionRelay(IPeer peer, MsgDestroyMinionRelay msg)
    {
        
    }

    public void OnHealMinionRelay(IPeer peer, MsgHealMinionRelay msg)
    {
        
    }

    public void OnPushMinionRelay(IPeer peer, MsgPushMinionRelay msg)
    {
        
    }

    public void OnSetMinionAnimationTriggerRelay(IPeer peer, MsgSetMinionAnimationTriggerRelay msg)
    {
        
    }

    public void OnRemoveMagicRelay(IPeer peer, MsgRemoveMagicRelay msg)
    {
        
    }

    public void OnFireArrowRelay(IPeer peer, MsgFireArrowRelay msg)
    {
        
    }

    public void OnFireballBombRelay(IPeer peer, MsgFireballBombRelay msg)
    {
        
    }

    public void OnMineBombRelay(IPeer peer, MsgMineBombRelay msg)
    {
        
    }

    public void OnSetMagicTargetIdRelay(IPeer peer, MsgSetMagicTargetIdRelay msg)
    {
        
    }

    public void OnSetMagicTargetRelay(IPeer peer, MsgSetMagicTargetRelay msg)
    {
        
    }
    #endregion
    

}