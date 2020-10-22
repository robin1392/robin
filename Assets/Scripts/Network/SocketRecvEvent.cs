#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
using RWCoreNetwork.NetService;
using RWGameProtocol;
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


    #region pause , resume , reconnect , disconnect

    public void OnPauseGameNotify(Peer peer, MsgPauseGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "PAUSE_GAME_NOTIFY  " + msg.PlayerUId.ToString(), "green");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.PAUSE_GAME_NOTIFY, msg);
    }

    public void OnResumeGameNotify(Peer peer, MsgResumeGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "RESUME_GAME_NOTIFY  " + msg.PlayerUId.ToString(), "green");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.RESUME_GAME_NOTIFY, msg);
    }
    
    public void OnReconnectGameNotify(Peer peer, MsgReconnectGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "RECONNECT_GAME_NOTIFY  " + msg.PlayerUId.ToString(), "green");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.RECONNECT_GAME_NOTIFY, msg);
    }
    
    
    
    
    public void OnDisconnectGameNotify(Peer peer, MsgDisconnectGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "DISCONNECT_GAME_NOTIFY  " + msg.PlayerUId.ToString(), "green");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.DISCONNECT_GAME_NOTIFY, msg);
    }
    public void OnReconnectGameAck(Peer peer, MsgReconnectGameAck msg)
    {
        UnityUtil.Print("ACK => ", "RECONNECT_GAME_ACK  " + msg.ErrorCode.ToString(), "green");

        //if (InGameManager.Get() != null)
        //InGameManager.Get().RecvInGameManager(GameProtocol.RECONNECT_GAME_ACK, msg);
            
        if(NetworkManager.Get() != null)
            NetworkManager.Get().ReconnectPacket(msg);
    }
    public void OnReadySyncGameAck(Peer peer, MsgReadySyncGameAck msg)
    {
        UnityUtil.Print("ACK => ", "READY_SYNC_GAME_ACK  " + msg.ErrorCode.ToString(), "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.READY_SYNC_GAME_ACK, msg);
    }
    public void OnReadySyncGameNotify(Peer peer, MsgReadySyncGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "READY_SYNC_GAME_NOTIFY  " + msg.PlayerUId.ToString(), "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.READY_SYNC_GAME_NOTIFY, msg);
    }
    
    
    
    public void OnStartSyncGameAck(Peer peer, MsgStartSyncGameAck msg)
    {
        UnityUtil.Print("ACK => ", "START_SYNC_GAME_ACK  " + msg.ErrorCode.ToString(), "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.START_SYNC_GAME_ACK, msg);
    }

    public void OnStartSyncGameNotify(Peer peer, MsgStartSyncGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "START_SYNC_GAME_NOTIFY  " + msg.PlayerInfo.PlayerUId.ToString(), "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.START_SYNC_GAME_NOTIFY, msg);
    }

    public void OnEndSyncGameAck(Peer peer, MsgEndSyncGameAck msg)
    {
        UnityUtil.Print("ACK => ", "END_SYNC_GAME_ACK  " + msg.ErrorCode.ToString(), "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.END_SYNC_GAME_ACK, msg);
    }

    public void OnEndSyncGameNotify(Peer peer, MsgEndSyncGameNotify msg)
    {
        UnityUtil.Print("NOTIFY => ", "END_SYNC_GAME_NOTIFY  " , "green");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.END_SYNC_GAME_NOTIFY, msg);
    }


#endregion
    
    


    #region join room


    // <summary>
    /// 게임 참가 응답 처리부
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="msg"></param>
    public void OnJoinGameAck(Peer peer, MsgJoinGameAck msg)
    {
        // something to do...

        //NetworkManager.Get().SendSocket.ReadyGameReq(peer);
        //SendSocket.ReadyGameReq(peer);
        UnityUtil.Print(" join recv ", "errocode : " + msg.ErrorCode, "white");
        UnityUtil.Print("join my info ", msg.PlayerInfo.PlayerUId + "  " +msg.PlayerInfo.Name + " , " + msg.PlayerInfo.IsBottomPlayer, "white");

        //
        NetworkManager.Get().GetNetInfo().SetPlayerInfo(msg.PlayerInfo);
        NetworkManager.Get().IsMaster = msg.PlayerInfo.IsBottomPlayer;
        GameStateManager.Get().CheckSendInGame();

    }

    #endregion
    
    

    #region leave

    public void OnLeaveGameAck(Peer peer, MsgLeaveGameAck msg)
    {
        UnityUtil.Print(" leave recv ", "errocode : " + msg.ErrorCode, "white");


        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEAVE_GAME_ACK);
    }

    #endregion

    #region ready game

    public void OnReadyGameAck(Peer peer, MsgReadyGameAck msg)
    {
        UnityUtil.Print(" ready recv ", "errocode : " + msg.ErrorCode, "white");
    }

    #endregion


    #region dice

    public void OnGetDiceAck(Peer peer, MsgGetDiceAck msg)
    {
        // my dice get
        UnityUtil.Print(" get dice recv ", "errocode : " + msg.ErrorCode, "white");

        // 차후엔 에러처리를 해야된다..
        if (msg.ErrorCode != 0)
        {
            UnityUtil.Print(" get dice recv ", "errocode : " + msg.ErrorCode, "red");
            return;
        }
            

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.GET_DICE_ACK, msg);

    }

    public void OnLevelUpDiceAck(Peer peer, MsgLevelUpDiceAck msg)
    {
        UnityUtil.Print(" level up dice recv ", "errocode : " + msg.ErrorCode, "white");
        
        // 차후엔 에러처리를 해야된다..
        if (msg.ErrorCode != 0)
        {
            UnityUtil.Print(" level up dice recv ", "errocode : " + msg.ErrorCode, "red");
            return;
        }
            

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEVEL_UP_DICE_ACK, msg);
    }

    public void OnUpgradeSpAck(Peer peer, MsgUpgradeSpAck msg)
    {
        UnityUtil.Print(" up sp recv ", "errocode : " + msg.ErrorCode, "white");
        
        // 차후엔 에러처리를 해야된다..
        if (msg.ErrorCode != 0)
        {
            UnityUtil.Print(" up sp recv ", "errocode : " + msg.ErrorCode, "red");
            return;
        }
            
                    
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.UPGRADE_SP_ACK, msg);
    }

    public void OnInGameUpDiceAck(Peer peer, MsgInGameUpDiceAck msg)
    {
        UnityUtil.Print(" in game upgrade recv ", "errocode : " + msg.ErrorCode, "white");
        
        // 차후엔 에러처리를 해야된다..
        if (msg.ErrorCode != 0)
        {
            UnityUtil.Print(" in game upgrade recv ", "errocode : " + msg.ErrorCode, "red");
            return;
        }
            
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.INGAME_UP_DICE_ACK, msg);
    }
    
    public void OnHitDamageAck(Peer peer, MsgHitDamageAck msg)
    {
        //UnityUtil.Print(" hit damage recv ", "errocode : " + msg.ErrorCode, "white");
        UnityUtil.Print("RECV => ", "HIT DAMAGE ACK  " +msg.PlayerUId.ToString() + "   " + msg.Damage + "   error code " + msg.ErrorCode , "green");
        
        // 차후엔 에러처리를 해야된다..
        if (msg.ErrorCode != 0)
        {
            UnityUtil.Print("RECV => ", "HIT DAMAGE ACK  " +msg.PlayerUId.ToString() + "   " + msg.Damage + "   error code " + msg.ErrorCode , "red");
            return;
        }
            
        
        //Global.g_networkBaseValue
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_ACK, msg);
    }

    public void HitDamageNotify(Peer peer, MsgHitDamageNotify msg)
    {
        //UnityUtil.Print("hit damage Notify", msg.PlayerUId.ToString() + "  " + msg.Damage.ToString(), "white");
        UnityUtil.Print("RECV => ", "HIT DAMAGE NOTIFY  " +msg.PlayerUId.ToString() + "   " + msg.Damage , "white");
        
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_NOTIFY, msg);
    }
    #endregion

    

    #region notify

    public void OnJoinGameNotify(Peer peer, MsgJoinGameNotify msg)
    {
        UnityUtil.Print("other info ", msg.OtherPlayerInfo.PlayerUId + "  " + msg.OtherPlayerInfo.Name + " , " + msg.OtherPlayerInfo.IsBottomPlayer, "white");

        // menu
        NetworkManager.Get().GetNetInfo().SetOtherInfo(msg.OtherPlayerInfo);
        GameStateManager.Get().CheckSendInGame();
    }

    public void OnDeactiveWaitingObjectNotify(Peer peer, MsgDeactiveWaitingObjectNotify msg)
    {
        UnityUtil.Print("Notify Wait", "DeActive Wait Game Start", "white");

        // ingame
        // 둘다 준비 끝낫다고 노티 이므로 
        // 게임 시작하자
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, msg.PlayerUId, msg.CurrentSp);
    }

    public void OnLeaveGameNotify(Peer peer, MsgLeaveGameNotify msg)
    {
        UnityUtil.Print("Notify Leave", msg.PlayerUId.ToString(), "white");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEAVE_GAME_NOTIFY, msg.PlayerUId);

        //if (InGameManager.Get() != null)
        //InGameManager.Get().OnOtherLeft(msg.PlayerUId);
    }

    public void OnSpawnNotify(Peer peer, MsgSpawnNotify msg)
    {
        UnityUtil.Print("spawn Notify ", msg.Wave.ToString(), "white");
        //
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.SPAWN_NOTIFY, msg.Wave);

    }

    public void OnAddSpNotify(Peer peer, MsgAddSpNotify msg)
    {
        UnityUtil.Print("Add Sp Notify", msg.PlayerUId.ToString() + "  " + msg.CurrentSp.ToString(), "white");
        //
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.ADD_SP_NOTIFY, msg.PlayerUId, msg.CurrentSp);
    }


    public void OnGetDiceNotify(Peer peer, MsgGetDiceNotify msg)
    {
        UnityUtil.Print("get dice Notify", msg.PlayerUId.ToString() + "  " + msg.DiceId.ToString(), "white");
        // other dice get
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.GET_DICE_NOTIFY, msg);

    }

    public void OnLevelUpDiceNotify(Peer peer, MsgLevelUpDiceNotify msg)
    {
        UnityUtil.Print("level up dice Notify", msg.PlayerUId.ToString() + "  " + msg.LevelupDiceId.ToString(), "white");

        // other dice level up
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEVEL_UP_DICE_NOTIFY, msg);
    }

    public void OnUpgradeSpNotify(Peer peer, MsgUpgradeSpNotify msg)
    {
        UnityUtil.Print("upgrade sp Notify", msg.PlayerUId.ToString() + "  " + msg.Upgrade.ToString(), "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.UPGRADE_SP_NOTIFY, msg);
    }

    public void OnInGameUpDiceNotify(Peer peer, MsgInGameUpDiceNotify msg)
    {
        UnityUtil.Print("ingame dice up Notify", msg.PlayerUId.ToString() + "  " + msg.DiceId.ToString(), "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.INGAME_UP_DICE_NOTIFY, msg);
    }

    
    
    public void OnEndGameNotify(Peer peer, MsgEndGameNotify msg)
    {
        UnityUtil.Print("end game Notify", msg.WinPlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.END_GAME_NOTIFY, msg);
    }


    

    #endregion
    
    
    
    
    #region relay

    public void OnHitDamageMinionRelay(Peer peer, MsgHitDamageMinionRelay msg)
    {
        UnityUtil.Print("RECV => ", "HIT_DAMAGE_MINION_RELAY " + msg.PlayerUId.ToString() + "  minion id " + msg.Id , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_MINION_RELAY, msg);
    }

    public void OnDestroyMinionRelay(Peer peer, MsgDestroyMinionRelay msg)
    {
        UnityUtil.Print("RECV => ", "DESTROY_MINION_RELAY  " +msg.PlayerUId.ToString() + "  minion id " + msg.Id , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.DESTROY_MINION_RELAY, msg);
    }

    public void OnHealMinionRelay(Peer peer, MsgHealMinionRelay msg)
    {
        UnityUtil.Print("RECV => ", "HEAL_MINION_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HEAL_MINION_RELAY, msg);
    }

    public void OnPushMinionRelay(Peer peer, MsgPushMinionRelay msg)
    {
        UnityUtil.Print("RECV => ", "PUSH_MINION_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.PUSH_MINION_RELAY, msg);
        
    }

    public void OnSetMinionAnimationTriggerRelay(Peer peer, MsgSetMinionAnimationTriggerRelay msg)
    {
        UnityUtil.Print("RECV => ", "SET_MINION_ANIMATION_TRIGGER_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, msg);
    }

    public void OnFireArrowRelay(Peer peer, MsgFireArrowRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_ARROW_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_ARROW_RELAY, msg);
    }

    public void OnFireballBombRelay(Peer peer, MsgFireballBombRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_BALL_BOMB_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_BALL_BOMB_RELAY, msg);
    }

    public void OnMineBombRelay(Peer peer, MsgMineBombRelay msg)
    {
        UnityUtil.Print("RECV => ", "MINE_BOMB_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.MINE_BOMB_RELAY, msg);
    }

    public void OnSetMagicTargetIdRelay(Peer peer, MsgSetMagicTargetIdRelay msg)
    {
        UnityUtil.Print("RECV => ", "SET_MAGIC_TARGET_ID_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SET_MAGIC_TARGET_ID_RELAY, msg);
    }

    public void OnSetMagicTargetRelay(Peer peer, MsgSetMagicTargetRelay msg)
    {
        UnityUtil.Print("RECV => ", "SET_MAGIC_TARGET_POS_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SET_MAGIC_TARGET_POS_RELAY, msg);
    }
    
    //
    public void OnSturnMinionRelay(Peer peer, MsgSturnMinionRelay msg)
    {
        UnityUtil.Print("RECV => ", "STURN_MINION_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.STURN_MINION_RELAY, msg);
    }

    public void OnRocketBombRelay(Peer peer, MsgRocketBombRelay msg)
    {
        UnityUtil.Print("RECV => ", "ROCKET_BOMB_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.ROCKET_BOMB_RELAY, msg);
    }

    public void OnIceBombRelay(Peer peer, MsgIceBombRelay msg)
    {
        UnityUtil.Print("RECV => ", "ICE_BOMB_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.ICE_BOMB_RELAY, msg);
    }

    public void OnDestroyMagicRelay(Peer peer, MsgDestroyMagicRelay msg)
    {
        UnityUtil.Print("RECV => ", "DESTROY_MAGIC_RELAY  " +msg.PlayerUId.ToString(), "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.DESTROY_MAGIC_RELAY, msg);
    }

    public void OnFireCannonBallRelay(Peer peer, MsgFireCannonBallRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_CANNON_BALL_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_CANNON_BALL_RELAY, msg);
    }

    public void OnFireSpearRelay(Peer peer, MsgFireSpearRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_SPEAR_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_SPEAR_RELAY, msg);
    }

    public void OnFireManFireRelay(Peer peer, MsgFireManFireRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_MAN_FIRE_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_MAN_FIRE_RELAY, msg);
    }

    public void OnActivatePoolObjectRelay(Peer peer, MsgActivatePoolObjectRelay msg)
    {
        UnityUtil.Print("RECV => ", "ACTIVATE_POOL_OBJECT_RELAY  " /*+msg.PlayerUId.ToString() */, "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, msg);
    }

    public void OnMinionCloackingRelay(Peer peer, MsgMinionCloackingRelay msg)
    {
        UnityUtil.Print("RECV => ", "MINION_CLOACKING_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.MINION_CLOACKING_RELAY, msg);
    }

    public void OnMinionFogOfWarRelay(Peer peer, MsgMinionFlagOfWarRelay msg)
    {
        UnityUtil.Print("RECV => ", "MINION_FOG_OF_WAR_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.MINION_FLAG_OF_WAR_RELAY, msg);
    }

    public void OnSendMessageVoidRelay(Peer peer, MsgSendMessageVoidRelay msg)
    {
        UnityUtil.Print("RECV => ", "SEND_MESSAGE_VOID_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SEND_MESSAGE_VOID_RELAY, msg);
    }

    public void OnSendMessageParam1Relay(Peer peer, MsgSendMessageParam1Relay msg)
    {
        UnityUtil.Print("RECV => ", "SEND_MESSAGE_PARAM1_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, msg);
    }

    public void OnNecromancerBulletRelay(Peer peer, MsgNecromancerBulletRelay msg)
    {
        UnityUtil.Print("RECV => ", "NECROMANCER_BULLET_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.NECROMANCER_BULLET_RELAY, msg);
    }

    public void OnSetMinionTargetRelay(Peer peer, MsgSetMinionTargetRelay msg)
    {
        UnityUtil.Print("RECV => ", "SET_MINION_TARGET_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SET_MINION_TARGET_RELAY, msg);
    }

    public void OnScarecrowRelay(Peer peer, MsgScarecrowRelay msg)
    {
        UnityUtil.Print("RECV => ", "SCARECROW_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SCARECROW_RELAY, msg);
    }

    public void OnLazerTargetRelay(Peer peer, MsgLayzerTargetRelay msg)
    {
        UnityUtil.Print("RECV => ", "LAYZER_TARGET_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.LAYZER_TARGET_RELAY, msg);
    }
    

    public void OnMinionStatusRelay(Peer peer, MsgMinionStatusRelay msg)
    {
        //UnityUtil.Print("minion status relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.MINION_STATUS_RELAY, msg);
    }


    public void OnFireBulletRelay(Peer peer, MsgFireBulletRelay msg)
    {
        UnityUtil.Print("RECV => ", "FIRE_BULLET_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_BULLET_RELAY, msg);
    }
    
    public void OnMinionInvincibilityRelay(Peer peer, MsgMinionInvincibilityRelay msg)
    {
        UnityUtil.Print("RECV => ", "INVINCIBILITY_RELAY  " +msg.PlayerUId.ToString() , "yellow");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.MINION_INVINCIBILITY_RELAY , msg);
    }
    
    
    #endregion
    

}
