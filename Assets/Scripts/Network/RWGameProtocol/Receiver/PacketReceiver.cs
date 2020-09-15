using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCoreNetwork;
using RWGameProtocol.Msg;

namespace RWGameProtocol
{
    public class PacketReceiver
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

        public delegate void ReconnectGameReqDelegate(IPeer peer, MsgReconnectGameReq msg);
        public ReconnectGameReqDelegate ReconnectGameReq;

        public delegate void ReconnectGameAckDelegate(IPeer peer, MsgHitDamageAck msg);
        public ReconnectGameAckDelegate ReconnectGameAck;

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

        public delegate void DisconnectGameNotifyDelegate(IPeer peer, MsgDisconnectGameNotify msg);
        public DisconnectGameNotifyDelegate DisconnectGameNotify;

        public delegate void ReconnectGameNotifyDelegate(IPeer peer, MsgReconnectGameNotify msg);
        public ReconnectGameNotifyDelegate ReconnectGameNotify;

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

    }
}
