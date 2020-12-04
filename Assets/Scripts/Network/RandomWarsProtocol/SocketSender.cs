using System;
using System.IO;
using RandomWarsService.Network.Socket.NetService;

namespace RandomWarsProtocol
{
    public class SocketSender
    {
        public void JoinGameReq(Peer peer, sbyte deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(deckIndex);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_REQ, ms.ToArray());
            }
        }


        public void JoinGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                playerInfo.Write(bw);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_ACK, ms.ToArray());
            }
        }


        public void JoinGameNotify(Peer peer, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                playerInfo.Write(bw);
                peer.SendPacket((int)GameProtocol.JOIN_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void JoinCoopGameReq(Peer peer, string playerSessionId, sbyte deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerSessionId);
                bw.Write(deckIndex);
                peer.SendPacket((int)GameProtocol.JOIN_COOP_GAME_REQ, ms.ToArray());
            }
        }


        public void JoinCoopGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                playerInfo.Write(bw);
                peer.SendPacket((int)GameProtocol.JOIN_COOP_GAME_ACK, ms.ToArray());
            }
        }


        public void JoinCoopGameNotify(Peer peer, MsgPlayerInfo coopPlayerInfo, MsgPlayerInfo[] otherPlayerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                coopPlayerInfo.Write(bw);

                int length = (otherPlayerInfo == null) ? 0 : otherPlayerInfo.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    otherPlayerInfo[i].Write(bw);
                }
                peer.SendPacket((int)GameProtocol.JOIN_COOP_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void LeaveGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_REQ, ms.ToArray());
            }
        }


        public void LeaveGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_ACK, ms.ToArray());
            }
        }


        public void LeaveGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.LEAVE_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void ReadyGameReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.READY_GAME_REQ, ms.ToArray());
            }
        }


        public void ReadyGameAck(Peer peer, GameErrorCode code)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                peer.SendPacket((int)GameProtocol.READY_GAME_ACK, ms.ToArray());
            }
        }


        public void DeactiveWaitingObjectNotify(Peer peer, ushort playerUid, int currentSp)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUid);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, ms.ToArray());
            }
        }


        public void AddSpNotify(Peer peer, ushort playerUId, int currentSp)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.ADD_SP_NOTIFY, ms.ToArray());
            }
        }


        public void SpawnNotify(Peer peer, int wave, MsgSpawnInfo[] spawnInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(wave);

                int length = (spawnInfo == null) ? 0 : spawnInfo.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    spawnInfo[i].Write(bw);
                }

                peer.SendPacket((int)GameProtocol.SPAWN_NOTIFY, ms.ToArray());
            }
        }

        public void CoopSpawnNotify(Peer peer, int wave, ushort PlayerUId, MsgSpawnInfo[] spawnInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(wave);
                bw.Write(PlayerUId);

                int length = (spawnInfo == null) ? 0 : spawnInfo.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    spawnInfo[i].Write(bw);
                }

                peer.SendPacket((int)GameProtocol.COOP_SPAWN_NOTIFY, ms.ToArray());
            }
        }

        public void MonsterSpawnNotify(Peer peer, ushort PlayerUId, MsgMonster spawnBossMonster)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(PlayerUId);
                spawnBossMonster.Write(bw);
                peer.SendPacket((int)GameProtocol.MONSTER_SPAWN_NOTIFY, ms.ToArray());
            }
        }



        public void PauseGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.PAUSE_GAME_NOTIFY, ms.ToArray());
            }
        }

        public void ResumeGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.RESUME_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void ReconnectGameReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_REQ, ms.ToArray());
            }
        }


        public void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerBase playerBase, MsgPlayerBase otherPlayerBase)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                playerBase.Write(bw);
                otherPlayerBase.Write(bw);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_ACK, ms.ToArray());
            }
        }


        public void ReconnectGameNotify(Peer peer, int playerUId)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.RECONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void ReadySyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public void ReadySyncGameAck(Peer peer, GameErrorCode code) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public void ReadySyncGameNotify(Peer peer, int playerUId) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.READY_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void StartSyncGameReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public void StartSyncGameAck(Peer peer, GameErrorCode code, int wave, MsgPlayerInfo playerInfo, MsgGameDice[] gameDiceData, MsgInGameUp[] inGameUp, MsgMinionStatusRelay lastStatusRelay, MsgPlayerInfo otherPlayerInfo, MsgGameDice[] otherGameDiceData, MsgInGameUp[] otherInGameUp, MsgMinionStatusRelay otherLastStatusRelay)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(wave);
                playerInfo.Write(bw);

                int length = (gameDiceData == null) ? 0 : gameDiceData.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    gameDiceData[i].Write(bw);
                }

                length = (inGameUp == null) ? 0 : inGameUp.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    inGameUp[i].Write(bw);
                }

                lastStatusRelay.Write(bw);

                otherPlayerInfo.Write(bw);

                length = (otherGameDiceData == null) ? 0 : otherGameDiceData.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    otherGameDiceData[i].Write(bw);
                }

                length = (otherInGameUp == null) ? 0 : otherInGameUp.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    otherInGameUp[i].Write(bw);
                }

                otherLastStatusRelay.Write(bw);

                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public void StartSyncGameNotify(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.START_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void EndSyncGameReq(Peer peer) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_REQ, ms.ToArray());
            }
        }


        public void EndSyncGameAck(Peer peer, GameErrorCode code, int remainWaveTime, byte spawnCount) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(remainWaveTime);
                bw.Write(spawnCount);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_ACK, ms.ToArray());
            }
        }


        public void EndSyncGameNotify(Peer peer, ushort playerUId, int remainWaveTime, byte spawnCount) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(remainWaveTime);
                bw.Write(spawnCount);
                peer.SendPacket((int)GameProtocol.END_SYNC_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void GetDiceReq(Peer peer)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.GET_DICE_REQ, ms.ToArray());
            }
        }


        public void GetDiceAck(Peer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp)
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(diceId);
                bw.Write(slotNum);
                bw.Write(level);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.GET_DICE_ACK, ms.ToArray());
            }
        }


        public void GetDiceNotify(Peer peer, ushort playerUId, int diceId, short slotNum, short level)
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


        public void MergeDiceReq(Peer peer, short resetFieldNum, short leveupFieldNum) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                peer.SendPacket((int)GameProtocol.MERGE_DICE_REQ, ms.ToArray());
            }
        }


        public void MergeDiceAck(Peer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                bw.Write(levelUpDiceId);
                bw.Write(level);
                peer.SendPacket((int)GameProtocol.MERGE_DICE_ACK, ms.ToArray());
            }
        }


        public void MergeDiceNotify(Peer peer, ushort playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(resetFieldNum);
                bw.Write(leveupFieldNum);
                bw.Write(levelUpDiceId);
                bw.Write(level);
                peer.SendPacket((int)GameProtocol.MERGE_DICE_NOTIFY, ms.ToArray());
            }
        }


        public void InGameUpDiceReq(Peer peer, int diceId) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(diceId);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_REQ, ms.ToArray());
            }
        }


        public void InGameUpDiceAck(Peer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(diceId);
                bw.Write(inGameUp);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.INGAME_UP_DICE_ACK, ms.ToArray());
            }
        }


        public void InGameUpDiceNotify(Peer peer, ushort playerUId, int diceId, short inGameUp) 
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


        public void UpgradeSpReq(Peer peer) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_REQ, ms.ToArray());
            }
        }


        public void UpgradeSpAck(Peer peer, GameErrorCode code, short upgrade, int currentSp) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(upgrade);
                bw.Write(currentSp);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_ACK, ms.ToArray());
            }
        }


        public void UpgradeSpNotify(Peer peer, ushort playerUId, short upgrade) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(upgrade);
                peer.SendPacket((int)GameProtocol.UPGRADE_SP_NOTIFY, ms.ToArray());
            }
        }


        public void HitDamageReq(Peer peer, ushort playerUId, MsgDamage[] damage) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);

                int length = (damage == null) ? 0 : damage.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    damage[i].Write(bw);
                }
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_REQ, ms.ToArray());
            }
        }


        public void HitDamageAck(Peer peer, GameErrorCode code, ushort playerUId, MsgDamageResult[] damageResult) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write(playerUId);

                int length = (damageResult == null) ? 0 : damageResult.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    damageResult[i].Write(bw);
                }
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_ACK, ms.ToArray());
            }
        }


        public void HitDamageNotify(Peer peer, ushort playerUId, MsgDamageResult[] damageResult)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);

                int length = (damageResult == null) ? 0 : damageResult.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    damageResult[i].Write(bw);
                }
                peer.SendPacket((int)GameProtocol.HIT_DAMAGE_NOTIFY, ms.ToArray());
            }
        }


        public void EndGameNotify(Peer peer, GameErrorCode code, GAME_RESULT gameResult, byte WinningStreak, MsgReward[] normalReward, MsgReward[] streakReward, MsgReward[] perfectReward) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)code);
                bw.Write((byte)gameResult);
                bw.Write(WinningStreak);

                int length = (normalReward == null) ? 0 : normalReward.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    normalReward[i].Write(bw);
                }

                length = (streakReward == null) ? 0 : streakReward.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    streakReward[i].Write(bw);
                }

                length = (perfectReward == null) ? 0 : perfectReward.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    perfectReward[i].Write(bw);
                }

                peer.SendPacket((int)GameProtocol.END_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void DisconnectGameNotify(Peer peer, int playerUId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                peer.SendPacket((int)GameProtocol.DISCONNECT_GAME_NOTIFY, ms.ToArray());
            }
        }


        public void HitDamageMinionRelay(Peer peer, ushort playerUId, ushort id, int damage) 
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


        public void DestroyMinionRelay(Peer peer, ushort playerUId, ushort id) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.DESTROY_MINION_RELAY, ms.ToArray());
            }
        }


        public void HealMinionRelay(Peer peer, ushort playerUId, ushort id, int heal) 
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


        public void PushMinionRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, short pushPower) 
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


        public void SetMinionAnimationTriggerRelay(Peer peer, ushort playerUId, ushort id, ushort targetId, byte trigger) 
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


        public void FireArrowRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, int damage, short moveSpeed) 
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


        public void FireballBombRelay(Peer peer, ushort playerUId, ushort id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.FIRE_BALL_BOMB_RELAY, ms.ToArray());
            }
        }


        public void MineBombRelay(Peer peer, ushort playerUId, ushort id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.MINE_BOMB_RELAY, ms.ToArray());
            }
        }


        public void SetMagicTargetIdRelay(Peer peer, ushort playerUId, ushort id, ushort targetId) 
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


        public void SetMagicTargetRelay(Peer peer, ushort playerUId, ushort id, short x, short z)
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


        public void SturnMinionRelay(Peer peer, ushort playerUId, ushort id, short sturnTime)
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


        public void RocketBombRelay(Peer peer, ushort playerUId, ushort id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.ROCKET_BOMB_RELAY, ms.ToArray());
            }
        }


        public void IceBombRelay(Peer peer, ushort playerUId, ushort id) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.ICE_BOMB_RELAY, ms.ToArray());
            }
        }


        public void MsgDestroyMagic(Peer peer, ushort playerUId, ushort baseStatId) 
        {
            
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(baseStatId);
                peer.SendPacket((int)GameProtocol.DESTROY_MAGIC_RELAY, ms.ToArray());
            }
        }


        public void MsgFireCannonBall(Peer peer, ushort playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, short range, byte type) 
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


        public void FireSpearRelay(Peer peer, ushort playerUId, MsgVector3 shootPos, ushort targetId, int power, short moveSpeed)
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


        public void FireManFireRelay(Peer peer, ushort playerUId, ushort id) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                peer.SendPacket((int)GameProtocol.FIRE_MAN_FIRE_RELAY, ms.ToArray());
            }
        }


        public void ActivatePoolObjectRelay(Peer peer, int poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation) 
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


        public void MinionCloackingRelay(Peer peer, ushort playerUId, ushort id, bool isCloacking)
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


        public void MinionFogOfWarRelay(Peer peer, ushort playerUId, ushort baseStatId, short effect, bool isFogOfWar) 
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


        public void SendMessageVoidRelay(Peer peer, ushort playerUId, ushort id, byte message) 
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


        public void SendMessageParam1Relay(Peer peer, ushort playerUId, ushort id, ushort targetId, byte message) 
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


        public void NecromancerBulletRelay(Peer peer, ushort playerUId, MsgVector3 shootPos, ushort targetId, int power, short bulletMoveSpeed) 
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


        public void SetMinionTargetRelay(Peer peer, ushort playerUId, ushort id, ushort targetId) 
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


        public void MinionStatusRelay(Peer peer, ushort playerUId, MsgMinionInfo[] minionInfo, MsgMinionStatus relay, int packetCount) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(packetCount);

                bw.Write(minionInfo.Length);
                for (int i = 0; i < minionInfo.Length; i++)
                {
                    minionInfo[i].Write(bw);
                }

                relay.Write(bw);
                peer.SendPacket((int)GameProtocol.MINION_STATUS_RELAY, ms.ToArray());
            }
        }


        public void ScarecrowRelay(Peer peer, ushort playerUId, ushort baseStatId, byte eyeLevel)
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


        public void LayzerTargetRelay(Peer peer, ushort playerUId, ushort id, ushort[] targetId) 
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


        public void FireBulletRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, int damage, short moveSpeedk, byte type) 
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerUId);
                bw.Write(id);
                pos.Write(bw);
                bw.Write(damage);
                bw.Write(moveSpeedk);
                bw.Write(type);
                peer.SendPacket((int)GameProtocol.FIRE_BULLET_RELAY, ms.ToArray());
            }
        }


        public void MinionInvincibilityRelay(Peer peer, ushort playerUId, ushort id, short time) 
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