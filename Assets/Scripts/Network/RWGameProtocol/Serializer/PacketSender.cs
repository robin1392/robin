using System;
using System.Collections.Generic;
using RWCoreNetwork.NetService;

namespace RWGameProtocol.Serializer
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
        public virtual void UpgradeSpNotify(Peer peer, ushort playerUId, short upgrade) { }
        public virtual void HitDamageReq(Peer peer, ushort playerUId, int damage) { }
        public virtual void HitDamageAck(Peer peer, GameErrorCode code, ushort playerUId, int damage, int currentHp) { }
        public virtual void ReconnectGameReq(Peer peer) { }
        public virtual void ReconnectGameAck(Peer peer, GameErrorCode code, MsgPlayerBase playerBase, MsgPlayerBase otherPlayerBase) { }
        
        public virtual void ReadySyncGameReq(Peer peer) { }
        public virtual void ReadySyncGameAck(Peer peer, GameErrorCode code) { }
        public virtual void ReadySyncGameNotify(Peer peer, int playerUId) { }

        public virtual void StartSyncGameReq(Peer peer, int playerId, int playerSpawnCount, MsgSyncMinionData[] syncMinionData, int otherPlayerId, int otherPlayerSpawnCount, MsgSyncMinionData[] otherSyncMinionData) { }
        public virtual void StartSyncGameAck(Peer peer, GameErrorCode code) { }
        public virtual void StartSyncGameNotify(Peer peer, MsgPlayerInfo playerInfo, int playerSpawnCount,  MsgGameDice[] gameDiceData, MsgInGameUp[] inGameUp, MsgSyncMinionData[] syncMinionData, MsgPlayerInfo otherPlayerInfo, int otherPlayerSpawnCount, MsgGameDice[] otherGameDiceData, MsgInGameUp[] otherInGameUp, MsgSyncMinionData[] otherSyncMinionData) { }
        public virtual void EndSyncGameReq(Peer peer) { }
        public virtual void EndSyncGameAck(Peer peer, GameErrorCode code) { }
        public virtual void EndSyncGameNotify(Peer peer) { }


        public virtual void HitDamageNotify(Peer peer, ushort playerUId, int damage, int currentHp) { }
        public virtual void JoinGameNotify(Peer peer, MsgPlayerInfo info) { }
        public virtual void LeaveGameNotify(Peer peer, int playerUId) { }
        public virtual void DeactiveWaitingObjectNotify(Peer peer, ushort playerUId, int currentSp) { }
        public virtual void GetDiceNotify(Peer peer, ushort playerUId, int diceId, short slotNum, short level) { }
        public virtual void LevelUpDiceNotify(Peer peer, ushort playerUId, short resetFieldNum, short leveupFieldNum, int levelUpDiceId, short level) { }
        public virtual void InGameUpDiceNotify(Peer peer, ushort playerUId, int diceId, short inGameUp) { }
        public virtual void AddSpNotify(Peer peer, ushort playerUId, int currentSp) { }
        public virtual void SpawnNotify(Peer peer, int wave) { }
        public virtual void EndGameNotify(Peer peer, GameErrorCode code, int winPlayerUId) { }
        public virtual void DisconnectGameNotify(Peer peer, int playerUId) { }
        public virtual void ReconnectGameNotify(Peer peer, int playerUId) { }
        public virtual void PauseGameNotify(Peer peer, int playerUId) { }
        public virtual void ResumeGameNotify(Peer peer, int playerUId) { }


        public virtual void HitDamageMinionRelay(Peer peer, ushort playerUId, ushort id, int damage) { }
        public virtual void DestroyMinionRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void HealMinionRelay(Peer peer, ushort playerUId, ushort id, int heal) { }
        public virtual void PushMinionRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, short pushPower) { }
        public virtual void SetMinionAnimationTriggerRelay(Peer peer, ushort playerUId, ushort id, ushort targetId, byte trigger) { }
        public virtual void FireArrowRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, int damage, short moveSpeed) { }
        public virtual void FireballBombRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void MineBombRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void SetMagicTargetIdRelay(Peer peer, ushort playerUId, ushort id, ushort targetId) { }
        public virtual void SetMagicTargetRelay(Peer peer, ushort playerUId, ushort id, short x, short z) { }
        public virtual void SturnMinionRelay(Peer peer, ushort playerUId, ushort id, short sturnTime) { }
        public virtual void RocketBombRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void IceBombRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void MsgDestroyMagic(Peer peer, ushort playerUId, ushort baseStatId) { }
        public virtual void MsgFireCannonBall(Peer peer, ushort playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, short range, byte type) { }
        public virtual void FireSpearRelay(Peer peer, ushort playerUId, MsgVector3 shootPos, ushort targetId, int power, short moveSpeed) { }
        public virtual void FireManFireRelay(Peer peer, ushort playerUId, ushort id) { }
        public virtual void ActivatePoolObjectRelay(Peer peer, int poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation) { }
        public virtual void MinionCloackingRelay(Peer peer, ushort playerUId, ushort id, bool isCloacking) { }
        public virtual void MinionFogOfWarRelay(Peer peer, ushort playerUId, ushort baseStatId, short effect, bool isFogOfWar) { }
        public virtual void SendMessageVoidRelay(Peer peer, ushort playerUId, ushort id, byte message) { }
        public virtual void SendMessageParam1Relay(Peer peer, ushort playerUId, ushort id, ushort targetId, byte message) { }
        public virtual void NecromancerBulletRelay(Peer peer, ushort playerUId, MsgVector3 shootPos, ushort targetId, int power, short bulletMoveSpeed) { }
        public virtual void SetMinionTargetRelay(Peer peer, ushort playerUId, ushort id, ushort targetId) { }
        public virtual void MinionStatusRelay(Peer peer, ushort playerUId, byte posIndex, MsgVector3[] pos, int[] hp, MsgMinionStatus relay, int packetCount) { }
        public virtual void ScarecrowRelay(Peer peer, ushort playerUId, ushort baseStatId, byte eyeLevel) { }
        public virtual void LayzerTargetRelay(Peer peer, ushort playerUId, ushort id, ushort[] targetId) { }
        public virtual void FireBulletRelay(Peer peer, ushort playerUId, ushort id, MsgVector3 pos, int damage, short moveSpeedk, byte type) { }
        public virtual void MinionInvincibilityRelay(Peer peer, ushort playerUId, ushort id, short time) { }


    }
}
