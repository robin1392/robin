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
        BinaryFormatter _bf;


        public StreamPacketReceiver()
        {
            _bf = new BinaryFormatter();
            _bf.Binder = new CustomizedBinder();
        }


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
                            MsgJoinGameReq msg = new MsgJoinGameReq();
                            msg.PlayerSessionId = (string)_bf.Deserialize(ms);
                            msg.DeckIndex = (sbyte)_bf.Deserialize(ms);
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
                            MsgJoinGameAck msg = new MsgJoinGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.PlayerInfo = (MsgPlayerInfo)_bf.Deserialize(ms);
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
                            MsgJoinGameNotify msg = new MsgJoinGameNotify();
                            msg.OtherPlayerInfo = (MsgPlayerInfo)_bf.Deserialize(ms);
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
                            MsgLeaveGameAck msg = new MsgLeaveGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
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
                            MsgLeaveGameNotify msg = new MsgLeaveGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgReadyGameAck msg = new MsgReadyGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
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
                            MsgDeactiveWaitingObjectNotify msg = new MsgDeactiveWaitingObjectNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.CurrentSp = (int)_bf.Deserialize(ms);
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
                            MsgAddSpNotify msg = new MsgAddSpNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.CurrentSp = (int)_bf.Deserialize(ms);
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
                            MsgSpawnNotify msg = new MsgSpawnNotify();
                            msg.Wave = (int)_bf.Deserialize(ms);
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
                            MsgEndGameNotify msg = new MsgEndGameNotify();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.WinPlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgDisconnectGameNotify msg = new MsgDisconnectGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgPauseGameNotify msg = new MsgPauseGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgResumeGameNotify msg = new MsgResumeGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgReconnectGameAck msg = new MsgReconnectGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.PlayerBase = (MsgPlayerBase)_bf.Deserialize(ms);
                            msg.OtherPlayerBase = (MsgPlayerBase)_bf.Deserialize(ms);
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
                            MsgReconnectGameNotify msg = new MsgReconnectGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgReadySyncGameAck msg = new MsgReadySyncGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
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
                            MsgReadySyncGameNotify msg = new MsgReadySyncGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgStartSyncGameReq msg = new MsgStartSyncGameReq();
                            msg.PlayerId = (int)_bf.Deserialize(ms);
                            msg.PlayerSpawnCount = (int)_bf.Deserialize(ms);
                            msg.SyncMinionData = (MsgSyncMinionData[])_bf.Deserialize(ms);
                            msg.OtherPlayerId = (int)_bf.Deserialize(ms);
                            msg.OtherPlayerSpawnCount = (int)_bf.Deserialize(ms);
                            msg.OtherSyncMinionData = (MsgSyncMinionData[])_bf.Deserialize(ms);
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
                            MsgStartSyncGameAck msg = new MsgStartSyncGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
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
                            MsgStartSyncGameNotify msg = new MsgStartSyncGameNotify();
                            msg.PlayerInfo = (MsgPlayerInfo)_bf.Deserialize(ms);
                            msg.GameDiceData = (MsgGameDice[])_bf.Deserialize(ms);
                            msg.InGameUp = (MsgInGameUp[])_bf.Deserialize(ms);
                            msg.SyncMinionData = (MsgSyncMinionData[])_bf.Deserialize(ms);
                            msg.PlayerSpawnCount = (int)_bf.Deserialize(ms);

                            msg.OtherPlayerInfo = (MsgPlayerInfo)_bf.Deserialize(ms);
                            msg.OtherGameDiceData = (MsgGameDice[])_bf.Deserialize(ms);
                            msg.OtherInGameUp = (MsgInGameUp[])_bf.Deserialize(ms);
                            msg.OtherSyncMinionData = (MsgSyncMinionData[])_bf.Deserialize(ms);
                            msg.OtherPlayerSpawnCount = (int)_bf.Deserialize(ms);

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
                            MsgEndSyncGameAck msg = new MsgEndSyncGameAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
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
                            MsgEndSyncGameNotify msg = new MsgEndSyncGameNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
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
                            MsgGetDiceAck msg = new MsgGetDiceAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.DiceId = (int)_bf.Deserialize(ms);
                            msg.SlotNum = (short)_bf.Deserialize(ms);
                            msg.Level = (short)_bf.Deserialize(ms);
                            msg.CurrentSp = (int)_bf.Deserialize(ms);
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
                            MsgGetDiceNotify msg = new MsgGetDiceNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.DiceId = (int)_bf.Deserialize(ms);
                            msg.SlotNum = (short)_bf.Deserialize(ms);
                            msg.Level = (short)_bf.Deserialize(ms);
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
                            MsgLevelUpDiceReq msg = new MsgLevelUpDiceReq();
                            msg.ResetFieldNum = (short)_bf.Deserialize(ms);
                            msg.LeveupFieldNum = (short)_bf.Deserialize(ms);
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
                            MsgLevelUpDiceAck msg = new MsgLevelUpDiceAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.ResetFieldNum = (short)_bf.Deserialize(ms);
                            msg.LeveupFieldNum = (short)_bf.Deserialize(ms);
                            msg.LevelupDiceId = (int)_bf.Deserialize(ms);
                            msg.Level = (short)_bf.Deserialize(ms);
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
                            MsgLevelUpDiceNotify msg = new MsgLevelUpDiceNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.ResetFieldNum = (short)_bf.Deserialize(ms);
                            msg.LeveupFieldNum = (short)_bf.Deserialize(ms);
                            msg.LevelupDiceId = (int)_bf.Deserialize(ms);
                            msg.Level = (short)_bf.Deserialize(ms);
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
                            MsgInGameUpDiceReq msg = new MsgInGameUpDiceReq();
                            msg.DiceId = (int)_bf.Deserialize(ms);
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
                            MsgInGameUpDiceAck msg = new MsgInGameUpDiceAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.DiceId = (int)_bf.Deserialize(ms);
                            msg.InGameUp = (short)_bf.Deserialize(ms);
                            msg.CurrentSp = (int)_bf.Deserialize(ms);
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
                            MsgInGameUpDiceNotify msg = new MsgInGameUpDiceNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.DiceId = (int)_bf.Deserialize(ms);
                            msg.InGameUp = (short)_bf.Deserialize(ms);
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
                            MsgUpgradeSpAck msg = new MsgUpgradeSpAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.Upgrade = (short)_bf.Deserialize(ms);
                            msg.CurrentSp = (int)_bf.Deserialize(ms);
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
                            MsgUpgradeSpNotify msg = new MsgUpgradeSpNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Upgrade = (short)_bf.Deserialize(ms);
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
                            MsgHitDamageReq msg = new MsgHitDamageReq();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
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
                            MsgHitDamageAck msg = new MsgHitDamageAck();
                            msg.ErrorCode = (short)_bf.Deserialize(ms);
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
                            msg.CurrentHp = (int)_bf.Deserialize(ms);
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
                            MsgHitDamageNotify msg = new MsgHitDamageNotify();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
                            msg.CurrentHp = (int)_bf.Deserialize(ms);
                            HitDamageNotify(peer, msg);
                        }
                    }
                    break;


                #region Relay Protocol                
                case GameProtocol.REMOVE_MINION_RELAY:
                    {
                        if (RemoveMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgRemoveMinionRelay msg = new MsgRemoveMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            RemoveMinionRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                    {
                        if (HitDamageMinionRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgHitDamageMinionRelay msg = new MsgHitDamageMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
                            msg.Delay = (int)_bf.Deserialize(ms);
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
                            MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
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
                            MsgHealMinionRelay msg = new MsgHealMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Heal = (int)_bf.Deserialize(ms);
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
                            MsgPushMinionRelay msg = new MsgPushMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Dir = (int[])_bf.Deserialize(ms);
                            msg.PushPower = (int)_bf.Deserialize(ms);
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
                            MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
                            msg.Trigger = (int)_bf.Deserialize(ms);
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
                            MsgFireArrowRelay msg = new MsgFireArrowRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Dir = (int[])_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
                            msg.MoveSpeed = (int)_bf.Deserialize(ms);
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
                            MsgFireballBombRelay msg = new MsgFireballBombRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
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
                            MsgMineBombRelay msg = new MsgMineBombRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            MineBombRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.REMOVE_MAGIC_RELAY:
                    {
                        if (RemoveMagicRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgRemoveMagicRelay msg = new MsgRemoveMagicRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            RemoveMagicRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                    {
                        if (SetMagicTargetIdRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
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
                            MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.X = (int)_bf.Deserialize(ms);
                            msg.Z = (int)_bf.Deserialize(ms);
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
                            MsgSturnMinionRelay msg = new MsgSturnMinionRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.SturnTime = (int)_bf.Deserialize(ms);
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
                            MsgRocketBombRelay msg = new MsgRocketBombRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
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
                            MsgIceBombRelay msg = new MsgIceBombRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
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
                            MsgDestroyMagicRelay msg = new MsgDestroyMagicRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.BaseStatId = (int)_bf.Deserialize(ms);
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
                            MsgFireCannonBallRelay msg = new MsgFireCannonBallRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.ShootPos = (MsgVector3)_bf.Deserialize(ms);
                            msg.TargetPos = (MsgVector3)_bf.Deserialize(ms);
                            msg.Power = (int)_bf.Deserialize(ms);
                            msg.Range = (int)_bf.Deserialize(ms);
                            msg.Type = (int)_bf.Deserialize(ms);
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
                            MsgFireSpearRelay msg = new MsgFireSpearRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.ShootPos = (MsgVector3)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
                            msg.Power = (int)_bf.Deserialize(ms);
                            msg.MoveSpeed = (int)_bf.Deserialize(ms);
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
                            MsgFireManFireRelay msg = new MsgFireManFireRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
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
                            MsgActivatePoolObjectRelay msg = new MsgActivatePoolObjectRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.PoolName = (int)_bf.Deserialize(ms);
                            msg.HitPos = (MsgVector3)_bf.Deserialize(ms);
                            msg.LocalScale = (MsgVector3)_bf.Deserialize(ms);
                            msg.Rotation = (MsgQuaternion)_bf.Deserialize(ms);
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
                            MsgMinionCloackingRelay msg = new MsgMinionCloackingRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.IsCloacking = (bool)_bf.Deserialize(ms);
                            MinionCloackingRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.MINION_FOG_OF_WAR_RELAY:
                    {
                        if (MinionFogOfWarRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgMinionFogOfWarRelay msg = new MsgMinionFogOfWarRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.BaseStatId = (int)_bf.Deserialize(ms);
                            msg.Effect = (int)_bf.Deserialize(ms);
                            msg.IsFogOfWar = (bool)_bf.Deserialize(ms);
                            MinionFogOfWarRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                    {
                        if (SendMessageVoidRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgSendMessageVoidRelay msg = new MsgSendMessageVoidRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Message = (int)_bf.Deserialize(ms);
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
                            MsgSendMessageParam1Relay msg = new MsgSendMessageParam1Relay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
                            msg.Message = (int)_bf.Deserialize(ms);
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
                            MsgNecromancerBulletRelay msg = new MsgNecromancerBulletRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.ShootPos = (MsgVector3)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
                            msg.Power = (int)_bf.Deserialize(ms);
                            msg.BulletMoveSpeed = (int)_bf.Deserialize(ms);
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
                            MsgSetMinionTargetRelay msg = new MsgSetMinionTargetRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.TargetId = (int)_bf.Deserialize(ms);
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
                            MsgMinionStatusRelay msg = new MsgMinionStatusRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.PosIndex = (byte)_bf.Deserialize(ms);
                            msg.Pos = (MsgVector3[])_bf.Deserialize(ms);
                            msg.Relay = (Dictionary<GameProtocol, List<object[]>>) _bf.Deserialize(ms);
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
                            MsgScarecrowRelay msg = new MsgScarecrowRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.BaseStatId = (int)_bf.Deserialize(ms);
                            msg.EyeLevel = (int)_bf.Deserialize(ms);
                            ScarecrowRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.LAYZER_TARGET_RELAY:
                    {
                        if (LazerTargetRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgLazerTargetRelay msg = new MsgLazerTargetRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.TargetIdArray = (int[])_bf.Deserialize(ms);
                            LazerTargetRelay(peer, msg);
                        }
                    }
                    break;
                case GameProtocol.FIRE_BULLET_RELAY:
                    {
                        if (FireBulletRelay == null)
                            return false;

                        
                        using (var ms = new MemoryStream(buffer))
                        {
                            MsgFireBulletRelay msg = new MsgFireBulletRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Dir = (int[])_bf.Deserialize(ms);
                            msg.Damage = (int)_bf.Deserialize(ms);
                            msg.MoveSpeed = (int)_bf.Deserialize(ms);
                            msg.Type = (int)_bf.Deserialize(ms);
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
                            MsgMinionInvincibilityRelay msg = new MsgMinionInvincibilityRelay();
                            msg.PlayerUId = (int)_bf.Deserialize(ms);
                            msg.Id = (int)_bf.Deserialize(ms);
                            msg.Time = (int)_bf.Deserialize(ms);
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
