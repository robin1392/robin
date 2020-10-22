using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
using RWCoreNetwork.NetService;
using RWCoreNetwork.NetPacket;
using RWGameProtocol.Msg;


namespace RWGameProtocol.Serializer
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



    public class StreamPacketReceiver : PacketReceiver, IPacketReceiver
    {
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
                            msg.PlayerSessionId = br.ReadString();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            SpawnNotify(peer, msg);
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
                            msg.ErrorCode = br.ReadInt16();
                            msg.WinPlayerUId = br.ReadInt32();
                            EndGameNotify(peer, msg);
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.PlayerId = br.ReadInt32();
                            msg.PlayerSpawnCount = br.ReadInt32();

                            int length = br.ReadInt32();
                            msg.SyncMinionData = new MsgSyncMinionData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.SyncMinionData[i] = MsgSyncMinionData.Read(br);
                            }

                            msg.OtherPlayerId = br.ReadInt32();
                            msg.OtherPlayerSpawnCount = br.ReadInt32();

                            length = br.ReadInt32();
                            msg.OtherSyncMinionData = new MsgSyncMinionData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.OtherSyncMinionData[i] = MsgSyncMinionData.Read(br);
                            }

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
                            msg.ErrorCode = br.ReadInt16();
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

                            length = br.ReadInt32();
                            msg.SyncMinionData = new MsgSyncMinionData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.SyncMinionData[i] = MsgSyncMinionData.Read(br);
                            }

                            msg.PlayerSpawnCount = br.ReadInt32();
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

                            length = br.ReadInt32();
                            msg.OtherSyncMinionData = new MsgSyncMinionData[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.OtherSyncMinionData[i] = MsgSyncMinionData.Read(br);
                            }

                            msg.OtherPlayerSpawnCount = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
                            msg.DiceId = br.ReadInt32();
                            msg.SlotNum = br.ReadInt16();
                            msg.Level = br.ReadInt16();
                            GetDiceNotify(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_REQ:
                    {
                        if (LevelUpDiceReq == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLevelUpDiceReq msg = new MsgLevelUpDiceReq();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            LevelUpDiceReq(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_ACK:
                    {
                        if (LevelUpDiceAck == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLevelUpDiceAck msg = new MsgLevelUpDiceAck();
                            msg.ErrorCode = br.ReadInt16();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            msg.LevelupDiceId = br.ReadInt32();
                            msg.Level = br.ReadInt16();
                            LevelUpDiceAck(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LEVEL_UP_DICE_NOTIFY:
                    {
                        if (LevelUpDiceNotify == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            BinaryReader br = new BinaryReader(ms);
                            MsgLevelUpDiceNotify msg = new MsgLevelUpDiceNotify();
                            msg.PlayerUId = br.ReadInt32();
                            msg.ResetFieldNum = br.ReadInt16();
                            msg.LeveupFieldNum = br.ReadInt16();
                            msg.LevelupDiceId = br.ReadInt32();
                            msg.Level = br.ReadInt16();
                            LevelUpDiceNotify(peer, msg);
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
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
                            msg.PlayerUId = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadInt32();
                            msg.Damage = br.ReadInt32();
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
                            msg.ErrorCode = br.ReadInt16();
                            msg.PlayerUId = br.ReadInt32();
                            msg.Damage = br.ReadInt32();
                            msg.CurrentHp = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadInt32();
                            msg.Damage = br.ReadInt32();
                            msg.CurrentHp = br.ReadInt32();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            MsgMinionStatusRelay msg = new MsgMinionStatusRelay();
                            msg.PlayerUId = br.ReadUInt16();
                            msg.PosIndex = br.ReadByte();

                            int length = br.ReadInt32();
                            msg.Pos = new MsgVector3[length];
                            for (int i = 0; i < length; i++)
                            {
                                msg.Pos[i] = MsgVector3.Read(br);
                            }

                            msg.Relay = MsgMinionStatus.Read(br);
                            msg.packetCount = br.ReadInt32();

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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
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
                            msg.PlayerUId = br.ReadUInt16();
                            msg.Id = br.ReadUInt16();
                            msg.Dir = MsgVector3.Read(br);
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
                            msg.PlayerUId = br.ReadUInt16();
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
