using System;
using System.IO;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public struct MsgSyncMinionData
    {
        public int minionId;
        public int minionDataId;
        public int minionHp;
        public int minionMaxHp;
        public int minionPower;
        public int minionEffect;
        public int minionEffectUpgrade;
        public int minionEffectIngameUpgrade;
        public int minionDuration;
        public int minionCooltime;
        public MsgVector3 minionPos;

        public void Write(BinaryWriter bw)
        {
            bw.Write(minionId);
            bw.Write(minionDataId);
            bw.Write(minionHp);
            bw.Write(minionMaxHp);
            bw.Write(minionPower);
            bw.Write(minionEffect);
            bw.Write(minionEffectUpgrade);
            bw.Write(minionEffectIngameUpgrade);
            bw.Write(minionDuration);
            bw.Write(minionCooltime);
            minionPos.Write(bw);
        }

        public void Read(BinaryReader br)
        {
            minionId = br.ReadInt32();
            minionDataId = br.ReadInt32();
            minionHp = br.ReadInt32();
            minionMaxHp = br.ReadInt32();
            minionPower = br.ReadInt32();
            minionEffect = br.ReadInt32();
            minionEffectUpgrade = br.ReadInt32();
            minionEffectIngameUpgrade = br.ReadInt32();
            minionDuration = br.ReadInt32();
            minionCooltime = br.ReadInt32();
            minionPos = MsgVector3.Read(br);
        }
    }

    [Serializable]
    public struct MsgGameDice
    {
        public int DiceId;
        public short SlotNum;
        public short Level;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(SlotNum);
            bw.Write(Level);
        }

        public void Read(BinaryReader br)
        {
            DiceId = br.ReadInt32();
            SlotNum = br.ReadInt16();
            Level = br.ReadInt16();
        }
    }


    [Serializable]
    public struct MsgInGameUp
    {
        public int DiceId;
        public short Grade;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(Grade);
        }

        public void Read(BinaryReader br)
        {
            DiceId = br.ReadInt32();
            Grade = br.ReadInt16();
        }
    }


    [Serializable]
    public class MsgStartSyncGameReq
    {
        public int PlayerId;
        public int PlayerSpawnCount;
        public MsgSyncMinionData[] SyncMinionData;
        
        public int OtherPlayerId;
        public int OtherPlayerSpawnCount;
        public MsgSyncMinionData[] OtherSyncMinionData;
    }


    [Serializable]
    public class MsgStartSyncGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgStartSyncGameNotify
    {
        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
        public MsgGameDice[] GameDiceData;
        public MsgInGameUp[] InGameUp;
        public MsgSyncMinionData[] SyncMinionData;
        public int PlayerSpawnCount;

        // 상대방 정보
        public MsgPlayerInfo OtherPlayerInfo;
        public MsgGameDice[] OtherGameDiceData;
        public MsgInGameUp[] OtherInGameUp;
        public MsgSyncMinionData[] OtherSyncMinionData;
        public int OtherPlayerSpawnCount;
    }
}
