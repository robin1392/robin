using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCoreNetwork.NetService;
using RWGameProtocol.Msg;

namespace RWGameProtocol.Serializer
{
    public class PacketReceiver
    {
        #region Req/Ack delegate                

        public delegate void JoinGameReqDelegate(Peer peer, MsgJoinGameReq msg);
        public JoinGameReqDelegate JoinGameReq;

        public delegate void JoinGameAckDelegate(Peer peer, MsgJoinGameAck msg);
        public JoinGameAckDelegate JoinGameAck;

        public delegate void LeaveGameReqDelegate(Peer peer, MsgLeaveGameReq msg);
        public LeaveGameReqDelegate LeaveGameReq;

        public delegate void LeaveGameAckDelegate(Peer peer, MsgLeaveGameAck msg);
        public LeaveGameAckDelegate LeaveGameAck;

        public delegate void ReadyGameReqDelegate(Peer peer, MsgReadyGameReq msg);
        public ReadyGameReqDelegate ReadyGameReq;

        public delegate void ReadyGameAckDelegate(Peer peer, MsgReadyGameAck msg);
        public ReadyGameAckDelegate ReadyGameAck;

        public delegate void GetDiceReqDelegate(Peer peer, MsgGetDiceReq msg);
        public GetDiceReqDelegate GetDiceReq;

        public delegate void GetDiceAckDelegate(Peer peer, MsgGetDiceAck msg);
        public GetDiceAckDelegate GetDiceAck;

        public delegate void LevelUpDiceReqDelegate(Peer peer, MsgLevelUpDiceReq msg);
        public LevelUpDiceReqDelegate LevelUpDiceReq;

        public delegate void LevelUpDiceAckDelegate(Peer peer, MsgLevelUpDiceAck msg);
        public LevelUpDiceAckDelegate LevelUpDiceAck;

        public delegate void InGameUpDiceReqDelegate(Peer peer, MsgInGameUpDiceReq msg);
        public InGameUpDiceReqDelegate InGameUpDiceReq;

        public delegate void InGameUpDiceAckDelegate(Peer peer, MsgInGameUpDiceAck msg);
        public InGameUpDiceAckDelegate InGameUpDiceAck;

        public delegate void UpgradeSpReqDelegate(Peer peer, MsgUpgradeSpReq msg);
        public UpgradeSpReqDelegate UpgradeSpReq;

        public delegate void UpgradeSpAckDelegate(Peer peer, MsgUpgradeSpAck msg);
        public UpgradeSpAckDelegate UpgradeSpAck;

        public delegate void HitDamageReqDelegate(Peer peer, MsgHitDamageReq msg);
        public HitDamageReqDelegate HitDamageReq;
        public delegate void HitDamageAckDelegate(Peer peer, MsgHitDamageAck msg);
        public HitDamageAckDelegate HitDamageAck;

        public delegate void ReconnectGameReqDelegate(Peer peer, MsgReconnectGameReq msg);
        public ReconnectGameReqDelegate ReconnectGameReq;
        public delegate void ReconnectGameAckDelegate(Peer peer, MsgReconnectGameAck msg);
        public ReconnectGameAckDelegate ReconnectGameAck;
        public delegate void ReconnectGameNotifyDelegate(Peer peer, MsgReconnectGameNotify msg);
        public ReconnectGameNotifyDelegate ReconnectGameNotify;

        public delegate void ReadySyncGameReqDelegate(Peer peer, MsgReadySyncGameReq msg);
        public ReadySyncGameReqDelegate ReadySyncGameReq;
        public delegate void ReadySyncGameAckDelegate(Peer peer, MsgReadySyncGameAck msg);
        public ReadySyncGameAckDelegate ReadySyncGameAck;
        public delegate void ReadySyncGameNotifyDelegate(Peer peer, MsgReadySyncGameNotify msg);
        public ReadySyncGameNotifyDelegate ReadySyncGameNotify;

        public delegate void StartSyncGameReqDelegate(Peer peer, MsgStartSyncGameReq msg);
        public StartSyncGameReqDelegate StartSyncGameReq;
        public delegate void StartSyncGameAckDelegate(Peer peer, MsgStartSyncGameAck msg);
        public StartSyncGameAckDelegate StartSyncGameAck;
        public delegate void StartSyncGameNotifyDelegate(Peer peer, MsgStartSyncGameNotify msg);
        public StartSyncGameNotifyDelegate StartSyncGameNotify;

