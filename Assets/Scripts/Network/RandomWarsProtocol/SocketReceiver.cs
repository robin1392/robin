using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using RandomWarsService.Network.Socket.NetService;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsProtocol.Msg;


namespace RandomWarsProtocol
{
    class CustomizedBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Define the new type to bind to
            Type typeToDeserialize = null;
            // Get the current assembly
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;
            // Create the new type and return it
            typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, currentAssembly));
            return typeToDeserialize;
        }
    }



    public class SocketReceiver : IPacketReceiver
    {
        #region Req/Ack delegate                

        public delegate void JoinGameReqDelegate(Peer peer, MsgJoinGameReq msg);
        public JoinGameReqDelegate JoinGameReq;
        public delegate void JoinGameAckDelegate(Peer peer, MsgJoinGameAck msg);
        public JoinGameAckDelegate JoinGameAck;

        public delegate void JoinCoopGameReqDelegate(Peer peer, MsgJoinCoopGameReq msg);
        public JoinCoopGameReqDelegate JoinCoopGameReq;
        public delegate void JoinCoopGameAckDelegate(Peer peer, MsgJoinCoopGameAck msg);
        public JoinCoopGameAckDelegate JoinCoopGameAck;

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

        public delegate void MergeDiceReqDelegate(Peer peer, MsgMergeDiceReq msg);
        public MergeDiceReqDelegate MergeDiceReq;
        public delegate void MergeDiceAckDelegate(Peer peer, MsgMergeDiceAck msg);
        public MergeDiceAckDelegate MergeDiceAck;

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

        public delegate void JoinCoopGameNotifyDelegate(Peer peer, MsgJoinCoopGameNotify msg);
        public JoinCoopGameNotifyDelegate JoinCoopGameNotify;

        public delegate void LeaveGameNotifyDelegate(Peer peer, MsgLeaveGameNotify msg);
        public LeaveGameNotifyDelegate LeaveGameNotify;

        public delegate void GetDiceNotifyDelegate(Peer peer, MsgGetDiceNotify msg);
        public GetDiceNotifyDelegate GetDiceNotify;

        public delegate void MergeDiceNotifyDelegate(Peer peer, MsgMergeDiceNotify msg);
        public MergeDiceNotifyDelegate MergeDiceNotify;

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

        public delegate void CoopSpawnNotifyDelegate(Peer peer, MsgCoopSpawnNotify msg);
        public CoopSpawnNotifyDelegate CoopSpawnNotify;

        public delegate void MonsterSpawnNotifyDelegate(Peer peer, MsgMonsterSpawnNotify msg);
        public MonsterSpawnNotifyDelegate MonsterSpawnNotify;

        public delegate void HitDamageNotifyDelegate(Peer peer, MsgHitDamageNotify msg);
        public HitDamageNotifyDelegate HitDamageNotify;

        public delegate void EndGameNotifyDelegate(Peer peer, MsgEndGameNotify msg);
        public EndGameNotifyDelegate EndGameNotify;

        public delegate void EndCoopGameNotifyDelegate(Peer peer, MsgEndCoopGameNotify msg);
        public EndCoopGameNotifyDelegate EndCoopGameNotify;


        public delegate void DisconnectGameNotifyDelegate(Peer peer, MsgDisconnectGameNotify msg);
        public DisconnectGameNotifyDelegate DisconnectGameNotify;

        public delegate void PauseGameNotifyDelegate(Peer peer, MsgPauseGameNotify msg);
        public PauseGameNotifyDelegate PauseGameNotify;

        public delegate void ResumeGameNotifyDelegate(Peer peer, MsgResumeGameNotify msg);
        public ResumeGameNotifyDelegate ResumeGameNotify;

        #endregion


        #region Relay delegate
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

        public delegate void MinionFlagOfWarRelayDelegate(Peer peer, MsgMinionFlagOfWarRelay msg);
        public MinionFlagOfWarRelayDelegate MinionFlagOfWarRelay;

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

        public delegate void LayzerTargetRelayDelegate(Peer peer, MsgLayzerTargetRelay msg);
        public LayzerTargetRelayDelegate LayzerTargetRelay;

        public delegate void FireBulletRelayDelegate(Peer peer, MsgFireBulletRelay msg);
        public FireBulletRelayDelegate FireBulletRelay;

        public delegate void MinionInvincibilityRelayDelegate(Peer peer, MsgMinionInvincibilityRelay msg);
        public MinionInvincibilityRelayDelegate MinionInvincibilityRelay;

        #endregion


        public bool Process(Peer peer, int protocolId, byte[] buffer)
        {
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.JOIN_GAME_REQ:
                    {
                        if (JoinGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinGameReq msg = new MsgJoinGameReq();
                            msg.DeckIndex = br.ReadSByte();
                            JoinGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.JOIN_GAME_ACK:
                    {
                        if (JoinGameAck == null)
                            return false;

                        //
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinGameAck msg = new MsgJoinGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.PlayerInfo = MsgPlayerInfo.Read(br);
                            JoinGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.JOIN_GAME_NOTIFY:
                    {
                        if (JoinGameNotify == null)
                            return false;

                        //
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinGameNotify msg = new MsgJoinGameNotify();
                            msg.OtherPlayerInfo = MsgPlayerInfo.Read(br);
                            JoinGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.JOIN_COOP_GAME_REQ:
                    {
                        if (JoinCoopGameReq == null)
                            return false;


                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinCoopGameReq msg = new MsgJoinCoopGameReq();
                            msg.DeckIndex = br.ReadSByte();
                            JoinCoopGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.JOIN_COOP_GAME_ACK:
                    {
                        if (JoinGameAck == null)
                            return false;

                        //
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinCoopGameAck msg = new MsgJoinCoopGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.PlayerInfo = MsgPlayerInfo.Read(br);
                            JoinCoopGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.JOIN_COOP_GAME_NOTIFY:
                    {
                        if (JoinCoopGameNotify == null)
                            return false;

                        //
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgJoinCoopGameNotify msg = new MsgJoinCoopGameNotify();
                            msg.CoopPlayerInfo = MsgPlayerInfo.Read(br);

                            int length = br.ReadInt32();
                            msg.OtherPlayerInfo = new MsgPlayerInfo[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.OtherPlayerInfo[i] = MsgPlayerInfo.Read(br);
                            }

                            JoinCoopGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEAVE_GAME_REQ:
                    {
                        if (LeaveGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLeaveGameReq msg = new MsgLeaveGameReq();
                            LeaveGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEAVE_GAME_ACK:
                    {
                        if (LeaveGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLeaveGameAck msg = new MsgLeaveGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            LeaveGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEAVE_GAME_NOTIFY:
                    {
                        if (LeaveGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLeaveGameNotify msg = new MsgLeaveGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            LeaveGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.READY_GAME_REQ:
                    {
                        if (ReadyGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReadyGameReq msg = new MsgReadyGameReq();
                            ReadyGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.READY_GAME_ACK:
                    {
                        if (ReadyGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReadyGameAck msg = new MsgReadyGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            ReadyGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY:
                    {
                        if (DeactiveWaitingObjectNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgDeactiveWaitingObjectNotify msg = new MsgDeactiveWaitingObjectNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.CurrentSp = br.ReadInt32();
                            DeactiveWaitingObjectNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.ADD_SP_NOTIFY:
                    {
                        if (AddSpNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgAddSpNotify msg = new MsgAddSpNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.CurrentSp = br.ReadInt32();
                            AddSpNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SPAWN_NOTIFY:
                    {
                        if (SpawnNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSpawnNotify msg = new MsgSpawnNotify();
                            msg.Wave = br.ReadInt32();

                            int length = br.ReadInt32();
                            msg.SpawnInfo = new MsgSpawnInfo[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.SpawnInfo[i] = MsgSpawnInfo.Read(br);
                            }

                            SpawnNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.COOP_SPAWN_NOTIFY:
                    {
                        if (CoopSpawnNotify == null)
                            return false;


                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgCoopSpawnNotify msg = new MsgCoopSpawnNotify();
                            msg.Wave = br.ReadInt32();
                            msg.PlayerUId = br.ReadUInt16();

                            int length = br.ReadInt32();
                            msg.SpawnInfo = new MsgSpawnInfo[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.SpawnInfo[i] = MsgSpawnInfo.Read(br);
                            }

                            CoopSpawnNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MONSTER_SPAWN_NOTIFY:
                    {
                        if (MonsterSpawnNotify == null)
                            return false;


                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMonsterSpawnNotify msg = new MsgMonsterSpawnNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.SpawnMonster = MsgMonster.Read(br);
                            MonsterSpawnNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.END_GAME_NOTIFY:
                    {
                        if (EndGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgEndGameNotify msg = new MsgEndGameNotify();
                            msg.ErrorCode = br.ReadInt32();
                            msg.GameResult = (GAME_RESULT)br.ReadByte();
                            msg.WinningStreak = br.ReadByte();

                            int length = br.ReadInt32();
                            msg.NormalReward = new MsgReward[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.NormalReward[i] = MsgReward.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.StreakReward = new MsgReward[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.StreakReward[i] = MsgReward.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.PerfectReward = new MsgReward[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.PerfectReward[i] = MsgReward.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.QuestData = new MsgQuestData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.QuestData[i] = MsgQuestData.Read(br);
                            }

                            EndGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.END_COOP_GAME_NOTIFY:
                    {
                        if (EndCoopGameNotify == null)
                            return false;


                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgEndCoopGameNotify msg = new MsgEndCoopGameNotify();
                            msg.ErrorCode = br.ReadInt32();
                            msg.GameResult = (GAME_RESULT)br.ReadByte();

                            int length = br.ReadInt32();
                            msg.NormalReward = new MsgReward[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.NormalReward[i] = MsgReward.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.QuestData = new MsgQuestData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.QuestData[i] = MsgQuestData.Read(br);
                            }

                            EndCoopGameNotify(peer, msg);
                        }
                    }
                    break;

                case GameProtocol.DISCONNECT_GAME_NOTIFY:
                    {
                        if (DisconnectGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgDisconnectGameNotify msg = new MsgDisconnectGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            DisconnectGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.PAUSE_GAME_NOTIFY:
                    {
                        if (PauseGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgPauseGameNotify msg = new MsgPauseGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            PauseGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.RESUME_GAME_NOTIFY:
                    {
                        if (ResumeGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgResumeGameNotify msg = new MsgResumeGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            ResumeGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_REQ:
                    {
                        if (ReconnectGameReq == null)
                            return false;

                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReconnectGameReq msg = new MsgReconnectGameReq();
                            ReconnectGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_ACK:
                    {
                        if (ReconnectGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReconnectGameAck msg = new MsgReconnectGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.PlayerBase = MsgPlayerBase.Read(br);
                            msg.OtherPlayerBase = MsgPlayerBase.Read(br);
                            ReconnectGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.RECONNECT_GAME_NOTIFY:
                    {
                        if (ReconnectGameNotify == null)
                            return false;

                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReconnectGameNotify msg = new MsgReconnectGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            ReconnectGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.READY_SYNC_GAME_REQ:
                    {
                        if (ReadySyncGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReadySyncGameReq msg = new MsgReadySyncGameReq();
                            ReadySyncGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.READY_SYNC_GAME_ACK:
                    {
                        if (ReadySyncGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReadySyncGameAck msg = new MsgReadySyncGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            ReadySyncGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.READY_SYNC_GAME_NOTIFY:
                    {
                        if (ReadySyncGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgReadySyncGameNotify msg = new MsgReadySyncGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            ReadySyncGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.START_SYNC_GAME_REQ:
                    {
                        if (StartSyncGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgStartSyncGameReq msg = new MsgStartSyncGameReq();
                            StartSyncGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.START_SYNC_GAME_ACK:
                    {
                        if (StartSyncGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgStartSyncGameAck msg = new MsgStartSyncGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.Wave = br.ReadInt32();
                            msg.PlayerInfo = MsgPlayerInfo.Read(br);

                            int length = br.ReadInt32();
                            msg.GameDiceData = new MsgGameDice[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.GameDiceData[i] = MsgGameDice.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.InGameUp = new MsgInGameUp[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.InGameUp[i] = MsgInGameUp.Read(br);
                            }

                            msg.LastStatusRelay = MsgMinionStatusRelay.Read(br);

                            msg.OtherPlayerInfo = MsgPlayerInfo.Read(br);

                            length = br.ReadInt32();
                            msg.OtherGameDiceData = new MsgGameDice[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.OtherGameDiceData[i] = MsgGameDice.Read(br);
                            }

                            length = br.ReadInt32();
                            msg.OtherInGameUp = new MsgInGameUp[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.OtherInGameUp[i] = MsgInGameUp.Read(br);
                            }

                            msg.OtherLastStatusRelay = MsgMinionStatusRelay.Read(br);
                            StartSyncGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.START_SYNC_GAME_NOTIFY:
                    {
                        if (StartSyncGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgStartSyncGameNotify msg = new MsgStartSyncGameNotify();
                            StartSyncGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.END_SYNC_GAME_REQ:
                    {
                        if (EndSyncGameReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgEndSyncGameReq msg = new MsgEndSyncGameReq();
                            EndSyncGameReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.END_SYNC_GAME_ACK:
                    {
                        if (EndSyncGameAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgEndSyncGameAck msg = new MsgEndSyncGameAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.Wave = br.ReadInt32();
                            msg.RemainWaveTime = br.ReadInt32();
                            msg.SpawnCount = br.ReadByte();
                            EndSyncGameAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.END_SYNC_GAME_NOTIFY:
                    {
                        if (EndSyncGameNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgEndSyncGameNotify msg = new MsgEndSyncGameNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.RemainWaveTime = br.ReadInt32();
                            msg.SpawnCount = br.ReadByte();
                            EndSyncGameNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.GET_DICE_REQ:
                    {
                        if (GetDiceReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgGetDiceReq msg = new MsgGetDiceReq();
                            GetDiceReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.GET_DICE_ACK:
                    {
                        if (GetDiceAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgGetDiceAck msg = new MsgGetDiceAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.DiceId = br.ReadInt32();
                            msg.SlotNum = br.ReadInt16();
                            msg.Level = br.ReadInt16();
                            msg.CurrentSp = br.ReadInt32();
                            GetDiceAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.GET_DICE_NOTIFY:
                    {
                        if (GetDiceNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgGetDiceNotify msg = new MsgGetDiceNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.DiceId = br.ReadInt32();
                            msg.SlotNum = br.ReadInt16();
                            msg.Level = br.ReadInt16();
                            GetDiceNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MERGE_DICE_REQ:
                    {
                        if (MergeDiceReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMergeDiceReq msg = new MsgMergeDiceReq();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            MergeDiceReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MERGE_DICE_ACK:
                    {
                        if (MergeDiceAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMergeDiceAck msg = new MsgMergeDiceAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            msg.LevelupDiceId = br.ReadInt32();
                            msg.Level = br.ReadInt16();
                            MergeDiceAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MERGE_DICE_NOTIFY:
                    {
                        if (MergeDiceNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMergeDiceNotify msg = new MsgMergeDiceNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            msg.LevelupDiceId = br.ReadInt32();
                            msg.Level = br.ReadInt16();
                            MergeDiceNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_REQ:
                    {
                        if (InGameUpDiceReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgInGameUpDiceReq msg = new MsgInGameUpDiceReq();
                            msg.DiceId = br.ReadInt32();
                            InGameUpDiceReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_ACK:
                    {
                        if (InGameUpDiceAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgInGameUpDiceAck msg = new MsgInGameUpDiceAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.DiceId = br.ReadInt32();
                            msg.InGameUp = br.ReadInt16();
                            msg.CurrentSp = br.ReadInt32();
                            InGameUpDiceAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.INGAME_UP_DICE_NOTIFY:
                    {
                        if (InGameUpDiceNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgInGameUpDiceNotify msg = new MsgInGameUpDiceNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.DiceId = br.ReadInt32();
                            msg.InGameUp = br.ReadInt16();
                            InGameUpDiceNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.UPGRADE_SP_REQ:
                    {
                        if (UpgradeSpReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgUpgradeSpReq msg = new MsgUpgradeSpReq();
                            UpgradeSpReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.UPGRADE_SP_ACK:
                    {
                        if (UpgradeSpAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgUpgradeSpAck msg = new MsgUpgradeSpAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.Upgrade = br.ReadInt16();
                            msg.CurrentSp = br.ReadInt32();
                            UpgradeSpAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.UPGRADE_SP_NOTIFY:
                    {
                        if (UpgradeSpNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgUpgradeSpNotify msg = new MsgUpgradeSpNotify();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.Upgrade = br.ReadInt16();
                            UpgradeSpNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_REQ:
                    {
                        if (HitDamageReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgHitDamageReq msg = new MsgHitDamageReq();
                            msg.PlayerUId = br.ReadUInt16();

                            int length = br.ReadInt32();
                            msg.Damage = new MsgDamage[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.Damage[i] = MsgDamage.Read(br);
                            }
                            HitDamageReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_ACK:
                    {
                        if (HitDamageAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgHitDamageAck msg = new MsgHitDamageAck();
                            msg.ErrorCode = br.ReadInt32();
                            msg.PlayerUId = br.ReadUInt16();

                            int length = br.ReadInt32();
                            msg.DamageResult = new MsgDamageResult[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.DamageResult[i] = MsgDamageResult.Read(br);
                            }
                            HitDamageAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_NOTIFY:
                    {
                        if (HitDamageNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgHitDamageNotify msg = new MsgHitDamageNotify();
                            msg.PlayerUId = br.ReadUInt16();

                            int length = br.ReadInt32();
                            msg.DamageResult = new MsgDamageResult[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.DamageResult[i] = MsgDamageResult.Read(br);
                            }
                            HitDamageNotify(peer, msg);
                        }
                    }
                    break;


                #region Relay Protocol                
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                    {
                        if (HitDamageMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgHitDamageMinionRelay msg = new MsgHitDamageMinionRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Damage = br.ReadInt32();
                            HitDamageMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.DESTROY_MINION_RELAY:
                    {
                        if (DestroyMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();
                            msg.Id = br.ReadUInt16();
                            DestroyMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.HEAL_MINION_RELAY:
                    {
                        if (HealMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgHealMinionRelay msg = new MsgHealMinionRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Heal = br.ReadInt32();
                            HealMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.PUSH_MINION_RELAY:
                    {
                        if (PushMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgPushMinionRelay msg = new MsgPushMinionRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Dir = MsgVector3.Read(br);
                            msg.PushPower = br.ReadInt16();
                            PushMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                    {
                        if (SetMinionAnimationTriggerRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();
                            msg.Id = br.ReadUInt16();
                            msg.TargetId = br.ReadUInt16();
                            msg.Trigger = br.ReadByte();
                            SetMinionAnimationTriggerRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_ARROW_RELAY:
                    {
                        if (FireArrowRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireArrowRelay msg = new MsgFireArrowRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Dir = MsgVector3.Read(br);
                            msg.Damage = br.ReadInt32();
                            msg.MoveSpeed = br.ReadInt16();
                            FireArrowRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_BALL_BOMB_RELAY:
                    {
                        if (FireballBombRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireballBombRelay msg = new MsgFireballBombRelay();
                            msg.Id = br.ReadUInt16();
                            FireballBombRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINE_BOMB_RELAY:
                    {
                        if (MineBombRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMineBombRelay msg = new MsgMineBombRelay();
                            msg.Id = br.ReadUInt16();
                            MineBombRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                    {
                        if (SetMagicTargetIdRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();
                            msg.Id = br.ReadUInt16();
                            msg.TargetId = br.ReadUInt16();
                            SetMagicTargetIdRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                    {
                        if (SetMagicTargetRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();
                            msg.Id = br.ReadUInt16();
                            msg.X = br.ReadInt16();
                            msg.Z = br.ReadInt16();
                            SetMagicTargetRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.STURN_MINION_RELAY:
                    {
                        if (SturnMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSturnMinionRelay msg = new MsgSturnMinionRelay();
                            msg.Id = br.ReadUInt16();
                            msg.SturnTime = br.ReadInt16();
                            SturnMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.ROCKET_BOMB_RELAY:
                    {
                        if (RocketBombRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgRocketBombRelay msg = new MsgRocketBombRelay();
                            msg.Id = br.ReadUInt16();
                            RocketBombRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.ICE_BOMB_RELAY:
                    {
                        if (IceBombRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgIceBombRelay msg = new MsgIceBombRelay();
                            msg.Id = br.ReadUInt16();
                            IceBombRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.DESTROY_MAGIC_RELAY:
                    {
                        if (DestroyMagicRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgDestroyMagicRelay msg = new MsgDestroyMagicRelay();
                            msg.BaseStatId = br.ReadUInt16();
                            DestroyMagicRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_CANNON_BALL_RELAY:
                    {
                        if (FireCannonBallRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireCannonBallRelay msg = new MsgFireCannonBallRelay();
                            msg.ShootPos = MsgVector3.Read(br);
                            msg.TargetPos = MsgVector3.Read(br);
                            msg.Power = br.ReadInt32();
                            msg.Range = br.ReadInt16();
                            msg.Type = br.ReadByte();
                            FireCannonBallRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_SPEAR_RELAY:
                    {
                        if (FireSpearRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireSpearRelay msg = new MsgFireSpearRelay();
                            msg.ShootPos = MsgVector3.Read(br);
                            msg.TargetId = br.ReadUInt16();
                            msg.Power = br.ReadInt32();
                            msg.MoveSpeed = br.ReadInt16();
                            FireSpearRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_MAN_FIRE_RELAY:
                    {
                        if (FireManFireRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireManFireRelay msg = new MsgFireManFireRelay();
                            msg.Id = br.ReadUInt16();
                            FireManFireRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                    {
                        if (ActivatePoolObjectRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgActivatePoolObjectRelay msg = new MsgActivatePoolObjectRelay();
                            msg.PoolName = br.ReadInt32();
                            msg.HitPos = MsgVector3.Read(br);
                            msg.LocalScale = MsgVector3.Read(br);
                            msg.Rotation = MsgQuaternion.Read(br);
                            ActivatePoolObjectRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINION_CLOACKING_RELAY:
                    {
                        if (MinionCloackingRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMinionCloackingRelay msg = new MsgMinionCloackingRelay();
                            msg.Id = br.ReadUInt16();
                            msg.IsCloacking = br.ReadBoolean();
                            MinionCloackingRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINION_FLAG_OF_WAR_RELAY:
                    {
                        if (MinionFlagOfWarRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMinionFlagOfWarRelay msg = new MsgMinionFlagOfWarRelay();
                            msg.BaseStatId = br.ReadUInt16();
                            msg.Effect = br.ReadByte();
                            msg.IsFogOfWar = br.ReadBoolean();
                            MinionFlagOfWarRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                    {
                        if (SendMessageVoidRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSendMessageVoidRelay msg = new MsgSendMessageVoidRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Message = br.ReadByte();
                            SendMessageVoidRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                    {
                        if (SendMessageParam1Relay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSendMessageParam1Relay msg = new MsgSendMessageParam1Relay();
                            msg.Id = br.ReadUInt16();
                            msg.TargetId = br.ReadUInt16();
                            msg.Message = br.ReadByte();
                            SendMessageParam1Relay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.NECROMANCER_BULLET_RELAY:
                    {
                        if (NecromancerBulletRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgNecromancerBulletRelay msg = new MsgNecromancerBulletRelay();
                            msg.ShootPos = MsgVector3.Read(br);
                            msg.TargetId = br.ReadUInt16();
                            msg.Power = br.ReadInt32();
                            msg.BulletMoveSpeed = br.ReadInt16();
                            NecromancerBulletRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SET_MINION_TARGET_RELAY:
                    {
                        if (SetMinionTargetRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgSetMinionTargetRelay msg = new MsgSetMinionTargetRelay();
                            msg.Id = br.ReadUInt16();
                            msg.TargetId = br.ReadUInt16();
                            SetMinionTargetRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINION_STATUS_RELAY:
                    {
                        if (MinionStatusRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMinionStatusRelay msg = MsgMinionStatusRelay.Read(br);
                            MinionStatusRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SCARECROW_RELAY:
                    {
                        if (ScarecrowRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgScarecrowRelay msg = new MsgScarecrowRelay();
                            msg.BaseStatId = br.ReadUInt16();
                            msg.EyeLevel = br.ReadByte();
                            ScarecrowRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LAYZER_TARGET_RELAY:
                    {
                        if (LayzerTargetRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLayzerTargetRelay msg = new MsgLayzerTargetRelay();
                            msg.Id = br.ReadUInt16();

                            int length = br.ReadInt32();
                            byte[] bytes = br.ReadBytes(length * sizeof(ushort));
                            msg.TargetIdArray = new ushort[length];
                            for (var index = 0; index < length; index++)
                            {
                                msg.TargetIdArray[index] = BitConverter.ToUInt16(bytes, index * sizeof(ushort));
                            }
                            LayzerTargetRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_BULLET_RELAY:
                    {
                        if (FireBulletRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgFireBulletRelay msg = new MsgFireBulletRelay();
                            msg.Id = br.ReadUInt16();
                            msg.targetId = br.ReadUInt16();
                            msg.Damage = br.ReadInt32();
                            msg.MoveSpeed = br.ReadInt16();
                            msg.Type = br.ReadByte();
                            FireBulletRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINION_INVINCIBILITY_RELAY:
                    {
                        if (MinionInvincibilityRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgMinionInvincibilityRelay msg = new MsgMinionInvincibilityRelay();
                            msg.Id = br.ReadUInt16();
                            msg.Time = br.ReadInt16();
                            MinionInvincibilityRelay(peer, msg);
                        }
                    }
                    break;
                #endregion
                default:
                    {
                        return false;
                    }
            }
            return true;
        }
    }
}
