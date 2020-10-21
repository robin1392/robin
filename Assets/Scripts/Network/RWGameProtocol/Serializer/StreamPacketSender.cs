using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RWCoreNetwork.NetService;
using RWGameProtocol.Msg;


namespace RWGameProtocol.Serializer
{
    public class StreamPacketSender : PacketSender
    {
        BinaryFormatter _bf;


        public StreamPacketSender()
        {
            _bf = new BinaryFormatter();
        }


        public override void JoinGameReq(Peer peer, string playerSessionId, sbyte deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerSessionId);
                _bf.Serialize(ms, deckIndex);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_REQ, ms.ToArray());
            }
        }


        public override void JoinGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, playerInfo);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_ACK, ms.ToArray());
            }
        }


        public override void JoinGameNotify(Peer peer, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerInfo);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void LeaveGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_REQ, ms.ToArray());
            }
        }


        public override void LeaveGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_ACK, ms.ToArray());
            }
        }


        public override void LeaveGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReadyGameReq(Peer peer)
        {
            
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.READY_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReadyGameAck(Peer peer, GameErrorCode code)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                peer.SendPacket((int)GameProtocol.READY_GAME_ACK, ms.ToArray());
            }
        }


        public override void DeactiveWaitingObjectNotify(Peer peer, int playerUid, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUid);
                _bf.Serialize(ms, currentSp);
                peer.SendPacket((int)GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, ms.ToArray());
            }
        }


        public override void AddSpNotify(Peer peer, int playerUId, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, currentSp);
                peer.SendPacket((int)GameProtocol.ADD_SP_NOTIFY, ms.ToArray());
            }
        }


        public override void SpawnNotify(Peer peer, int wave)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, wave);
                peer.SendPacket((int)GameProtocol.SPAWN_NOTIFY, ms.ToArray());
            }
        }

        public override void PauseGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.PAUSE_GAME_NOTIFY, ms.ToArray());
            }
        }

        public override void ResumeGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.RESUME_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReconnectGameReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerBase playerBase, MsgPlayerBase otherPlayerBase)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, playerBase);
                _bf.Serialize(ms, otherPlayerBase);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_ACK, ms.ToArray());
            }
        }


        public override void ReconnectGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void ReadySyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void ReadySyncGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void ReadySyncGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void StartSyncGameReq(Peer peer, int playerId, int playerSpawnCount, MsgSyncMinionData[] syncMinionData, int otherPlayerId, int otherPlayerSpawnCount, MsgSyncMinionData[] otherSyncMinionData)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerId);
                _bf.Serialize(ms, playerSpawnCount);
                _bf.Serialize(ms, syncMinionData);
                _bf.Serialize(ms, otherPlayerId);
                _bf.Serialize(ms, otherPlayerSpawnCount);
                _bf.Serialize(ms, otherSyncMinionData);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void StartSyncGameAck(Peer peer, GameErrorCode code)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void StartSyncGameNotify(Peer peer, MsgPlayerInfo playerInfo, int playerSpawnCount, MsgGameDice[] gameDiceData, MsgInGameUp[] inGameUp, MsgSyncMinionData[] syncMinionData, MsgPlayerInfo otherPlayerInfo, int otherPlayerSpawnCount, MsgGameDice[] otherGameDiceData, MsgInGameUp[] otherInGameUp, MsgSyncMinionData[] otherSyncMinionData)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerInfo);
                _bf.Serialize(ms, gameDiceData);
                _bf.Serialize(ms, inGameUp);
                _bf.Serialize(ms, syncMinionData);
                _bf.Serialize(ms, playerSpawnCount);
                _bf.Serialize(ms, otherPlayerInfo);
                _bf.Serialize(ms, otherGameDiceData);
                _bf.Serialize(ms, otherInGameUp);
                _bf.Serialize(ms, otherSyncMinionData);
                _bf.Serialize(ms, otherPlayerSpawnCount);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void EndSyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public override void EndSyncGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public override void EndSyncGameNotify(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void GetDiceReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.GET_DICE_REQ, ms.ToArray());
            }
        }


        public override void GetDiceAck(Peer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, diceId);
                _bf.Serialize(ms, slotNum);
                _bf.Serialize(ms, level);
                _bf.Serialize(ms, currentSp);
                peer.SendPacket((int)GameProtocol.GET_DICE_ACK, ms.ToArray());
            }
        }


        public override void GetDiceNotify(Peer peer, int playerUId, int diceId, short slotNum, short level)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, diceId);
                _bf.Serialize(ms, slotNum);
                _bf.Serialize(ms, level);
                peer.SendPacket((int)GameProtocol.GET_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void LevelUpDiceReq(Peer peer, short resetFieldNum, short leveupFieldNum) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, resetFieldNum);
                _bf.Serialize(ms, leveupFieldNum);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_REQ, ms.ToArray());
            }
        }


        public override void LevelUpDiceAck(Peer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, resetFieldNum);
                _bf.Serialize(ms, leveupFieldNum);
                _bf.Serialize(ms, levelUpDiceId);
                _bf.Serialize(ms, level);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_ACK, ms.ToArray());
            }
        }


        public override void LevelUpDiceNotify(Peer peer, int playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, resetFieldNum);
                _bf.Serialize(ms, leveupFieldNum);
                _bf.Serialize(ms, levelUpDiceId);
                _bf.Serialize(ms, level);
                peer.SendPacket((int)GameProtocol.LEVEL_UP_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void InGameUpDiceReq(Peer peer, int diceId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, diceId);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_REQ, ms.ToArray());
            }
        }


        public override void InGameUpDiceAck(Peer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, diceId);
                _bf.Serialize(ms, inGameUp);
                _bf.Serialize(ms, currentSp);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_ACK, ms.ToArray());
            }
        }


        public override void InGameUpDiceNotify(Peer peer, int playerUId, int diceId, short inGameUp) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, diceId);
                _bf.Serialize(ms, inGameUp);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_NOTIFY, ms.ToArray());
            }
        }


        public override void UpgradeSpReq(Peer peer) 
        {
            
            using (var ms = new MemoryStream())
            {
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_REQ, ms.ToArray());
            }
        }


        public override void UpgradeSpAck(Peer peer, GameErrorCode code, short upgrade, int currentSp) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, upgrade);
                _bf.Serialize(ms, currentSp);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_ACK, ms.ToArray());
            }
        }


        public override void UpgradeSpNotify(Peer peer, int playerUId, short upgrade) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, upgrade);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_NOTIFY, ms.ToArray());
            }
        }


        public override void HitDamageReq(Peer peer, int playerUId, int damage) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, damage);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_REQ, ms.ToArray());
            }
        }


        public override void HitDamageAck(Peer peer, GameErrorCode code, int playerUId, int damage, int currentHp) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, damage);
                _bf.Serialize(ms, currentHp);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_ACK, ms.ToArray());
            }
        }


        public override void HitDamageNotify(Peer peer, int playerUId, int damage, int currentHp)
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, damage);
                _bf.Serialize(ms, currentHp);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_NOTIFY, ms.ToArray());
            }
        }


        public override void EndGameNotify(Peer peer, GameErrorCode code, int winPlayerUId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, (short)code);
                _bf.Serialize(ms, winPlayerUId);
                peer.SendPacket((int)GameProtocol.END_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void DisconnectGameNotify(Peer peer, int playerUId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                peer.SendPacket((int)GameProtocol.DISCONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }


        public override void RemoveMinionRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.REMOVE_MINION_RELAY, ms.ToArray());
            }
        }


        public override void HitDamageMinionRelay(Peer peer, int playerUId, int id, int damage) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, damage);
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_MINION_RELAY, ms.ToArray());
            }
        }


        public override void DestroyMinionRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.DESTROY_MINION_RELAY, ms.ToArray());
            }
        }


        public override void HealMinionRelay(Peer peer, int playerUId, int id, int heal) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, heal);
                peer.SendPacket((int)GameProtocol.HEAL_MINION_RELAY, ms.ToArray());
            }
        }


        public override void PushMinionRelay(Peer peer, int playerUId, int id, int x, int y, int z, int pushPower) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, new int[3] { x, y, z});
                _bf.Serialize(ms, pushPower);
                peer.SendPacket((int)GameProtocol.PUSH_MINION_RELAY, ms.ToArray());
            }
        }


        public override void SetMinionAnimationTriggerRelay(Peer peer, int playerUId, int id, int targetId, int trigger) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, targetId);
                _bf.Serialize(ms, trigger);
                peer.SendPacket((int)GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, ms.ToArray());
            }
        }


        public override void FireArrowRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeed) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, new int[3] { x, y, z });
                _bf.Serialize(ms, damage);
                _bf.Serialize(ms, moveSpeed);
                peer.SendPacket((int)GameProtocol.FIRE_ARROW_RELAY, ms.ToArray());
            }
        }


        public override void FireballBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.FIRE_BALL_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void MineBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.MINE_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void RemoveMagicRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.REMOVE_MAGIC_RELAY, ms.ToArray());
            }
        }


        public override void SetMagicTargetIdRelay(Peer peer, int playerUId, int id, int targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, targetId);
                peer.SendPacket((int)GameProtocol.SET_MAGIC_TARGET_ID_RELAY, ms.ToArray());
            }
        }


        public override void SetMagicTargetRelay(Peer peer, int playerUId, int id, int x, int z)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, x);
                _bf.Serialize(ms, z);
                peer.SendPacket((int)GameProtocol.SET_MAGIC_TARGET_POS_RELAY, ms.ToArray());
            }
        }


        public override void SturnMinionRelay(Peer peer, int playerUId, int id, int sturnTime)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, sturnTime);
                peer.SendPacket((int)GameProtocol.STURN_MINION_RELAY, ms.ToArray());
            }
        }


        public override void RocketBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.ROCKET_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void IceBombRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.ICE_BOMB_RELAY, ms.ToArray());
            }
        }


        public override void MsgDestroyMagic(Peer peer, int playerUId, int baseStatId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, baseStatId);
                peer.SendPacket((int)GameProtocol.DESTROY_MAGIC_RELAY, ms.ToArray());
            }
        }


        public override void MsgFireCannonBall(Peer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range, int type) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, shootPos);
                _bf.Serialize(ms, targetPos);
                _bf.Serialize(ms, power);
                _bf.Serialize(ms, range);
                _bf.Serialize(ms, type);
                peer.SendPacket((int)GameProtocol.FIRE_CANNON_BALL_RELAY, ms.ToArray());
            }
        }


        public override void FireSpearRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int moveSpeed)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, shootPos);
                _bf.Serialize(ms, targetId);
                _bf.Serialize(ms, power);
                _bf.Serialize(ms, moveSpeed);
                peer.SendPacket((int)GameProtocol.FIRE_SPEAR_RELAY, ms.ToArray());
            }
        }


        public override void FireManFireRelay(Peer peer, int playerUId, int id) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                peer.SendPacket((int)GameProtocol.FIRE_MAN_FIRE_RELAY, ms.ToArray());
            }
        }


        public override void ActivatePoolObjectRelay(Peer peer, int poolName, MsgVector3 hitPos, MsgQuaternion rotation, MsgVector3 localScale) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, poolName);
                _bf.Serialize(ms, hitPos);
                _bf.Serialize(ms, localScale);
                _bf.Serialize(ms, rotation);
                peer.SendPacket((int)GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, ms.ToArray());
            }
        }


        public override void MinionCloackingRelay(Peer peer, int playerUId, int id, bool isCloacking)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, isCloacking);
                peer.SendPacket((int)GameProtocol.MINION_CLOACKING_RELAY, ms.ToArray());
            }
        }


        public override void MinionFogOfWarRelay(Peer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, baseStatId);
                _bf.Serialize(ms, effect);
                _bf.Serialize(ms, isFogOfWar);
                peer.SendPacket((int)GameProtocol.MINION_FLAG_OF_WAR_RELAY, ms.ToArray());
            }
        }


        public override void SendMessageVoidRelay(Peer peer, int playerUId, int id, int message) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, message);
                peer.SendPacket((int)GameProtocol.SEND_MESSAGE_VOID_RELAY, ms.ToArray());
            }
        }


        public override void SendMessageParam1Relay(Peer peer, int playerUId, int id, int targetId, int message) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, targetId);
                _bf.Serialize(ms, message);
                peer.SendPacket((int)GameProtocol.SEND_MESSAGE_PARAM1_RELAY, ms.ToArray());
            }
        }


        public override void NecromancerBulletRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, shootPos);
                _bf.Serialize(ms, targetId);
                _bf.Serialize(ms, power);
                _bf.Serialize(ms, bulletMoveSpeed);
                peer.SendPacket((int)GameProtocol.NECROMANCER_BULLET_RELAY, ms.ToArray());
            }
        }


        public override void SetMinionTargetRelay(Peer peer, int playerUId, int id, int targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, targetId);
                peer.SendPacket((int)GameProtocol.SET_MINION_TARGET_RELAY, ms.ToArray());
            }
        }


        public override void MinionStatusRelay(Peer peer, int playerUId, byte posIndex, MsgVector3[] pos, Dictionary<GameProtocol, List<object>> relay) 
        {
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, posIndex);
                _bf.Serialize(ms, pos);
                _bf.Serialize(ms, relay);
                peer.SendPacket((int)GameProtocol.MINION_STATUS_RELAY, ms.ToArray());
            }
        }


        public override void ScarecrowRelay(Peer peer, int playerUId, int baseStatId, int eyeLevel)
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, baseStatId);
                _bf.Serialize(ms, eyeLevel);
                peer.SendPacket((int)GameProtocol.SCARECROW_RELAY, ms.ToArray());
            }
        }


        public override void LayzerTargetRelay(Peer peer, int playerUId, int id, int[] targetId) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, targetId);
                peer.SendPacket((int)GameProtocol.LAYZER_TARGET_RELAY, ms.ToArray());
            }
        }


        public override void FireBulletRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeedk, int type) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, new int[3] { x, y, z });
                _bf.Serialize(ms, damage);
                _bf.Serialize(ms, moveSpeedk);
                _bf.Serialize(ms, type);
                peer.SendPacket((int)GameProtocol.FIRE_BULLET_RELAY, ms.ToArray());
            }
        }


        public override void MinionInvincibilityRelay(Peer peer, int playerUId, int id, int time) 
        {
            
            using (var ms = new MemoryStream())
            {
                _bf.Serialize(ms, playerUId);
                _bf.Serialize(ms, id);
                _bf.Serialize(ms, time);
                peer.SendPacket((int)GameProtocol.MINION_INVINCIBILITY_RELAY, ms.ToArray());
            }
        }
    }
}