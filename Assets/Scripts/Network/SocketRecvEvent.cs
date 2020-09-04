﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
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
        UnityUtil.Print(" join recv ", "errocode : " + msg.ErrorCode, "white");
        UnityUtil.Print("join my info ", msg.PlayerInfo.Name + " , " + msg.PlayerInfo.IsBottomPlayer, "white");

        NetworkManager.Get().GetNetInfo().SetPlayerInfo(msg.PlayerInfo);
        NetworkManager.Get().IsMaster = msg.PlayerInfo.IsBottomPlayer;
        GameStateManager.Get().CheckSendInGame();

    }
    #endregion
    
    #region leave
    public void OnLeaveGameAck(IPeer peer, MsgLeaveGameAck msg)
    {
        UnityUtil.Print(" leave recv ", "errocode : " + msg.ErrorCode, "white");
        
        
        if( InGameManager.Get() != null )
            InGameManager.Get().CallBackLeaveRoom();
    }
    #endregion
    
    #region ready game

    public void OnReadyGameAck(IPeer peer, MsgReadyGameAck msg)
    {
        UnityUtil.Print(" ready recv ", "errocode : " + msg.ErrorCode, "white");
    }
    #endregion
    
    #region deck



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
        UnityUtil.Print("other info ", msg.OtherPlayerInfo.Name + " , " + msg.OtherPlayerInfo.IsBottomPlayer, "white");
        
        NetworkManager.Get().GetNetInfo().SetOtherInfo(msg.OtherPlayerInfo);
        
        GameStateManager.Get().CheckSendInGame();
    }
    
    public void OnDeactiveWaitingObjectNotify(IPeer peer, MsgDeactiveWaitingObjectNotify msg)
    {
        UnityUtil.Print("Notify Wait" , "DeActive Wait Game Start" , "white");
        
        // 둘다 준비 끝낫다고 노티 이므로 
        // 게임 시작하자
        if (InGameManager.Get() != null)
            InGameManager.Get().NetStartGame();
        // 
    }

    public void OnLeaveGameNotify(IPeer peer, MsgLeaveGameNotify msg)
    {
        UnityUtil.Print("Notify Leave" , msg.PlayerUId.ToString() , "white");

        if (InGameManager.Get() != null)
            InGameManager.Get().OnOtherLeft(msg.PlayerUId);
    }

    public void OnGetDiceNotify(IPeer peer, MsgGetDiceNotify msg)
    {
        
    }

    public void OnSpawnNotify(IPeer peer, MsgSpawnNotify msg)
    {
        
    }

    public void OnAddSpNotify(IPeer peer, MsgAddSpNotify msg)
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
