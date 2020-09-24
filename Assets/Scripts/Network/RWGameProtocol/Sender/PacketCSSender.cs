using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using RWCoreNetwork;
using RWGameProtocol.Msg;

namespace RWGameProtocol
{
    public class PacketCSSender : PacketSender
    {
        /// <summary>
        /// 플레이어 게임 참여 요청
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="playerSessionId"></param>
        public override void JoinGameReq(Peer peer, string playerSessionId, sbyte deckIndex)
        {
            MsgJoinGameReq msg = new MsgJoinGameReq();
            msg.PlayerSessionId = playerSessionId;
            msg.DeckIndex = deckIndex;
            peer.SendPacket((short)GameProtocol.JOIN_GAME_REQ, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 참여 응답
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="code"></param>
        public override void JoinGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
        {
            MsgJoinGameAck msg = new MsgJoinGameAck();
            msg.ErrorCode = (short)code;
            msg.PlayerInfo = playerInfo;
            peer.SendPacket((short)GameProtocol.JOIN_GAME_ACK, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 퇴장 요청
        /// </summary>
        /// <param name="peer"></param>
        public override void LeaveGameReq(Peer peer)
        {
            MsgLeaveGameReq msg = new MsgLeaveGameReq();
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_REQ, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 퇴장 응답
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="code"></param>
        public override void LeaveGameAck(Peer peer, GameErrorCode code)
        {
            MsgLeaveGameAck msg = new MsgLeaveGameAck();
            msg.ErrorCode = (short)code;
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_ACK, msg.Serialize());
        }


        public override void ReadyGameReq(Peer peer)
        {
            MsgReadyGameReq msg = new MsgReadyGameReq();
            peer.SendPacket((short)GameProtocol.READY_GAME_REQ, msg.Serialize());
        }


        public override void ReadyGameAck(Peer peer, GameErrorCode code)
        {
            MsgReadyGameAck msg = new MsgReadyGameAck();
            peer.SendPacket((short)GameProtocol.READY_GAME_ACK, msg.Serialize());
        }


        public override void GetDiceReq(Peer peer)
        {
            MsgGetDiceReq msg = new MsgGetDiceReq();
            peer.SendPacket((short)GameProtocol.GET_DICE_REQ, msg.Serialize());
        }


        public override void GetDiceAck(Peer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp)
        {
            MsgGetDiceAck msg = new MsgGetDiceAck();
            msg.ErrorCode = (short)code;
            msg.DiceId = diceId;
            msg.SlotNum = slotNum;
            msg.Level = level;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.GET_DICE_ACK, msg.Serialize());
        }


        public override void LevelUpDiceReq(Peer peer, short resetFieldNum, short leveupFieldNum)
        {
            MsgLevelUpDiceReq msg = new MsgLevelUpDiceReq();
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_REQ, msg.Serialize());
        }


        public override void LevelUpDiceAck(Peer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level)
        {
            MsgLevelUpDiceAck msg = new MsgLevelUpDiceAck();
            msg.ErrorCode = (short)code;
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            msg.LevelupDiceId = levelUpDiceId;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_ACK, msg.Serialize());
        }

        public override void InGameUpDiceReq(Peer peer, int diceId)
        {
            MsgInGameUpDiceReq msg = new MsgInGameUpDiceReq();
            msg.DiceId = diceId;
            peer.SendPacket((short)GameProtocol.INGAME_UP_DICE_REQ, msg.Serialize());
        }


        public override void InGameUpDiceAck(Peer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp)
        {
            MsgInGameUpDiceAck msg = new MsgInGameUpDiceAck();
            msg.ErrorCode = (short)code;
            msg.DiceId = diceId;
            msg.InGameUp = inGameUp;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.INGAME_UP_DICE_ACK, msg.Serialize());
        }

        public override void UpgradeSpReq(Peer peer)
        {
            MsgUpgradeSpReq msg = new MsgUpgradeSpReq();
            peer.SendPacket((short)GameProtocol.UPGRADE_SP_REQ, msg.Serialize());
        }


