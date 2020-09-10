#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
using RWCoreNetwork;
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

        //
        NetworkManager.Get().GetNetInfo().SetPlayerInfo(msg.PlayerInfo);
        NetworkManager.Get().IsMaster = msg.PlayerInfo.IsBottomPlayer;
        GameStateManager.Get().CheckSendInGame();

    }

    #endregion

    #region leave

    public void OnLeaveGameAck(IPeer peer, MsgLeaveGameAck msg)
    {
        UnityUtil.Print(" leave recv ", "errocode : " + msg.ErrorCode, "white");


        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEAVE_GAME_ACK);
    }

    #endregion

    #region ready game

    public void OnReadyGameAck(IPeer peer, MsgReadyGameAck msg)
    {
        UnityUtil.Print(" ready recv ", "errocode : " + msg.ErrorCode, "white");
    }

    #endregion


    #region dice

    public void OnGetDiceAck(IPeer peer, MsgGetDiceAck msg)
    {
        // my dice get
        UnityUtil.Print(" get dice recv ", "errocode : " + msg.ErrorCode, "white");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.GET_DICE_ACK, msg);

    }

    public void OnLevelUpDiceAck(IPeer peer, MsgLevelUpDiceAck msg)
    {
        UnityUtil.Print(" level up dice recv ", "errocode : " + msg.ErrorCode, "white");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEVEL_UP_DICE_ACK, msg);
    }

    public void OnUpgradeSpAck(IPeer peer, MsgUpgradeSpAck msg)
    {
        UnityUtil.Print(" up sp recv ", "errocode : " + msg.ErrorCode, "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.UPGRADE_SP_ACK, msg);
    }

    public void OnInGameUpDiceAck(IPeer peer, MsgInGameUpDiceAck msg)
    {
        UnityUtil.Print(" in game upgrade recv ", "errocode : " + msg.ErrorCode, "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.INGAME_UP_DICE_ACK, msg);
    }
    
    public void OnHitDamageAck(IPeer peer, MsgHitDamageAck msg)
    {
        UnityUtil.Print(" hit damage recv ", "errocode : " + msg.ErrorCode, "white");
        
        //Global.g_networkBaseValue
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_ACK, msg);
    }

    
    #endregion

    

    #region notify

    public void OnJoinGameNotify(IPeer peer, MsgJoinGameNotify msg)
    {
        UnityUtil.Print("other info ", msg.OtherPlayerInfo.Name + " , " + msg.OtherPlayerInfo.IsBottomPlayer, "white");

        // menu
        NetworkManager.Get().GetNetInfo().SetOtherInfo(msg.OtherPlayerInfo);
        GameStateManager.Get().CheckSendInGame();
    }

    public void OnDeactiveWaitingObjectNotify(IPeer peer, MsgDeactiveWaitingObjectNotify msg)
    {
        UnityUtil.Print("Notify Wait", "DeActive Wait Game Start", "white");

        // ingame
        // 둘다 준비 끝낫다고 노티 이므로 
        // 게임 시작하자
        if (InGameManager.Get() != null)
            InGameManager.Get()
                .RecvInGameManager(GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, msg.PlayerUId, msg.CurrentSp);
        // 
    }

    public void OnLeaveGameNotify(IPeer peer, MsgLeaveGameNotify msg)
    {
        UnityUtil.Print("Notify Leave", msg.PlayerUId.ToString(), "white");

        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEAVE_GAME_NOTIFY, msg.PlayerUId);

        //if (InGameManager.Get() != null)
        //InGameManager.Get().OnOtherLeft(msg.PlayerUId);
    }

    public void OnSpawnNotify(IPeer peer, MsgSpawnNotify msg)
    {
        UnityUtil.Print("spawn Notify ", msg.Wave.ToString(), "white");
        //
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.SPAWN_NOTIFY, msg.Wave);

    }

    public void OnAddSpNotify(IPeer peer, MsgAddSpNotify msg)
    {
        UnityUtil.Print("Add Sp Notify", msg.PlayerUId.ToString() + "  " + msg.CurrentSp.ToString(), "white");
        //
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.ADD_SP_NOTIFY, msg.PlayerUId, msg.CurrentSp);
    }


    public void OnGetDiceNotify(IPeer peer, MsgGetDiceNotify msg)
    {
        UnityUtil.Print("get dice Notify", msg.PlayerUId.ToString() + "  " + msg.DiceId.ToString(), "white");
        // other dice get
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.GET_DICE_NOTIFY, msg);

    }

    public void OnLevelUpDiceNotify(IPeer peer, MsgLevelUpDiceNotify msg)
    {
        UnityUtil.Print("level up dice Notify", msg.PlayerUId.ToString() + "  " + msg.LevelupDiceId.ToString(), "white");

        // other dice level up
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.LEVEL_UP_DICE_NOTIFY, msg);
    }

    public void OnUpgradeSpNotify(IPeer peer, MsgUpgradeSpNotify msg)
    {
        UnityUtil.Print("upgrade sp Notify", msg.PlayerUId.ToString() + "  " + msg.Upgrade.ToString(), "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.UPGRADE_SP_NOTIFY, msg);
    }

    public void OnInGameUpDiceNotify(IPeer peer, MsgInGameUpDiceNotify msg)
    {
        UnityUtil.Print("ingame dice up Notify", msg.PlayerUId.ToString() + "  " + msg.DiceId.ToString(), "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.INGAME_UP_DICE_NOTIFY, msg);
    }

    public void HitDamageNotify(IPeer peer, MsgHitDamageNotify msg)
    {
        UnityUtil.Print("hit damage Notify", msg.PlayerUId.ToString() + "  " + msg.Damage.ToString(), "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_NOTIFY, msg);
    }
    
    public void OnEndGameNotify(IPeer peer, MsgEndGameNotify msg)
    {
        UnityUtil.Print("end game Notify", msg.WinPlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvInGameManager(GameProtocol.END_GAME_NOTIFY, msg);
    }

    #endregion
    
    
    
    
    #region relay

    public void OnRemoveMinionRelay(IPeer peer, MsgRemoveMinionRelay msg)
    {
        UnityUtil.Print("remove mi relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.REMOVE_MINION_RELAY, msg);
    }

    public void OnHitDamageMinionRelay(IPeer peer, MsgHitDamageMinionRelay msg)
    {
        UnityUtil.Print("hit mi relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HIT_DAMAGE_MINION_RELAY, msg);
    }

    public void OnDestroyMinionRelay(IPeer peer, MsgDestroyMinionRelay msg)
    {
        UnityUtil.Print("dest mi relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.DESTROY_MINION_RELAY, msg);
    }

    public void OnHealMinionRelay(IPeer peer, MsgHealMinionRelay msg)
    {
        UnityUtil.Print("remove mi relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.HEAL_MINION_RELAY, msg);
    }

    public void OnPushMinionRelay(IPeer peer, MsgPushMinionRelay msg)
    {
        UnityUtil.Print("remove mi relay", msg.PlayerUId.ToString() , "white");
        
    }

    public void OnSetMinionAnimationTriggerRelay(IPeer peer, MsgSetMinionAnimationTriggerRelay msg)
    {
        UnityUtil.Print("ani trigger relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, msg);
    }

    public void OnFireArrowRelay(IPeer peer, MsgFireArrowRelay msg)
    {
        UnityUtil.Print("fire arrow relay", msg.PlayerUId.ToString() , "white");
        
        if (InGameManager.Get() != null)
            InGameManager.Get().RecvPlayerManager(GameProtocol.FIRE_ARROW_RELAY, msg);
    }

    public void OnRemoveMagicRelay(IPeer peer, MsgRemoveMagicRelay msg)
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
