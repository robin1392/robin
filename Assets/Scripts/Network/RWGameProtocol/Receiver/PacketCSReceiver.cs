using System;
using System.Collections.Generic;
using System.Text;
using RWCoreNetwork;
using RWCoreNetwork.NetPacket;
using RWGameProtocol.Msg;

namespace RWGameProtocol
{

    public class PacketCSReceiver : PacketReceiver, IPacketReceiver
    {
        public bool Process(Peer peer, short protocolId, byte[] data)
        {
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.JOIN_GAME_REQ:
                    {
                        if (JoinGameReq == null) 
                            return false;
                        JoinGameReq(peer, MsgJoinGameReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.JOIN_GAME_ACK:
                    {
                        if (JoinGameAck == null) 
                            return false;
                        JoinGameAck(peer, MsgJoinGameAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEAVE_GAME_REQ:
                    {
                        if (LeaveGameReq == null) 
                            return false;
                        LeaveGameReq(peer, MsgLeaveGameReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEAVE_GAME_ACK:
                    {
                        if (LeaveGameAck == null) 
                            return false;
                        LeaveGameAck(peer, MsgLeaveGameAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.READY_GAME_REQ:
                    {
                        if (ReadyGameReq == null) 
                            return false;
                        ReadyGameReq(peer, MsgReadyGameReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.READY_GAME_ACK:
                    {
                        if (ReadyGameAck == null) 
                            return false;
                        ReadyGameAck(peer, MsgReadyGameAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.GET_DICE_REQ:
                    {
                        if (GetDiceReq == null) 
                            return false;
                        GetDiceReq(peer, MsgGetDiceReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.GET_DICE_ACK:
                    {
                        if (GetDiceAck == null) 
                            return false;
                        GetDiceAck(peer, MsgGetDiceAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_REQ:
                    {
                        if (LevelUpDiceReq == null) 
                            return false;
                        LevelUpDiceReq(peer, MsgLevelUpDiceReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_ACK:
                    {
                        if (LevelUpDiceAck == null) 
                            return false;
                        LevelUpDiceAck(peer, MsgLevelUpDiceAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_REQ:
                    {
                        if (InGameUpDiceReq == null) 
                            return false;
                        InGameUpDiceReq(peer, MsgInGameUpDiceReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_ACK:
                    {
                        if (InGameUpDiceAck == null) 
                            return false;
                        InGameUpDiceAck(peer, MsgInGameUpDiceAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.UPGRADE_SP_REQ:
                    {
                        if (UpgradeSpReq == null) 
                            return false;
                        UpgradeSpReq(peer, MsgUpgradeSpReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.UPGRADE_SP_ACK:
                    {
                        if (UpgradeSpAck == null) 
                            return false;
                        UpgradeSpAck(peer, MsgUpgradeSpAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_REQ:
                    {
                        if (HitDamageReq == null) 
                            return false;
                        HitDamageReq(peer, MsgHitDamageReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_ACK:
                    {
                        if (HitDamageAck == null) 
                            return false;
                        HitDamageAck(peer, MsgHitDamageAck.Deserialize(data));
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_REQ:
                    {
                        if (ReconnectGameReq == null)
                            return false;
                        ReconnectGameReq(peer, MsgReconnectGameReq.Deserialize(data));
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_ACK:
                    {
                        if (ReconnectGameAck == null)
                            return false;
                        ReconnectGameAck(peer, MsgHitDamageAck.Deserialize(data));
                    }
                    break;

                #region Notify Protocol                
                case GameProtocol.JOIN_GAME_NOTIFY:
                    {
                        if (JoinGameNotify == null) 
                            return false;
                        JoinGameNotify(peer, MsgJoinGameNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    {
                        if (LeaveGameNotify == null) 
                            return false;
                        LeaveGameNotify(peer, MsgLeaveGameNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY:
                    {
                        if (DeactiveWaitingObjectNotify == null) 
                            return false;
                        DeactiveWaitingObjectNotify(peer, MsgDeactiveWaitingObjectNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.GET_DICE_NOTIFY:
                    {
                        if (GetDiceNotify == null) 
                            return false;
                        GetDiceNotify(peer, MsgGetDiceNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_NOTIFY:
                    {
                        if (LevelUpDiceNotify == null)
                            return false;
                        LevelUpDiceNotify(peer, MsgLevelUpDiceNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_NOTIFY:
                    {
                        if (InGameUpDiceNotify == null)
                            return false;
                        InGameUpDiceNotify(peer, MsgInGameUpDiceNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.UPGRADE_SP_NOTIFY:
                    {
                        if (UpgradeSpNotify == null)
                            return false;
                        UpgradeSpNotify(peer, MsgUpgradeSpNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.ADD_SP_NOTIFY:
                    {
                        if (AddSpNotify == null)
                            return false;
                        AddSpNotify(peer, MsgAddSpNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.SPAWN_NOTIFY:
                    {
                        if (SpawnNotify == null)
                            return false;
                        SpawnNotify(peer, MsgSpawnNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_NOTIFY:
                    {
                        if (HitDamageNotify == null)
                            return false;
                        HitDamageNotify(peer, MsgHitDamageNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.END_GAME_NOTIFY:
                    {
                        if (EndGameNotify == null)
                            return false;
                        EndGameNotify(peer, MsgEndGameNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.DISCONNECT_GAME_NOTIFY:
                    {
                        if (DisconnectGameNotify == null)
                            return false;
                        DisconnectGameNotify(peer, MsgDisconnectGameNotify.Deserialize(data));
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_NOTIFY:
                    {
                        if (ReconnectGameNotify == null)
                            return false;
                        ReconnectGameNotify(peer, MsgReconnectGameNotify.Deserialize(data));
                    }
                    break;

                #endregion


                #region Relay Protocol                
                case GameProtocol.REMOVE_MINION_RELAY:
                    {
                        if (RemoveMinionRelay == null)
                            return false;
                        RemoveMinionRelay(peer, MsgRemoveMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                    {
                        if (HitDamageMinionRelay == null)
                            return false;
                        HitDamageMinionRelay(peer, MsgHitDamageMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.DESTROY_MINION_RELAY:
                    {
                        if (DestroyMinionRelay == null)
                            return false;
                        DestroyMinionRelay(peer, MsgDestroyMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.HEAL_MINION_RELAY:
                    {
                        if (HealMinionRelay == null)
                            return false;
                        HealMinionRelay(peer, MsgHealMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.PUSH_MINION_RELAY:
                    {
                        if (PushMinionRelay == null)
                            return false;
                        PushMinionRelay(peer, MsgPushMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                    {
                        if (SetMinionAnimationTriggerRelay == null)
                            return false;
                        SetMinionAnimationTriggerRelay(peer, MsgSetMinionAnimationTriggerRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_ARROW_RELAY:
                    {
                        if (FireArrowRelay == null)
                            return false;
                        FireArrowRelay(peer, MsgFireArrowRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_BALL_BOMB_RELAY:
                    {
                        if (FireballBombRelay == null)
                            return false;
                        FireballBombRelay(peer, MsgFireballBombRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.MINE_BOMB_RELAY:
                    {
                        if (MineBombRelay == null)
                            return false;
                        MineBombRelay(peer, MsgMineBombRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.REMOVE_MAGIC_RELAY:
                    {
                        if (RemoveMagicRelay == null)
                            return false;
                        RemoveMagicRelay(peer, MsgRemoveMagicRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                    {
                        if (SetMagicTargetIdRelay == null)
                            return false;
                        SetMagicTargetIdRelay(peer, MsgSetMagicTargetIdRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                    {
                        if (SetMagicTargetRelay == null)
                            return false;
                        SetMagicTargetRelay(peer, MsgSetMagicTargetRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.STURN_MINION_RELAY:
                    {
                        if (SturnMinionRelay == null)
                            return false;
                        SturnMinionRelay(peer, MsgSturnMinionRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.ROCKET_BOMB_RELAY:
                    {
                        if (RocketBombRelay == null)
                            return false;
                        RocketBombRelay(peer, MsgRocketBombRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.ICE_BOMB_RELAY:
                    {
                        if (IceBombRelay == null)
                            return false;
                        IceBombRelay(peer, MsgIceBombRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.DESTROY_MAGIC_RELAY:
                    {
                        if (DestroyMagicRelay == null)
                            return false;
                        DestroyMagicRelay(peer, MsgDestroyMagicRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_CANNON_BALL_RELAY:
                    {
                        if (FireCannonBallRelay == null)
                            return false;
                        FireCannonBallRelay(peer, MsgFireCannonBallRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_SPEAR_RELAY:
                    {
                        if (FireSpearRelay == null)
                            return false;
                        FireSpearRelay(peer, MsgFireSpearRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_MAN_FIRE_RELAY:
                    {
                        if (FireManFireRelay == null)
                            return false;
                        FireManFireRelay(peer, MsgFireManFireRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                    {
                        if (ActivatePoolObjectRelay == null)
                            return false;
                        ActivatePoolObjectRelay(peer, MsgActivatePoolObjectRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.MINION_CLOACKING_RELAY:
                    {
                        if (MinionCloackingRelay == null)
                            return false;
                        MinionCloackingRelay(peer, MsgMinionCloackingRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.MINION_FOG_OF_WAR_RELAY:
                    {
                        if (MinionFogOfWarRelay == null)
                            return false;
                        MinionFogOfWarRelay(peer, MsgMinionFogOfWarRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                    {
                        if (SendMessageVoidRelay == null)
                            return false;
                        SendMessageVoidRelay(peer, MsgSendMessageVoidRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                    {
                        if (SendMessageParam1Relay == null)
                            return false;
                        SendMessageParam1Relay(peer, MsgSendMessageParam1Relay.Deserialize(data));
                    }
                    break;
                case GameProtocol.NECROMANCER_BULLET_RELAY:
                    {
                        if (NecromancerBulletRelay == null)
                            return false;
                        NecromancerBulletRelay(peer, MsgNecromancerBulletRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SET_MINION_TARGET_RELAY:
                    {
                        if (SetMinionTargetRelay == null)
                            return false;
                        SetMinionTargetRelay(peer, MsgSetMinionTargetRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.MINION_STATUS_RELAY:
                    {
                        if (MinionStatusRelay == null)
                            return false;
                        MinionStatusRelay(peer, MsgMinionStatusRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.SCARECROW_RELAY:
                    {
                        if (ScarecrowRelay == null)
                            return false;
                        ScarecrowRelay(peer, MsgScarecrowRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.LAYZER_TARGET_RELAY:
                    {
                        if (LazerTargetRelay == null)
                            return false;
                        LazerTargetRelay(peer, MsgLazerTargetRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.FIRE_BULLET_RELAY:
                    {
                        if (FireBulletRelay == null)
                            return false;
                        FireBulletRelay(peer, MsgFireBulletRelay.Deserialize(data));
                    }
                    break;
                case GameProtocol.MINION_INVINCIBILITY_RELAY:
                    {
                        if (MinionInvincibilityRelay == null)
                            return false;
                        MinionInvincibilityRelay(peer, MsgMinionInvincibilityRelay.Deserialize(data));
                    }
                    break;
                    #endregion

            }

            return true;
        }
    }
}
