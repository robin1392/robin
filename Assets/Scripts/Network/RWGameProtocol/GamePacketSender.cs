using System;
using System.Collections.Generic;
using System.Text;
using RWCoreNetwork;
using RWGameProtocol.Msg;

namespace RWGameProtocol
{
    public class GamePacketSender
    {
        /// <summary>
        /// 플레이어 게임 참여 요청
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="playerSessionId"></param>
        public void JoinGameReq(IPeer peer, string playerSessionId)
        {
            MsgJoinGameReq msg = new MsgJoinGameReq();
            msg.PlayerSessionId = playerSessionId;
            peer.SendPacket((short)GameProtocol.JOIN_GAME_REQ, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 참여 응답
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="code"></param>
        public void JoinGameAck(IPeer peer, GameErrorCode code, MsgPlayerInfo playerInfo)
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
        public void LeaveGameReq(IPeer peer)
        {
            MsgLeaveGameReq msg = new MsgLeaveGameReq();
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_REQ, msg.Serialize());
        }


        /// <summary>
        /// 플레이어 게임 퇴장 응답
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="code"></param>
        public void LeaveGameAck(IPeer peer, GameErrorCode code)
        {
            MsgLeaveGameAck msg = new MsgLeaveGameAck();
            msg.ErrorCode = (short)code;
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_ACK, msg.Serialize());
        }


        public void ReadyGameReq(IPeer peer)
        {
            MsgReadyGameReq msg = new MsgReadyGameReq();
            peer.SendPacket((short)GameProtocol.READY_GAME_REQ, msg.Serialize());
        }


        public void ReadyGameAck(IPeer peer, GameErrorCode code)
        {
            MsgReadyGameAck msg = new MsgReadyGameAck();
            peer.SendPacket((short)GameProtocol.READY_GAME_ACK, msg.Serialize());
        }


        public void SetDeckReq(IPeer peer, int[] deck)
        {
            MsgSetDeckReq msg = new MsgSetDeckReq();
            msg.Deck = deck;
            peer.SendPacket((short)GameProtocol.SET_DECK_REQ, msg.Serialize());
        }


        public void SetDeckAck(IPeer peer, GameErrorCode code)
        {
            MsgSetDeckAck msg = new MsgSetDeckAck();
            msg.ErrorCode = (short)code;
            peer.SendPacket((short)GameProtocol.SET_DECK_ACK, msg.Serialize());
        }


        public void GetDiceReq(IPeer peer)
        {
            MsgGetDiceReq msg = new MsgGetDiceReq();
            peer.SendPacket((short)GameProtocol.GET_DICE_REQ, msg.Serialize());
        }


        public void GetDiceAck(IPeer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp)
        {
            MsgGetDiceAck msg = new MsgGetDiceAck();
            msg.ErrorCode = (short)code;
            msg.DiceId = diceId;
            msg.SlotNum = slotNum;
            msg.Level = level;
            msg.CurrentSp = currentSp;
            peer.SendPacket((short)GameProtocol.GET_DICE_ACK, msg.Serialize());
        }


        public void LevelUpDiceReq(IPeer peer, int resetFieldNum, int leveupFieldNum)
        {
            MsgLevelUpDiceReq msg = new MsgLevelUpDiceReq();
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_REQ, msg.Serialize());
        }