        public delegate void EndSyncGameReqDelegate(Peer peer, MsgEndSyncGameReq msg);
        public EndSyncGameReqDelegate EndSyncGameReq;
        public delegate void EndSyncGameAckDelegate(Peer peer, MsgEndSyncGameAck msg);
        public EndSyncGameAckDelegate EndSyncGameAck;
        public delegate void EndSyncGameNotifyDelegate(Peer peer, MsgEndSyncGameNotify msg);
        public EndSyncGameNotifyDelegate EndSyncGameNotify;
        #endregion


        #region Notify delegate                

        public delegate void JoinGameNotifyDelegate(Peer peer, MsgJoinGameNotify msg);
        public JoinGameNotifyDelegate JoinGameNotify;

        public delegate void LeaveGameNotifyDelegate(Peer peer, MsgLeaveGameNotify msg);
        public LeaveGameNotifyDelegate LeaveGameNotify;

        public delegate void GetDiceNotifyDelegate(Peer peer, MsgGetDiceNotify msg);
        public GetDiceNotifyDelegate GetDiceNotify;

        public delegate void LevelUpDiceNotifyDelegate(Peer peer, MsgLevelUpDiceNotify msg);
        public LevelUpDiceNotifyDelegate LevelUpDiceNotify;

        public delegate void InGameUpDiceNotifyDelegate(Peer peer, MsgInGameUpDiceNotify msg);
        public InGameUpDiceNotifyDelegate InGameUpDiceNotify;

        public delegate void UpgradeSpNotifyDelegate(Peer peer, MsgUpgradeSpNotify msg);
        public UpgradeSpNotifyDelegate UpgradeSpNotify;

        public delegate void DeactiveWaitingObjectNotifyDelegate(Peer peer, MsgDeactiveWaitingObjectNotify msg);
        public DeactiveWaitingObjectNotifyDelegate DeactiveWaitingObjectNotify;

        public delegate void AddSpNotifyDelegate(Peer peer, MsgAddSpNotify msg);
        public AddSpNotifyDelegate AddSpNotify;

        public delegate void SpawnNotifyDelegate(Peer peer, MsgSpawnNotify msg);
        public SpawnNotifyDelegate SpawnNotify;

        public delegate void HitDamageNotifyDelegate(Peer peer, MsgHitDamageNotify msg);
        public HitDamageNotifyDelegate HitDamageNotify;

        public delegate void EndGameNotifyDelegate(Peer peer, MsgEndGameNotify msg);
        public EndGameNotifyDelegate EndGameNotify;

        public delegate void DisconnectGameNotifyDelegate(Peer peer, MsgDisconnectGameNotify msg);
        public DisconnectGameNotifyDelegate DisconnectGameNotify;

        public delegate void PauseGameNotifyDelegate(Peer peer, MsgPauseGameNotify msg);
        public PauseGameNotifyDelegate PauseGameNotify;

        public delegate void ResumeGameNotifyDelegate(Peer peer, MsgResumeGameNotify msg);
        public ResumeGameNotifyDelegate ResumeGameNotify;

        #endregion


        #region Relay delegate
        public delegate void RemoveMinionDelegate(Peer peer, MsgRemoveMinionRelay msg);
        public RemoveMinionDelegate RemoveMinionRelay;

        public delegate void HitDamageMinionDelegate(Peer peer, MsgHitDamageMinionRelay msg);
        public HitDamageMinionDelegate HitDamageMinionRelay;

        public delegate void DestroyMinionDelegate(Peer peer, MsgDestroyMinionRelay msg);
        public DestroyMinionDelegate DestroyMinionRelay;

        public delegate void HealMinionDelegate(Peer peer, MsgHealMinionRelay msg);
        public HealMinionDelegate HealMinionRelay;

        public delegate void PushMinionDelegate(Peer peer, MsgPushMinionRelay msg);
        public PushMinionDelegate PushMinionRelay;

        public delegate void SetMinionAnimationTriggerDelegate(Peer peer, MsgSetMinionAnimationTriggerRelay msg);
        public SetMinionAnimationTriggerDelegate SetMinionAnimationTriggerRelay;

