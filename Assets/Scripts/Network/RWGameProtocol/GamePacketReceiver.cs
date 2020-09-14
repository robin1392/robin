using System;
using System.Collections.Generic;
using System.Text;
using RWCoreNetwork;
using RWCoreNetwork.NetPacket;
using RWGameProtocol.Msg;

namespace RWGameProtocol
{

    public class GamePacketReceiver : IPacketProcessor
    {
        #region Req/Ack delegate                

        public delegate void JoinGameReqDelegate(IPeer peer, MsgJoinGameReq msg);
        public JoinGameReqDelegate JoinGameReq;

        public delegate void JoinGameAckDelegate(IPeer peer, MsgJoinGameAck msg);
        public JoinGameAckDelegate JoinGameAck;

        public delegate void LeaveGameReqDelegate(IPeer peer, MsgLeaveGameReq msg);
        public LeaveGameReqDelegate LeaveGameReq;

        public delegate void LeaveGameAckDelegate(IPeer peer, MsgLeaveGameAck msg);
        public LeaveGameAckDelegate LeaveGameAck;

        public delegate void ReadyGameReqDelegate(IPeer peer, MsgReadyGameReq msg);
        public ReadyGameReqDelegate ReadyGameReq;

        public delegate void ReadyGameAckDelegate(IPeer peer, MsgReadyGameAck msg);
        public ReadyGameAckDelegate ReadyGameAck;

        public delegate void GetDiceReqDelegate(IPeer peer, MsgGetDiceReq msg);
        public GetDiceReqDelegate GetDiceReq;

        public delegate void GetDiceAckDelegate(IPeer peer, MsgGetDiceAck msg);
        public GetDiceAckDelegate GetDiceAck;

        public delegate void LevelUpDiceReqDelegate(IPeer peer, MsgLevelUpDiceReq msg);
        public LevelUpDiceReqDelegate LevelUpDiceReq;

        public delegate void LevelUpDiceAckDelegate(IPeer peer, MsgLevelUpDiceAck msg);
        public LevelUpDiceAckDelegate LevelUpDiceAck;

        public delegate void InGameUpDiceReqDelegate(IPeer peer, MsgInGameUpDiceReq msg);
        public InGameUpDiceReqDelegate InGameUpDiceReq;

        public delegate void InGameUpDiceAckDelegate(IPeer peer, MsgInGameUpDiceAck msg);
        public InGameUpDiceAckDelegate InGameUpDiceAck;

        public delegate void UpgradeSpReqDelegate(IPeer peer, MsgUpgradeSpReq msg);
        public UpgradeSpReqDelegate UpgradeSpReq;

        public delegate void UpgradeSpAckDelegate(IPeer peer, MsgUpgradeSpAck msg);
        public UpgradeSpAckDelegate UpgradeSpAck;

        public delegate void HitDamageReqDelegate(IPeer peer, MsgHitDamageReq msg);
        public HitDamageReqDelegate HitDamageReq;

        public delegate void HitDamageAckDelegate(IPeer peer, MsgHitDamageAck msg);
        public HitDamageAckDelegate HitDamageAck;

        #endregion


        #region Notify delegate                

        public delegate void JoinGameNotifyDelegate(IPeer peer, MsgJoinGameNotify msg);
        public JoinGameNotifyDelegate JoinGameNotify;

        public delegate void LeaveGameNotifyDelegate(IPeer peer, MsgLeaveGameNotify msg);
        public LeaveGameNotifyDelegate LeaveGameNotify;

        public delegate void GetDiceNotifyDelegate(IPeer peer, MsgGetDiceNotify msg);
        public GetDiceNotifyDelegate GetDiceNotify;

        public delegate void LevelUpDiceNotifyDelegate(IPeer peer, MsgLevelUpDiceNotify msg);
        public LevelUpDiceNotifyDelegate LevelUpDiceNotify;

        public delegate void InGameUpDiceNotifyDelegate(IPeer peer, MsgInGameUpDiceNotify msg);
        public InGameUpDiceNotifyDelegate InGameUpDiceNotify;

        public delegate void UpgradeSpNotifyDelegate(IPeer peer, MsgUpgradeSpNotify msg);
        public UpgradeSpNotifyDelegate UpgradeSpNotify;

        public delegate void DeactiveWaitingObjectNotifyDelegate(IPeer peer, MsgDeactiveWaitingObjectNotify msg);
        public DeactiveWaitingObjectNotifyDelegate DeactiveWaitingObjectNotify;

        public delegate void AddSpNotifyDelegate(IPeer peer, MsgAddSpNotify msg);
        public AddSpNotifyDelegate AddSpNotify;

        public delegate void SpawnNotifyDelegate(IPeer peer, MsgSpawnNotify msg);
        public SpawnNotifyDelegate SpawnNotify;

        public delegate void HitDamageNotifyDelegate(IPeer peer, MsgHitDamageNotify msg);
        public HitDamageNotifyDelegate HitDamageNotify;

        public delegate void EndGameNotifyDelegate(IPeer peer, MsgEndGameNotify msg);
        public EndGameNotifyDelegate EndGameNotify;

        #endregion


