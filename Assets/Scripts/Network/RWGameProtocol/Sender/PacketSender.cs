﻿using System;
using System.Collections.Generic;
using RWCoreNetwork;
using RWGameProtocol.Msg;


namespace RWGameProtocol
{
    public class PacketSender
    {
        public virtual void JoinGameReq(Peer peer, string playerSessionId, sbyte deckIndex) { }
        public virtual void JoinGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo) { }
        public virtual void LeaveGameReq(Peer peer) { }
        public virtual void LeaveGameAck(Peer peer, GameErrorCode code) { }
        public virtual void ReadyGameReq(Peer peer) { }
        public virtual void ReadyGameAck(Peer peer, GameErrorCode code) { }
        public virtual void GetDiceReq(Peer peer) { }
        public virtual void GetDiceAck(Peer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp) { }
        public virtual void LevelUpDiceReq(Peer peer, short resetFieldNum, short leveupFieldNum) { }
        public virtual void LevelUpDiceAck(Peer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) { }
        public virtual void InGameUpDiceReq(Peer peer, int diceId) { }
        public virtual void InGameUpDiceAck(Peer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp) { }
        public virtual void UpgradeSpReq(Peer peer) { }
        public virtual void UpgradeSpAck(Peer peer, GameErrorCode code, short upgrade, int currentSp) { }
        public virtual void UpgradeSpNotify(Peer peer, int playerUId, short upgrade) { }
        public virtual void HitDamageReq(Peer peer, int damage) { }
        public virtual void HitDamageAck(Peer peer, GameErrorCode code, int damage) { }
        public virtual void ReconnectGameReq(Peer peer, int playerUId) { }
        public virtual void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerInfo playerInfo, int wave) { }


        public virtual void HitDamageNotify(Peer peer, int playerUId, int damage) { }
        public virtual void JoinGameNotify(Peer peer, MsgPlayerInfo info) { }
        public virtual void LeaveGameNotify(Peer peer, int playerUId) { }
        public virtual void DeactiveWaitingObjectNotify(Peer peer, int playerUid, int currentSp) { }
        public virtual void GetDiceNotify(Peer peer, int playerUid, int diceId, short slotNum, short level) { }
        public virtual void LevelUpDiceNotify(Peer peer, int playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) { }
        public virtual void InGameUpDiceNotify(Peer peer, int playerUId, int diceId, short inGameUp) { }
        public virtual void AddSpNotify(Peer peer, int playerUId, int currentSp) { }
        public virtual void SpawnNotify(Peer peer, int wave) { }
        public virtual void EndGameNotify(Peer peer, int winPlayerUId) { }
        public virtual void DisconnectGameNotify(Peer peer, int playerUId) { }
        public virtual void ReconnectGameNotify(Peer peer, int playerUId) { }

        public virtual void RemoveMinionRelay(Peer peer, int playerUId, int id) { }
        public virtual void HitDamageMinionRelay(Peer peer, int playerUId, int id, int damage, int delay) { }
        public virtual void DestroyMinionRelay(Peer peer, int playerUId, int id) { }
        public virtual void HealMinionRelay(Peer peer, int playerUId, int id, int heal) { }
        public virtual void PushMinionRelay(Peer peer, int playerUId, int id, int x, int y, int z, int pushPower) { }
        public virtual void SetMinionAnimationTriggerRelay(Peer peer, int playerUId, int id, string trigger) { }
        public virtual void FireArrowRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeed) { }
        public virtual void FireballBombRelay(Peer peer, int playerUId, int id) { }
        public virtual void MineBombRelay(Peer peer, int playerUId, int id) { }
        public virtual void RemoveMagicRelay(Peer peer, int playerUId, int id) { }
        public virtual void SetMagicTargetIdRelay(Peer peer, int playerUId, int id, int targetId) { }
        public virtual void SetMagicTargetRelay(Peer peer, int playerUId, int id, int x, int z) { }
        public virtual void SturnMinionRelay(Peer peer, int playerUId, int id, int sturnTime) { }
        public virtual void RocketBombRelay(Peer peer, int playerUId, int id) { }
        public virtual void IceBombRelay(Peer peer, int playerUId, int id) { }
        public virtual void MsgDestroyMagic(Peer peer, int playerUId, int baseStatId) { }
        public virtual void MsgFireCannonBall(Peer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range) { }
        public virtual void FireSpearRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int moveSpeed) { }
        public virtual void FireManFireRelay(Peer peer, int playerUId, int id) { }
        public virtual void ActivatePoolObjectRelay(Peer peer, int playerUId, string poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation) { }
        public virtual void MinionCloackingRelay(Peer peer, int playerUId, int id, bool isCloacking) { }
        public virtual void MinionFogOfWarRelay(Peer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar) { }
        public virtual void SendMessageVoidRelay(Peer peer, int playerUId, int id, string message) { }
        public virtual void SendMessageParam1Relay(Peer peer, int playerUId, int id, int targetId, string message) { }
        public virtual void NecromancerBulletRelay(Peer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed) { }
        public virtual void SetMinionTargetRelay(Peer peer, int playerUId, int id, int targetId) { }
        public virtual void MinionStatusRelay(Peer peer, int playerUId, byte posIndex, MsgVector3[] pos) { }
        public virtual void ScarecrowRelay(Peer peer, int playerUId, int baseStatId, int eyeLevel) { }
        public virtual void LayzerTargetRelay(Peer peer, int playerUId, int id, int[] targetId) { }

    }
}