        public void LevelUpDiceAck(IPeer peer, GameErrorCode code, int resetFieldNum, int leveupFieldNum, int levelUpDiceId, int level)
        {
            MsgLevelUpDiceAck msg = new MsgLevelUpDiceAck();
            msg.ErrorCode = (short)code;
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            msg.LevelupDiceId = levelUpDiceId;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_ACK, msg.Serialize());
        }


        public void HitDamageReq(IPeer peer, float damage, float delay)
        {
            MsgHitDamageReq msg = new MsgHitDamageReq();
            msg.Damage = damage;
            msg.Delay = delay;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_REQ, msg.Serialize());
        }


        public void HitDamageAck(IPeer peer, GameErrorCode code, float damage, float delay)
        {
            MsgHitDamageAck msg = new MsgHitDamageAck();
            msg.ErrorCode = (short)code;
            msg.Damage = damage;
            msg.Delay = delay;
            peer.SendPacket((short)GameProtocol.HIT_DAMAGE_ACK, msg.Serialize());
        }


        #region Notify Protocol                

        /// <summary>
        /// 플레이어 게임 참여를 다른 유저에게 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="info"></param>
        public void JoinGameNotify(IPeer peer, MsgPlayerInfo info)
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
        public void LeaveGameNotify(IPeer peer, int playerUId)
        {
            MsgLeaveGameNotify msg = new MsgLeaveGameNotify();
            msg.PlayerUId = playerUId;
            peer.SendPacket((short)GameProtocol.LEAVE_GAME_NOTIFY, msg.Serialize());
        }


        public void DeactiveWaitingObjectNotify(IPeer peer)
        {
            MsgDeactiveWaitingObjectNotify msg = new MsgDeactiveWaitingObjectNotify();
            peer.SendPacket((short)GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, msg.Serialize());
        }


        public void GetDiceNotify(IPeer peer, int playerUid, int diceId, short slotNum, short level)
        {
            MsgGetDiceNotify msg = new MsgGetDiceNotify();
            msg.PlayerUId = playerUid;
            msg.DiceId = diceId;
            msg.SlotNum = slotNum;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.GET_DICE_NOTIFY, msg.Serialize());
        }


        public void LevelUpDiceNotify(IPeer peer, int resetFieldNum, int leveupFieldNum, int levelUpDiceId, int level)
        {
            MsgLevelUpDiceNotify msg = new MsgLevelUpDiceNotify();
            msg.ResetFieldNum = resetFieldNum;
            msg.LeveupFieldNum = leveupFieldNum;
            msg.LevelupDiceId = levelUpDiceId;
            msg.Level = level;
            peer.SendPacket((short)GameProtocol.LEVEL_UP_DICE_NOTIFY, msg.Serialize());
        }


        /// <summary>
        /// sp 추가 알림
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="currentSp"></param>
        public void AddSpNotify(IPeer peer, int playerUId, int currentSp)
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
        public void SpawnNotify(IPeer peer, int wave)
        {
            MsgSpawnNotify msg = new MsgSpawnNotify();
            msg.Wave = wave;
            peer.SendPacket((short)GameProtocol.SPAWN_NOTIFY, msg.Serialize());
        }

        #endregion

        #region Relay Protocol                

        /// <summary>
        /// 미니언 제거 중계
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public void RemoveMinionRelay(IPeer peer, int id)
        {
            MsgRemoveMinionRelay msg = new MsgRemoveMinionRelay();
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
        public void HitDamageMinionRelay(IPeer peer, int id, float damage, float delay)
        {
            MsgHitDamageMinionRelay msg = new MsgHitDamageMinionRelay();
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
        public void DestroyMinionRelay(IPeer peer, int id)
        {
            MsgDestroyMinionRelay msg = new MsgDestroyMinionRelay();
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.DESTROY_MINION_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 미니언 회복 중계
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="heal"></param>
        public void HealMinionRelay(IPeer peer, int id, float heal)
        {
            MsgHealMinionRelay msg = new MsgHealMinionRelay();
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
        public void PushMinionRelay(IPeer peer, int id, float x, float y, float z, float pushPower)
        {
            MsgPushMinionRelay msg = new MsgPushMinionRelay();
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
        public void SetMinionAnimationTriggerRelay(IPeer peer, int id, string trigger)
        {
            MsgSetMinionAnimationTriggerRelay msg = new MsgSetMinionAnimationTriggerRelay();
            msg.Id = id;
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
        public void FireArrowRelay(IPeer peer, int id, float x, float y, float z, float damage)
        {
            MsgFireArrowRelay msg = new MsgFireArrowRelay();
            msg.Id = id;
            msg.Dir[0] = x;
            msg.Dir[1] = y;
            msg.Dir[2] = z;
            msg.Damage = damage;
            peer.SendPacket((short)GameProtocol.FIRE_ARROW_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public void FireballBombRelay(IPeer peer, int id)
        {
            MsgFireballBombRelay msg = new MsgFireballBombRelay();
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.FIREBALL_BOMB_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public void MineBombRelay(IPeer peer, int id)
        {
            MsgMineBombRelay msg = new MsgMineBombRelay();
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.MINE_BOMB_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 마법 제거
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        public void RemoveMagicRelay(IPeer peer, int id)
        {
            MsgRemoveMagicRelay msg = new MsgRemoveMagicRelay();
            msg.Id = id;
            peer.SendPacket((short)GameProtocol.REMOVE_MAGIC_RELAY, msg.Serialize());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        public void SetMagicTargetIdRelay(IPeer peer, int id, int targetId)
        {
            MsgSetMagicTargetIdRelay msg = new MsgSetMagicTargetIdRelay();
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
        public void SetMagicTargetRelay(IPeer peer, int id, float x, float z)
        {
            MsgSetMagicTargetRelay msg = new MsgSetMagicTargetRelay();
            msg.Id = id;
            msg.X = x;
            msg.Z = z;
            peer.SendPacket((short)GameProtocol.SET_MAGIC_TARGET_POS_RELAY, msg.Serialize());
        }

        #endregion
    }
}