        #region Relay delegate                
        public delegate void RemoveMinionDelegate(IPeer peer, MsgRemoveMinionRelay msg);
        public RemoveMinionDelegate RemoveMinionRelay;

        public delegate void HitDamageMinionDelegate(IPeer peer, MsgHitDamageMinionRelay msg);
        public HitDamageMinionDelegate HitDamageMinionRelay;

        public delegate void DestroyMinionDelegate(IPeer peer, MsgDestroyMinionRelay msg);
        public DestroyMinionDelegate DestroyMinionRelay;

        public delegate void HealMinionDelegate(IPeer peer, MsgHealMinionRelay msg);
        public HealMinionDelegate HealMinionRelay;

        public delegate void PushMinionDelegate(IPeer peer, MsgPushMinionRelay msg);
        public PushMinionDelegate PushMinionRelay;

        public delegate void SetMinionAnimationTriggerDelegate(IPeer peer, MsgSetMinionAnimationTriggerRelay msg);
        public SetMinionAnimationTriggerDelegate SetMinionAnimationTriggerRelay;

        public delegate void RemoveMagicDelegate(IPeer peer, MsgRemoveMagicRelay msg);
        public RemoveMagicDelegate RemoveMagicRelay;

        public delegate void FireArrowDelegate(IPeer peer, MsgFireArrowRelay msg);
        public FireArrowDelegate FireArrowRelay;

        public delegate void FireballBombDelegate(IPeer peer, MsgFireballBombRelay msg);
        public FireballBombDelegate FireballBombRelay;

        public delegate void MineBombDelegate(IPeer peer, MsgMineBombRelay msg);
        public MineBombDelegate MineBombRelay;

        public delegate void SetMagicTargetIdDelegate(IPeer peer, MsgSetMagicTargetIdRelay msg);
        public SetMagicTargetIdDelegate SetMagicTargetIdRelay;
  
        public delegate void SetMagicTargetDelegate(IPeer peer, MsgSetMagicTargetRelay msg);
        public SetMagicTargetDelegate SetMagicTargetRelay;

        public delegate void SturnMinionRelayDelegate(IPeer peer, MsgSturnMinionRelay msg);
        public SturnMinionRelayDelegate SturnMinionRelay;

        public delegate void RocketBombRelayDelegate(IPeer peer, MsgRocketBombRelay msg);
        public RocketBombRelayDelegate RocketBombRelay;

        public delegate void IceBombRelayDelegate(IPeer peer, MsgIceBombRelay msg);
        public IceBombRelayDelegate IceBombRelay;

        public delegate void DestroyMagicRelayDelegate(IPeer peer, MsgDestroyMagicRelay msg);
        public DestroyMagicRelayDelegate DestroyMagicRelay;

        public delegate void FireCannonBallRelayDelegate(IPeer peer, MsgFireCannonBallRelay msg);
        public FireCannonBallRelayDelegate FireCannonBallRelay;

        public delegate void FireSpearRelayDelegate(IPeer peer, MsgFireSpearRelay msg);
        public FireSpearRelayDelegate FireSpearRelay;

        public delegate void FireManFireRelayDelegate(IPeer peer, MsgFireManFireRelay msg);
        public FireManFireRelayDelegate FireManFireRelay;

        public delegate void ActivatePoolObjectRelayDelegate(IPeer peer, MsgActivatePoolObjectRelay msg);
        public ActivatePoolObjectRelayDelegate ActivatePoolObjectRelay;

        public delegate void MinionCloackingRelayDelegate(IPeer peer, MsgMinionCloackingRelay msg);
        public MinionCloackingRelayDelegate MinionCloackingRelay;

        public delegate void MinionFogOfWarRelayDelegate(IPeer peer, MsgMinionFogOfWarRelay msg);
        public MinionFogOfWarRelayDelegate MinionFogOfWarRelay;

        public delegate void SendMessageVoidRelayDelegate(IPeer peer, MsgSendMessageVoidRelay msg);
        public SendMessageVoidRelayDelegate SendMessageVoidRelay;

        public delegate void SendMessageParam1RelayDelegate(IPeer peer, MsgSendMessageParam1Relay msg);
        public SendMessageParam1RelayDelegate SendMessageParam1Relay;

        public delegate void NecromancerBulletRelayDelegate(IPeer peer, MsgNecromancerBulletRelay msg);
        public NecromancerBulletRelayDelegate NecromancerBulletRelay;

        public delegate void SetMinionTargetRelayDelegate(IPeer peer, MsgSetMinionTargetRelay msg);
        public SetMinionTargetRelayDelegate SetMinionTargetRelay;

        public delegate void MinionStatusRelayDelegate(IPeer peer, MsgMinionStatusRelay msg);
        public MinionStatusRelayDelegate MinionStatusRelay;

        public delegate void ScarecrowRelayDelegate(IPeer peer, MsgScarecrowRelay msg);
        public ScarecrowRelayDelegate ScarecrowRelay;

        public delegate void LazerTargetRelayDelegate(IPeer peer, MsgLazerTargetRelay msg);
        public LazerTargetRelayDelegate LazerTargetRelay;

        #endregion


        public bool Run(IPeer peer, short protocolId, byte[] data)
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
                        if (LeaveGameReq == null)
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
                    #endregion

            }

            return true;
        }
    }
}