        public delegate void RemoveMagicDelegate(Peer peer, MsgRemoveMagicRelay msg);
        public RemoveMagicDelegate RemoveMagicRelay;

        public delegate void FireArrowDelegate(Peer peer, MsgFireArrowRelay msg);
        public FireArrowDelegate FireArrowRelay;

        public delegate void FireballBombDelegate(Peer peer, MsgFireballBombRelay msg);
        public FireballBombDelegate FireballBombRelay;

        public delegate void MineBombDelegate(Peer peer, MsgMineBombRelay msg);
        public MineBombDelegate MineBombRelay;

        public delegate void SetMagicTargetIdDelegate(Peer peer, MsgSetMagicTargetIdRelay msg);
        public SetMagicTargetIdDelegate SetMagicTargetIdRelay;

        public delegate void SetMagicTargetDelegate(Peer peer, MsgSetMagicTargetRelay msg);
        public SetMagicTargetDelegate SetMagicTargetRelay;

        public delegate void SturnMinionRelayDelegate(Peer peer, MsgSturnMinionRelay msg);
        public SturnMinionRelayDelegate SturnMinionRelay;

        public delegate void RocketBombRelayDelegate(Peer peer, MsgRocketBombRelay msg);
        public RocketBombRelayDelegate RocketBombRelay;

        public delegate void IceBombRelayDelegate(Peer peer, MsgIceBombRelay msg);
        public IceBombRelayDelegate IceBombRelay;

        public delegate void DestroyMagicRelayDelegate(Peer peer, MsgDestroyMagicRelay msg);
        public DestroyMagicRelayDelegate DestroyMagicRelay;

        public delegate void FireCannonBallRelayDelegate(Peer peer, MsgFireCannonBallRelay msg);
        public FireCannonBallRelayDelegate FireCannonBallRelay;

        public delegate void FireSpearRelayDelegate(Peer peer, MsgFireSpearRelay msg);
        public FireSpearRelayDelegate FireSpearRelay;

        public delegate void FireManFireRelayDelegate(Peer peer, MsgFireManFireRelay msg);
        public FireManFireRelayDelegate FireManFireRelay;

        public delegate void ActivatePoolObjectRelayDelegate(Peer peer, MsgActivatePoolObjectRelay msg);
        public ActivatePoolObjectRelayDelegate ActivatePoolObjectRelay;

        public delegate void MinionCloackingRelayDelegate(Peer peer, MsgMinionCloackingRelay msg);
        public MinionCloackingRelayDelegate MinionCloackingRelay;

        public delegate void MinionFogOfWarRelayDelegate(Peer peer, MsgMinionFogOfWarRelay msg);
        public MinionFogOfWarRelayDelegate MinionFogOfWarRelay;

        public delegate void SendMessageVoidRelayDelegate(Peer peer, MsgSendMessageVoidRelay msg);
        public SendMessageVoidRelayDelegate SendMessageVoidRelay;

        public delegate void SendMessageParam1RelayDelegate(Peer peer, MsgSendMessageParam1Relay msg);
        public SendMessageParam1RelayDelegate SendMessageParam1Relay;

        public delegate void NecromancerBulletRelayDelegate(Peer peer, MsgNecromancerBulletRelay msg);
        public NecromancerBulletRelayDelegate NecromancerBulletRelay;

        public delegate void SetMinionTargetRelayDelegate(Peer peer, MsgSetMinionTargetRelay msg);
        public SetMinionTargetRelayDelegate SetMinionTargetRelay;

        public delegate void MinionStatusRelayDelegate(Peer peer, MsgMinionStatusRelay msg);
        public MinionStatusRelayDelegate MinionStatusRelay;

        public delegate void ScarecrowRelayDelegate(Peer peer, MsgScarecrowRelay msg);
        public ScarecrowRelayDelegate ScarecrowRelay;

        public delegate void LazerTargetRelayDelegate(Peer peer, MsgLazerTargetRelay msg);
        public LazerTargetRelayDelegate LazerTargetRelay;

        public delegate void FireBulletRelayDelegate(Peer peer, MsgFireBulletRelay msg);
        public FireBulletRelayDelegate FireBulletRelay;

        public delegate void MinionInvincibilityRelayDelegate(Peer peer, MsgMinionInvincibilityRelay msg);
        public MinionInvincibilityRelayDelegate MinionInvincibilityRelay;

        #endregion

    }
}
