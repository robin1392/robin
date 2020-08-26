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

        public delegate void SetDeckReqDelegate(IPeer peer, MsgSetDeckReq msg);
        public SetDeckReqDelegate SetDeckReq;

        public delegate void SetDeckAckDelegate(IPeer peer, MsgSetDeckAck msg);
        public SetDeckAckDelegate SetDeckAck;

        public delegate void GetDiceReqDelegate(IPeer peer, MsgGetDiceReq msg);
        public GetDiceReqDelegate GetDiceReq;

        public delegate void GetDiceAckDelegate(IPeer peer, MsgGetDiceAck msg);
        public GetDiceAckDelegate GetDiceAck;

        public delegate void LevelUpDiceReqDelegate(IPeer peer, MsgLevelUpDiceReq msg);
        public LevelUpDiceReqDelegate LevelUpDiceReq;

        public delegate void LevelUpDiceAckDelegate(IPeer peer, MsgLevelUpDiceAck msg);
        public LevelUpDiceAckDelegate LevelUpDiceAck;

        public delegate void HitDamageReqDelegate(IPeer peer, MsgHitDamageReq msg);
        public HitDamageReqDelegate HitDamageReq;

        public delegate void HitDamageAckDelegate(IPeer peer, MsgHitDamageAck msg);
        public HitDamageAckDelegate HitDamageAck;



        #region Notify delegate                

        public delegate void JoinGameNotifyDelegate(IPeer peer, MsgJoinGameNotify msg);
        public JoinGameNotifyDelegate JoinGameNotify;

        public delegate void LeaveGameNotifyDelegate(IPeer peer, MsgLeaveGameNotify msg);
        public LeaveGameNotifyDelegate LeaveGameNotify;

        public delegate void GetDiceNotifyDelegate(IPeer peer, MsgGetDiceNotify msg);
        public GetDiceNotifyDelegate GetDiceNotify;

        public delegate void DeactiveWaitingObjectNotifyDelegate(IPeer peer, MsgDeactiveWaitingObjectNotify msg);
        public DeactiveWaitingObjectNotifyDelegate DeactiveWaitingObjectNotify;

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

        #endregion


        public bool DoWork(IPeer peer, short protocolId, byte[] data)
        {
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.JOIN_GAME_REQ: JoinGameReq(peer, MsgJoinGameReq.Deserialize(data)); break;
                case GameProtocol.JOIN_GAME_ACK: JoinGameAck(peer, MsgJoinGameAck.Deserialize(data)); break;
                case GameProtocol.LEAVE_GAME_REQ: LeaveGameReq(peer, MsgLeaveGameReq.Deserialize(data)); break;
                case GameProtocol.LEAVE_GAME_ACK: LeaveGameAck(peer, MsgLeaveGameAck.Deserialize(data)); break;
                case GameProtocol.READY_GAME_REQ: ReadyGameReq(peer, MsgReadyGameReq.Deserialize(data)); break;
                case GameProtocol.READY_GAME_ACK: ReadyGameAck(peer, MsgReadyGameAck.Deserialize(data)); break;
                case GameProtocol.SET_DECK_REQ: SetDeckReq(peer, MsgSetDeckReq.Deserialize(data)); break;
                case GameProtocol.SET_DECK_ACK: SetDeckAck(peer, MsgSetDeckAck.Deserialize(data)); break;
                case GameProtocol.GET_DICE_REQ: GetDiceReq(peer, MsgGetDiceReq.Deserialize(data)); break;
                case GameProtocol.GET_DICE_ACK: GetDiceAck(peer, MsgGetDiceAck.Deserialize(data)); break;
                case GameProtocol.LEVEL_UP_DICE_REQ: LevelUpDiceReq(peer, MsgLevelUpDiceReq.Deserialize(data)); break;
                case GameProtocol.LEVEL_UP_DICE_ACK: LevelUpDiceAck(peer, MsgLevelUpDiceAck.Deserialize(data)); break;
                case GameProtocol.HIT_DAMAGE_REQ: HitDamageReq(peer, MsgHitDamageReq.Deserialize(data)); break;
                case GameProtocol.HIT_DAMAGE_ACK: HitDamageAck(peer, MsgHitDamageAck.Deserialize(data)); break;

                #region Notify Protocol                
                case GameProtocol.JOIN_GAME_NOTIFY: JoinGameNotify(peer, MsgJoinGameNotify.Deserialize(data)); break;
                case GameProtocol.LEAVE_GAME_NOTIFY: LeaveGameNotify(peer, MsgLeaveGameNotify.Deserialize(data)); break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY: DeactiveWaitingObjectNotify(peer, MsgDeactiveWaitingObjectNotify.Deserialize(data)); break;
                case GameProtocol.GET_DICE_NOTIFY: GetDiceNotify(peer, MsgGetDiceNotify.Deserialize(data)); break;

                #endregion

                #region Relay Protocol                
                case GameProtocol.REMOVE_MINION_RELAY: RemoveMinionRelay(peer, MsgRemoveMinionRelay.Deserialize(data)); break;
                case GameProtocol.HIT_DAMAGE_MINION_RELAY: HitDamageMinionRelay(peer, MsgHitDamageMinionRelay.Deserialize(data)); break;
                case GameProtocol.HEAL_MINION_RELAY: HealMinionRelay(peer, MsgHealMinionRelay.Deserialize(data)); break;
                case GameProtocol.PUSH_MINION_RELAY: PushMinionRelay(peer, MsgPushMinionRelay.Deserialize(data)); break;
                case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY: SetMinionAnimationTriggerRelay(peer, MsgSetMinionAnimationTriggerRelay.Deserialize(data)); break;
                case GameProtocol.FIRE_ARROW_RELAY: FireArrowRelay(peer, MsgFireArrowRelay.Deserialize(data)); break;
                case GameProtocol.FIREBALL_BOMB_RELAY: FireballBombRelay(peer, MsgFireballBombRelay.Deserialize(data)); break;
                case GameProtocol.MINE_BOMB_RELAY: MineBombRelay(peer, MsgMineBombRelay.Deserialize(data)); break;
                case GameProtocol.REMOVE_MAGIC_RELAY: RemoveMagicRelay(peer, MsgRemoveMagicRelay.Deserialize(data)); break;
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY: SetMagicTargetIdRelay(peer, MsgSetMagicTargetIdRelay.Deserialize(data)); break;
                case GameProtocol.SET_MAGIC_TARGET_POS_RELAY: SetMagicTargetRelay(peer, MsgSetMagicTargetRelay.Deserialize(data)); break;
                   
                #endregion

            }

            return true;
        }
    }
}