        public override void UpgradeSpAck(Peer peer, GameErrorCode code, short upgrade, int currentSp)
        {
            MsgUpgradeSpAck msg = new MsgUpgradeSpAck();
            msg.ErrorCode = (short)code;
            msg.Upgrade = upgrade;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.UPGRADE_SP_ACK, msg.Serialize());
        }


        public override void UpgradeSpNotify(Peer peer, int playerUId, short upgrade)
        {
            MsgUpgradeSpNotify msg = new MsgUpgradeSpNotify();
            msg.PlayerUId = playerUId;
            msg.Upgrade = upgrade;
            peer.SendPacket((short)GameProtocol.UPGRADE_SP_ACK, msg.Serialize());
        }


        public override void HitDamageReq(Peer peer, int playerUId, int damage)
        {
            MsgHitDamageReq msg = new MsgHitDamageReq();
            msg.PlayerUId = playerUId;
            msg.Damage = damage;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_REQ, msg.Serialize());
        }


        public override void HitDamageAck(Peer peer, GameErrorCode code, int playerUId, int damage)
        {
            MsgHitDamageAck msg = new MsgHitDamageAck();
            msg.ErrorCode = (short)code;
            msg.PlayerUId = playerUId;
            msg.Damage = damage;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_ACK, msg.Serialize());
        }

        public override void ReconnectGameReq(Peer peer, int playerUId) 
        {
            MsgReconnectGameReq msg = new MsgReconnectGameReq();
            msg.PlayerUId = playerUId;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_REQ, msg.Serialize());
        }

        public override void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo, int wave) 
        {
            MsgReconnectGameAck msg = new MsgReconnectGameAck();
            msg.ErrorCode = (short)code;
            msg.PlayerInfo = playerInfo;
            msg.Wave = wave;
            peer.SendPacket((short)GameProtocol.RECONNECT_GAME_ACK, msg.Serialize());
        }

        public override void HitDamageNotify(Peer peer, int playerUId, int damage)
        {
            MsgHitDamageNotify msg = new MsgHitDamageNotify();
            msg.PlayerUId = playerUId;
            msg.Damage = damage;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_NOTIFY, msg.Serialize());
        }


        #region Notify Protocol                

        /// <summary>
        /// 플레이어 게임 참여를 다른 유저에게 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="info"></param>
        public override void JoinGameNotify(Peer peer, MsgPlayerInfo info)
        {
            MsgJoinGameNotify msg = new MsgJoinGameNotify();
            msg.OtherPlayerInfo = info;
            peer.SendPacket((short)GameProtocol.JOIN_GAME_NOTIFY, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 퇴장 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="playerUId"></param>
        public override void LeaveGameNotify(Peer peer, int playerUId)
        {
            MsgLeaveGameNotify msg = new MsgLeaveGameNotify();
            msg.PlayerUId = playerUId;
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_NOTIFY, msg.Serialize());
        }

    
        public override void DeactiveWaitingObjectNotify(Peer peer, int playerUid, int currentSp)
        {
            MsgDeactiveWaitingObjectNotify msg = new MsgDeactiveWaitingObjectNotify();
            msg.PlayerUId = playerUid;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, msg.Serialize());
        }


        public override void GetDiceNotify(Peer peer, int playerUid, int diceId, short slotNum, short level)
        {
            MsgGetDiceNotify msg = new MsgGetDiceNotify();
            msg.PlayerUId = playerUid;
            msg.DiceId = diceId;
            msg.SlotNum = slotNum;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.GET_DICE_NOTIFY, msg.Serialize());
        }


        public override void LevelUpDiceNotify(Peer peer, int playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level)
        {
            MsgLevelUpDiceNotify msg = new MsgLevelUpDiceNotify();
            msg.PlayerUId = playerUId;
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            msg.LevelupDiceId = levelUpDiceId;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_NOTIFY, msg.Serialize());
        }


        public override void InGameUpDiceNotify(Peer peer, int playerUId, int diceId, short inGameUp)
        {
            MsgInGameUpDiceNotify msg = new MsgInGameUpDiceNotify();
            msg.PlayerUId = playerUId;
            msg.DiceId = diceId;
            msg.InGameUp = inGameUp;
            peer.SendPacket((short)GameProtocol.INGAME_UP_DICE_NOTIFY, msg.Serialize());
        }


        /// <summary>
        /// sp 추가 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="currentSp"></param>
        public override void AddSpNotify(Peer peer, int playerUId, int currentSp)
        {
            MsgAddSpNotify msg = new MsgAddSpNotify();
            msg.PlayerUId = playerUId;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.ADD_SP_NOTIFY, msg.Serialize());
        }


        /// <summary>
        /// 스폰 시작 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="wave"></param>
        public override void SpawnNotify(Peer peer, int wave)
        {
            MsgSpawnNotify msg = new MsgSpawnNotify();
            msg.Wave = wave;
            peer.SendPacket((short)GameProtocol.SPAWN_NOTIFY, msg.Serialize());
        }


        /// <summary>
        /// 게임 종료 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="winPlayerUId"></param>
        public override void EndGameNotify(Peer peer, int winPlayerUId)
        {
            MsgEndGameNotify msg = new MsgEndGameNotify();
            msg.WinPlayerUId = winPlayerUId;
            peer.SendPacket((short)GameProtocol.END_GAME_NOTIFY, msg.Serialize());
        }

        public override void DisconnectGameNotify(Peer peer, int playerUId)
        {
            MsgDisconnectGameNotify msg = new MsgDisconnectGameNotify();
            msg.PlayerUId = playerUId;
            peer.SendPacket((short)GameProtocol.DISCONNECT_GAME_NOTIFY, msg.Serialize());
        }

        public override void ReconnectGameNotify(Peer peer, int playerUId)
        {
            MsgReconnectGameNotify msg = new MsgReconnectGameNotify();
            msg.PlayerUId = playerUId;
            peer.SendPacket((short)GameProtocol.RECONNECT_GAME_NOTIFY, msg.Serialize());
        }

        #endregion

        #region Relay Protocol                

        /// <summary>
        /// 미니언 제거 중계
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public override void RemoveMinionRelay(Peer peer, int playerUId, int id)
        {
            MsgRemoveMinionRelay msg = new MsgRemoveMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.REMOVE_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="damage"></param>
        /// <param name="delay"></param>
        public override void HitDamageMinionRelay(Peer peer, int playerUId, int id, int damage, int delay)
        {
            MsgHitDamageMinionRelay msg = new MsgHitDamageMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Damage = damage;
            msg.Delay = delay;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public override void DestroyMinionRelay(Peer peer, int playerUId, int id)
        {
            MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.DESTROY_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 미니언 회복 중계
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="heal"></param>
        public override void HealMinionRelay(Peer peer, int playerUId, int id, int heal)
        {
            MsgHealMinionRelay msg = new MsgHealMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Heal = heal;
            peer.SendPacket((short)GameProtocol.HEAL_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="pushPower"></param>
        public override void PushMinionRelay(Peer peer, int playerUId, int id, int x, int y, int z, int pushPower)
        {
            MsgPushMinionRelay msg = new MsgPushMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Dir[0] = x;
            msg.Dir[1] = y;
            msg.Dir[2] = z;
            msg.PushPower = pushPower;
            peer.SendPacket((short)GameProtocol.PUSH_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="trigger"></param>
        public override void SetMinionAnimationTriggerRelay(Peer peer, int playerUId, int id, int targetId, int trigger)
        {
            MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.TargetId = targetId;
            msg.Trigger = trigger;
            peer.SendPacket((short)GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="damage"></param>
        public override void FireArrowRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeed)
        {
            MsgFireArrowRelay msg = new MsgFireArrowRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Dir[0] = x;
            msg.Dir[1] = y;
            msg.Dir[2] = z;
            msg.Damage = damage;
            msg.MoveSpeed = moveSpeed;
            peer.SendPacket((short)GameProtocol.FIRE_ARROW_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public override void FireballBombRelay(Peer peer, int playerUId, int id)
        {
            MsgFireballBombRelay msg = new MsgFireballBombRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.FIRE_BALL_BOMB_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public override void MineBombRelay(Peer peer, int playerUId, int id)
        {
            MsgMineBombRelay msg = new MsgMineBombRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.MINE_BOMB_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 마법 제거
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public override void RemoveMagicRelay(Peer peer, int playerUId, int id)
        {
            MsgRemoveMagicRelay msg = new MsgRemoveMagicRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.REMOVE_MAGIC_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        public override void SetMagicTargetIdRelay(Peer peer, int playerUId, int id, int targetId)
        {
            MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.TargetId = targetId;
            peer.SendPacket((short)GameProtocol.SET_MAGIC_TARGET_ID_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void SetMagicTargetRelay(Peer peer, int playerUId, int id, int x, int z)
        {
            MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.X = x;
            msg.Z = z;
            peer.SendPacket((short)GameProtocol.SET_MAGIC_TARGET_POS_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="playerUId"></param>
        /// <param name="id"></param>
        /// <param name="sturnTime"></param>
        public override void SturnMinionRelay(Peer peer, int playerUId, int id, int sturnTime)
        {
            MsgSturnMinionRelay msg = new MsgSturnMinionRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.SturnTime = sturnTime;
            peer.SendPacket((short)GameProtocol.STURN_MINION_RELAY, msg.Serialize());
        }


        public override void RocketBombRelay(Peer peer, int playerUId, int id)
        {
            MsgRocketBombRelay msg = new MsgRocketBombRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.ROCKET_BOMB_RELAY, msg.Serialize());
        }


        public override void IceBombRelay(Peer peer, int playerUId, int id)
        {
            MsgIceBombRelay msg = new MsgIceBombRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.ICE_BOMB_RELAY, msg.Serialize());
        }


        public override void MsgDestroyMagic(Peer peer, int playerUId, int baseStatId)
        {
            MsgDestroyMagicRelay msg = new MsgDestroyMagicRelay();
            msg.PlayerUId = playerUId;
            msg.BaseStatId = baseStatId;
            peer.SendPacket((short)GameProtocol.DESTROY_MAGIC_RELAY, msg.Serialize());
        }


        public override void MsgFireCannonBall(Peer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range, int type)
        {
            MsgFireCannonBallRelay msg = new MsgFireCannonBallRelay();
            msg.PlayerUId = playerUId;
            msg.ShootPos = shootPos;
            msg.TargetPos = targetPos;
            msg.Power = power;
            msg.Range = range;
            msg.Type = type;
            peer.SendPacket((short)GameProtocol.FIRE_CANNON_BALL_RELAY, msg.Serialize());
        }


        public override void FireSpearRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int moveSpeed)
        {
            MsgFireSpearRelay msg = new MsgFireSpearRelay();
            msg.PlayerUId = playerUId;
            msg.ShootPos = shootPos;
            msg.TargetId = targetId;
            msg.Power = power;
            msg.MoveSpeed = moveSpeed;
            peer.SendPacket((short)GameProtocol.FIRE_SPEAR_RELAY, msg.Serialize());
        }


        public override void FireManFireRelay(Peer peer, int playerUId, int id)
        {
            MsgFireManFireRelay msg = new MsgFireManFireRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.FIRE_MAN_FIRE_RELAY, msg.Serialize());
        }


        public override void ActivatePoolObjectRelay(Peer peer, int playerUId, string poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation)
        {
            MsgActivatePoolObjectRelay msg = new MsgActivatePoolObjectRelay();
            msg.PlayerUId = playerUId;
            msg.PoolName = poolName;
            msg.HitPos = hitPos;
            msg.LocalScale = localScale;
            msg.Rotation = rotation;
            peer.SendPacket((short)GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, msg.Serialize());
        }


        public override void MinionCloackingRelay(Peer peer, int playerUId, int id, bool isCloacking)
        {
            MsgMinionCloackingRelay msg = new MsgMinionCloackingRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.IsCloacking = isCloacking;
            peer.SendPacket((short)GameProtocol.MINION_CLOACKING_RELAY, msg.Serialize());
        }


        public override void MinionFogOfWarRelay(Peer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar)
        {
            MsgMinionFogOfWarRelay msg = new MsgMinionFogOfWarRelay();
            msg.PlayerUId = playerUId;
            msg.BaseStatId = baseStatId;
            msg.Effect = effect;
            msg.IsFogOfWar = isFogOfWar;
            peer.SendPacket((short)GameProtocol.MINION_FOG_OF_WAR_RELAY, msg.Serialize());
        }


        public override void SendMessageVoidRelay(Peer peer, int playerUId, int id, string message)
        {
            MsgSendMessageVoidRelay msg = new MsgSendMessageVoidRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Message = message;
            peer.SendPacket((short)GameProtocol.SEND_MESSAGE_VOID_RELAY, msg.Serialize());
        }


        public override void SendMessageParam1Relay(Peer peer, int playerUId, int id, int targetId, string message)
        {
            MsgSendMessageParam1Relay msg = new MsgSendMessageParam1Relay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.TargetId = targetId;
            msg.Message = message;
            peer.SendPacket((short)GameProtocol.SEND_MESSAGE_PARAM1_RELAY, msg.Serialize());
        }


        public override void NecromancerBulletRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed)
        {
            MsgNecromancerBulletRelay msg = new MsgNecromancerBulletRelay();
            msg.PlayerUId = playerUId;
            msg.ShootPos = shootPos;
            msg.TargetId = targetId;
            msg.Power = power;
            msg.BulletMoveSpeed = bulletMoveSpeed;
            peer.SendPacket((short)GameProtocol.NECROMANCER_BULLET_RELAY, msg.Serialize());
        }


        public override void SetMinionTargetRelay(Peer peer, int playerUId, int id, int targetId)
        {
            MsgSetMinionTargetRelay msg = new MsgSetMinionTargetRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.TargetId = targetId;
            peer.SendPacket((short)GameProtocol.SET_MINION_TARGET_RELAY, msg.Serialize());
        }

        public override void MinionStatusRelay(Peer peer, int playerUId, byte posIndex, MsgVector3[] pos)
        {
            MsgMinionStatusRelay msg = new MsgMinionStatusRelay();
            msg.PlayerUId = playerUId;
            msg.PosIndex = posIndex;
            msg.Pos = pos;
            peer.SendPacket((short)GameProtocol.MINION_STATUS_RELAY, msg.Serialize());
        }

        public override void ScarecrowRelay(Peer peer, int playerUId, int baseStatId, int eyeLevel)
        {
            MsgScarecrowRelay msg = new MsgScarecrowRelay();
            msg.PlayerUId = playerUId;
            msg.BaseStatId = baseStatId;
            msg.EyeLevel = eyeLevel;
            peer.SendPacket((short)GameProtocol.SCARECROW_RELAY, msg.Serialize());
        }


        public override void LayzerTargetRelay(Peer peer, int playerUId, int id, int[] targetId)
        {
            MsgLazerTargetRelay msg = new MsgLazerTargetRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.TargetIdArray = targetId;
            peer.SendPacket((short)GameProtocol.LAYZER_TARGET_RELAY, msg.Serialize());
        }

        public override void FireBulletRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeed, int type)
        {
            MsgFireBulletRelay msg = new MsgFireBulletRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Dir[0] = x;
            msg.Dir[1] = y;
            msg.Dir[2] = z;
            msg.Damage = damage;
            msg.MoveSpeed = moveSpeed;
            msg.Type = type;
            peer.SendPacket((short)GameProtocol.FIRE_BULLET_RELAY, msg.Serialize());
        }

        public override void MinionInvincibilityRelay(Peer peer, int playerUId, int id, int time)
        {
            MsgMinionInvincibilityRelay msg = new MsgMinionInvincibilityRelay();
            msg.PlayerUId = playerUId;
            msg.Id = id;
            msg.Time = time;
            peer.SendPacket((short)GameProtocol.MINION_INVINCIBILITY_RELAY, msg.Serialize());
        }


        #endregion
    }
}
