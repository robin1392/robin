using System;
using System.Collections.Generic;
using RWCoreNetwork;
using RWGameProtocol.Msg;


namespace RWGameProtocol
{
    public class PacketSender
    {
        public virtual void JoinGameReq(IPeer peer, string playerSessionId, sbyte deckIndex) { }
        public virtual void JoinGameAck(IPeer peer, GameErrorCode code, MsgPlayerInfo playerInfo) { }
        public virtual void LeaveGameReq(IPeer peer) { }
        public virtual void LeaveGameAck(IPeer peer, GameErrorCode code) { }
        public virtual void ReadyGameReq(IPeer peer) { }
        public virtual void ReadyGameAck(IPeer peer, GameErrorCode code) { }
        public virtual void GetDiceReq(IPeer peer) { }
        public virtual void GetDiceAck(IPeer peer, GameErrorCode code, int diceId, short slotNum, short level, int currentSp) { }
        public virtual void LevelUpDiceReq(IPeer peer, short resetFieldNum, short leveupFieldNum) { }
        public virtual void LevelUpDiceAck(IPeer peer, GameErrorCode code, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) { }
        public virtual void InGameUpDiceReq(IPeer peer, int diceId) { }
        public virtual void InGameUpDiceAck(IPeer peer, GameErrorCode code, int diceId, short inGameUp, int currentSp) { }
        public virtual void UpgradeSpReq(IPeer peer) { }
        public virtual void UpgradeSpAck(IPeer peer, GameErrorCode code, short upgrade, int currentSp) { }
        public virtual void UpgradeSpNotify(IPeer peer, int playerUId, short upgrade) { }
        public virtual void HitDamageReq(IPeer peer, int damage) { }
        public virtual void HitDamageAck(IPeer peer, GameErrorCode code, int damage) { }
        public virtual void ReconnectGameReq(IPeer peer, int playerUId) { }
        public virtual void ReconnectGameAck(IPeer peer, GameErrorCode code, MsgPlayerInfo playerInfo, int wave) { }


        public virtual void HitDamageNotify(IPeer peer, int playerUId, int damage) { }
        public virtual void JoinGameNotify(IPeer peer, MsgPlayerInfo info) { }
        public virtual void LeaveGameNotify(IPeer peer, int playerUId) { }
        public virtual void DeactiveWaitingObjectNotify(IPeer peer, int playerUid, int currentSp) { }
        public virtual void GetDiceNotify(IPeer peer, int playerUid, int diceId, short slotNum, short level) { }
        public virtual void LevelUpDiceNotify(IPeer peer, int playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) { }
        public virtual void InGameUpDiceNotify(IPeer peer, int playerUId, int diceId, short inGameUp) { }
        public virtual void AddSpNotify(IPeer peer, int playerUId, int currentSp) { }
        public virtual void SpawnNotify(IPeer peer, int wave) { }
        public virtual void EndGameNotify(IPeer peer, int winPlayerUId) { }
        public virtual void DisconnectGameNotify(IPeer peer, int playerUId) { }
        public virtual void ReconnectGameNotify(IPeer peer, int playerUId) { }

        public virtual void RemoveMinionRelay(IPeer peer, int playerUId, int id) { }
        public virtual void HitDamageMinionRelay(IPeer peer, int playerUId, int id, int damage, int delay) { }
        public virtual void DestroyMinionRelay(IPeer peer, int playerUId, int id) { }
        public virtual void HealMinionRelay(IPeer peer, int playerUId, int id, int heal) { }
        public virtual void PushMinionRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int pushPower) { }
        public virtual void SetMinionAnimationTriggerRelay(IPeer peer, int playerUId, int id, string trigger) { }
        public virtual void FireArrowRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeed) { }
        public virtual void FireballBombRelay(IPeer peer, int playerUId, int id) { }
        public virtual void MineBombRelay(IPeer peer, int playerUId, int id) { }
        public virtual void RemoveMagicRelay(IPeer peer, int playerUId, int id) { }
        public virtual void SetMagicTargetIdRelay(IPeer peer, int playerUId, int id, int targetId) { }
        public virtual void SetMagicTargetRelay(IPeer peer, int playerUId, int id, int x, int z) { }
        public virtual void SturnMinionRelay(IPeer peer, int playerUId, int id, int sturnTime) { }
        public virtual void RocketBombRelay(IPeer peer, int playerUId, int id) { }
        public virtual void IceBombRelay(IPeer peer, int playerUId, int id) { }
        public virtual void MsgDestroyMagic(IPeer peer, int playerUId, int baseStatId) { }
        public virtual void MsgFireCannonBall(IPeer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range) { }
        public virtual void FireSpearRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int moveSpeed) { }
        public virtual void FireManFireRelay(IPeer peer, int playerUId, int id) { }
        public virtual void ActivatePoolObjectRelay(IPeer peer, int playerUId, string poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation) { }
        public virtual void MinionCloackingRelay(IPeer peer, int playerUId, int id, bool isCloacking) { }
        public virtual void MinionFogOfWarRelay(IPeer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar) { }
        public virtual void SendMessageVoidRelay(IPeer peer, int playerUId, int id, string message) { }
        public virtual void SendMessageParam1Relay(IPeer peer, int playerUId, int id, int targetId, string message) { }
        public virtual void NecromancerBulletRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed) { }
        public virtual void SetMinionTargetRelay(IPeer peer, int playerUId, int id, int targetId) { }
        public virtual void MinionStatusRelay(IPeer peer, int playerUId, byte posIndex, MsgVector3[] pos) { }
        public virtual void ScarecrowRelay(IPeer peer, int playerUId, int baseStatId, int eyeLevel) { }
        public virtual void LayzerTargetRelay(IPeer peer, int playerUId, int id, int[] targetId) { }

    }
}
