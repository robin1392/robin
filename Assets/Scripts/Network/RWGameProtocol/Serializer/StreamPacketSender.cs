using System;
using System.IO;
using System.Runtime.InteropServices;
using RWCoreNetwork.NetService;
using RWGameProtocol.Msg;

namespace RWGameProtocol.Serializer
{
    public class StreamPacketSender : PacketSender
    {
        public override void JoinGameReq(Peer peer, string playerSessionId, sbyte deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerSessionId);
                bw.Write(deckIndex);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_REQ, ms.ToArray());
            }
        }


        public override void JoinGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                playerInfo.Write(bw);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_ACK, ms.ToArray());
            }
        }


        public override void JoinGameNotify(Peer peer, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                playerInfo.Write(bw);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void LeaveGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_REQ, ms.ToArray());
            }
        }


        public override void LeaveGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_ACK, ms.ToArray());
            }
        }


        public override void LeaveGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReadyGameReq(Peer peer)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.READY_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReadyGameAck(Peer peer, GameErrorCode code)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                peer.SendPacket((int)GameProtocol.READY_GAME_ACK, ms.ToArray());
            }
        }


        public override void DeactiveWaitingObjectNotify(Peer peer, int playerUid, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUid);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, ms.ToArray());
            }
        }


        public override void AddSpNotify(Peer peer, int playerUId, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.ADD_SP_NOTIFY, ms.ToArray());
            }
        }


        public override void SpawnNotify(Peer peer, int wave)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(wave);
                peer.SendPacket((int)GameProtocol.SPAWN_NOTIFY, ms.ToArray());
            }
        }

        public override void PauseGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.PAUSE_GAME_NOTIFY, ms.ToArray());
            }
        }

        public override void ResumeGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.RESUME_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReconnectGameReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerBase playerBase, MsgPlayerBase otherPlayerBase)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                playerBase.Write(bw);
                otherPlayerBase.Write(bw);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_ACK, ms.ToArray());
            }
        }


        public override void ReconnectGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReadySyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReadySyncGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void ReadySyncGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void StartSyncGameReq(Peer peer, int playerId, int playerSpawnCount, MsgSyncMinionData[] syncMinionData, int otherPlayerId, int otherPlayerSpawnCount, MsgSyncMinionData[] otherSyncMinionData)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerId);
                bw.Write(playerSpawnCount);
                bw.Write(syncMinionData.Length);
                for (int i = 0; i < syncMinionData.Length; i++)
                {
                    syncMinionData[i].Write(bw);
                }

                bw.Write(otherPlayerId);
                bw.Write(otherPlayerSpawnCount);
                bw.Write(otherSyncMinionData.Length);
                for (int i = 0; i < otherSyncMinionData.Length; i++)
                {
                    otherSyncMinionData[i].Write(bw);
                }

                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void StartSyncGameAck(Peer peer, GameErrorCode code)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void StartSyncGameNotify(Peer peer, MsgPlayerInfo playerInfo, int playerSpawnCount, MsgGameDice[] gameDiceData, MsgInGameUp[] inGameUp, MsgSyncMinionData[] syncMinionData, MsgPlayerInfo otherPlayerInfo, int otherPlayerSpawnCount, MsgGameDice[] otherGameDiceData, MsgInGameUp[] otherInGameUp, MsgSyncMinionData[] otherSyncMinionData)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                playerInfo.Write(bw);

                bw.Write(gameDiceData.Length);
                byte[] bytes = new byte[gameDiceData.Length * sizeof(int)];
                Buffer.BlockCopy(gameDiceData, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(inGameUp.Length);
                bytes = new byte[inGameUp.Length * sizeof(int)];
                Buffer.BlockCopy(inGameUp, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(syncMinionData.Length);
                bytes = new byte[syncMinionData.Length * sizeof(int)];
                Buffer.BlockCopy(syncMinionData, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(playerSpawnCount);
                otherPlayerInfo.Write(bw);

                bw.Write(otherGameDiceData.Length);
                bytes = new byte[otherGameDiceData.Length * sizeof(int)];
                Buffer.BlockCopy(otherGameDiceData, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(otherInGameUp.Length);
                bytes = new byte[otherInGameUp.Length * sizeof(int)];
                Buffer.BlockCopy(otherInGameUp, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(otherSyncMinionData.Length);
                bytes = new byte[otherSyncMinionData.Length * sizeof(int)];
                Buffer.BlockCopy(otherSyncMinionData, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);

                bw.Write(otherPlayerSpawnCount);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void EndSyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void EndSyncGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void EndSyncGameNotify(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void GetDiceReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.GET_DICE_REQ, ms.ToArray());
            }
        }


        public override void GetDiceAck(Peer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(diceId);
                bw.Write(slotNum);
                bw.Write(level);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.GET_DICE_ACK, ms.ToArray());
            }
        }


        public override void GetDiceNotify(Peer peer, int playerUId, int diceId, short slotNum, short level)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(diceId);
                bw.Write(slotNum);
                bw.Write(level);
                peer.SendPacket((int)GameProtocol.GET_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void LevelUpDiceReq(Peer peer, short resetFieldNum, short leveupFieldNum) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_REQ, ms.ToArray());
            }
        }


        public override void LevelUpDiceAck(Peer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                bw.Write(levelUpDiceId);
                bw.Write(level);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_ACK, ms.ToArray());
            }
        }


        public override void LevelUpDiceNotify(Peer peer, int playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                bw.Write(levelUpDiceId);
                bw.Write(level);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void InGameUpDiceReq(Peer peer, int diceId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(diceId);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_REQ, ms.ToArray());
            }
        }


        public override void InGameUpDiceAck(Peer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(diceId);
                bw.Write(inGameUp);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_ACK, ms.ToArray());
            }
        }


        public override void InGameUpDiceNotify(Peer peer, int playerUId, int diceId, short inGameUp) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(diceId);
                bw.Write(inGameUp);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void UpgradeSpReq(Peer peer) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_REQ, ms.ToArray());
            }
        }


        public override void UpgradeSpAck(Peer peer, GameErrorCode code, short upgrade, int currentSp) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(upgrade);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_ACK, ms.ToArray());
            }
        }


        public override void UpgradeSpNotify(Peer peer, int playerUId, short upgrade) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(upgrade);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_NOTIFY, ms.ToArray());
            }
        }


        public override void HitDamageReq(Peer peer, int playerUId, int damage) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(damage);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_REQ, ms.ToArray());
            }
        }


        public override void HitDamageAck(Peer peer, GameErrorCode code, int playerUId, int damage, int currentHp) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(playerUId);
                bw.Write(damage);
                bw.Write(currentHp);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_ACK, ms.ToArray());
            }
        }


        public override void HitDamageNotify(Peer peer, int playerUId, int damage, int currentHp)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(damage);
                bw.Write(currentHp);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_NOTIFY, ms.ToArray());
            }
        }


        public override void EndGameNotify(Peer peer, GameErrorCode code, int winPlayerUId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((short)code);
                bw.Write(winPlayerUId);
                peer.SendPacket((int)GameProtocol.END_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void DisconnectGameNotify(Peer peer, int playerUId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.DISCONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }
        

        public override void HitDamageMinionRelay(Peer peer, int playerUId, int id, int damage) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(damage);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_MINION_RELAY, ms.ToArray());
            }
        }


        public override void DestroyMinionRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.DESTROY_MINION_RELAY, ms.ToArray());
            }
        }


        public override void HealMinionRelay(Peer peer, int playerUId, int id, int heal) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(heal);
                peer.SendPacket((int)GameProtocol.HEAL_MINION_RELAY, ms.ToArray());
            }
        }


        public override void PushMinionRelay(Peer peer, int playerUId, int id, MsgVector3 pos, int pushPower) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                pos.Write(bw);
                bw.Write(pushPower);
                peer.SendPacket((int)GameProtocol.PUSH_MINION_RELAY, ms.ToArray());
            }
        }


        public override void SetMinionAnimationTriggerRelay(Peer peer, int playerUId, int id, int targetId, int trigger) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(targetId);
                bw.Write(trigger);
                peer.SendPacket((int)GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, ms.ToArray());
            }
        }


        public override void FireArrowRelay(Peer peer, int playerUId, int id, MsgVector3 pos, int damage, int moveSpeed) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                pos.Write(bw);
                bw.Write(damage);
                bw.Write(moveSpeed);
                peer.SendPacket((int)GameProtocol.FIRE_ARROW_RELAY, ms.ToArray());
            }
        }


        public override void FireballBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.FIRE_BALL_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void MineBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.MINE_BOMB_RELAY, ms.ToArray());
            }
        }
        

        public override void SetMagicTargetIdRelay(Peer peer, int playerUId, int id, int targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(targetId);
                peer.SendPacket((int)GameProtocol.SET_MAGIC_TARGET_ID_RELAY, ms.ToArray());
            }
        }


        public override void SetMagicTargetRelay(Peer peer, int playerUId, int id, int x, int z)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(x);
                bw.Write(z);
                peer.SendPacket((int)GameProtocol.SET_MAGIC_TARGET_POS_RELAY, ms.ToArray());
            }
        }


        public override void SturnMinionRelay(Peer peer, int playerUId, int id, int sturnTime)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(sturnTime);
                peer.SendPacket((int)GameProtocol.STURN_MINION_RELAY, ms.ToArray());
            }
        }


        public override void RocketBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.ROCKET_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void IceBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.ICE_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void MsgDestroyMagic(Peer peer, int playerUId, int baseStatId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(baseStatId);
                peer.SendPacket((int)GameProtocol.DESTROY_MAGIC_RELAY, ms.ToArray());
            }
        }


        public override void MsgFireCannonBall(Peer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range, int type) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                shootPos.Write(bw);
                targetPos.Write(bw);
                bw.Write(power);
                bw.Write(range);
                bw.Write(type);
                peer.SendPacket((int)GameProtocol.FIRE_CANNON_BALL_RELAY, ms.ToArray());
            }
        }


        public override void FireSpearRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int moveSpeed)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                shootPos.Write(bw);
                bw.Write(targetId);
                bw.Write(power);
                bw.Write(moveSpeed);
                peer.SendPacket((int)GameProtocol.FIRE_SPEAR_RELAY, ms.ToArray());
            }
        }


        public override void FireManFireRelay(Peer peer, int playerUId, int id) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.FIRE_MAN_FIRE_RELAY, ms.ToArray());
            }
        }


        public override void ActivatePoolObjectRelay(Peer peer, int poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(poolName);
                hitPos.Write(bw);
                localScale.Write(bw);
                rotation.Write(bw);
                peer.SendPacket((int)GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, ms.ToArray());
            }
        }


        public override void MinionCloackingRelay(Peer peer, int playerUId, int id, bool isCloacking)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(isCloacking);
                peer.SendPacket((int)GameProtocol.MINION_CLOACKING_RELAY, ms.ToArray());
            }
        }


        public override void MinionFogOfWarRelay(Peer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(baseStatId);
                bw.Write(effect);
                bw.Write(isFogOfWar);
                peer.SendPacket((int)GameProtocol.MINION_FLAG_OF_WAR_RELAY, ms.ToArray());
            }
        }


        public override void SendMessageVoidRelay(Peer peer, int playerUId, int id, int message) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(message);
                peer.SendPacket((int)GameProtocol.SEND_MESSAGE_VOID_RELAY, ms.ToArray());
            }
        }


        public override void SendMessageParam1Relay(Peer peer, int playerUId, int id, int targetId, int message) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(targetId);
                bw.Write(message);
                peer.SendPacket((int)GameProtocol.SEND_MESSAGE_PARAM1_RELAY, ms.ToArray());
            }
        }


        public override void NecromancerBulletRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                shootPos.Write(bw);
                bw.Write(targetId);
                bw.Write(power);
                bw.Write(bulletMoveSpeed);
                peer.SendPacket((int)GameProtocol.NECROMANCER_BULLET_RELAY, ms.ToArray());
            }
        }


        public override void SetMinionTargetRelay(Peer peer, int playerUId, int id, int targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(targetId);
                peer.SendPacket((int)GameProtocol.SET_MINION_TARGET_RELAY, ms.ToArray());
            }
        }


        public override void MinionStatusRelay(Peer peer, int playerUId, byte posIndex, MsgVector3[] pos, MsgMinionStatus relay, int packetCount) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(posIndex);

                bw.Write(pos.Length);
                for (int i = 0; i < pos.Length; i++)
                {
                    pos[i].Write(bw);
                }

                relay.Write(bw);
                bw.Write(packetCount);
                peer.SendPacket((int)GameProtocol.MINION_STATUS_RELAY, ms.ToArray());
            }
        }


        public override void ScarecrowRelay(Peer peer, int playerUId, int baseStatId, int eyeLevel)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(baseStatId);
                bw.Write(eyeLevel);
                peer.SendPacket((int)GameProtocol.SCARECROW_RELAY, ms.ToArray());
            }
        }


        public override void LayzerTargetRelay(Peer peer, int playerUId, int id, int[] targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);

                bw.Write(targetId.Length);
                byte[] bytes = new byte[targetId.Length * sizeof(int)];
                Buffer.BlockCopy(targetId, 0, bytes, 0, sizeof(byte));
                bw.Write(bytes);
                peer.SendPacket((int)GameProtocol.LAYZER_TARGET_RELAY, ms.ToArray());
            }
        }


        public override void FireBulletRelay(Peer peer, int playerUId, int id, MsgVector3 dir, int damage, int moveSpeedk, int type) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                dir.Write(bw);
                bw.Write(damage);
                bw.Write(moveSpeedk);
                bw.Write(type);
                peer.SendPacket((int)GameProtocol.FIRE_BULLET_RELAY, ms.ToArray());
            }
        }


        public override void MinionInvincibilityRelay(Peer peer, int playerUId, int id, int time) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                bw.Write(time);
                peer.SendPacket((int)GameProtocol.MINION_INVINCIBILITY_RELAY, ms.ToArray());
            }
        }
    }
}