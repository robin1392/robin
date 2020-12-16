using System;
using System.IO;

namespace Template.Stage.RandomWarsMatch.Common
{
    [Serializable]
    public class MsgPlayerBase
    {
        public ushort PlayerUId;
        public bool IsBottomPlayer;
        public string Name;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(Name);
        }

        public static MsgPlayerBase Read(BinaryReader br)
        {
            MsgPlayerBase data = new MsgPlayerBase();
            data.PlayerUId = br.ReadUInt16();
            data.IsBottomPlayer = br.ReadBoolean();
            data.Name = br.ReadString();
            return data;
        }
    }


    [Serializable]
    public class MsgPlayerInfo
    {
        public ushort PlayerUId;
        public bool IsBottomPlayer;
        public bool IsMaster;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public int Trophy;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceLevelArray;


        public MsgPlayerInfo()
        {
            PlayerUId = 0;
            IsBottomPlayer = false;
            IsMaster = false;
            Name = string.Empty;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(IsMaster);
            bw.Write(Name);
            bw.Write(CurrentSp);
            bw.Write(TowerHp);
            bw.Write(Trophy);
            bw.Write(SpGrade);
            bw.Write(GetDiceCount);

            bw.Write(DiceIdArray.Length);
            byte[] bytes = new byte[DiceIdArray.Length * sizeof(int)];
            Buffer.BlockCopy(DiceIdArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);

            bw.Write(DiceLevelArray.Length);
            bytes = new byte[DiceLevelArray.Length * sizeof(short)];
            Buffer.BlockCopy(DiceLevelArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgPlayerInfo Read(BinaryReader br)
        {
            MsgPlayerInfo data = new MsgPlayerInfo();
            data.PlayerUId = br.ReadUInt16();
            data.IsBottomPlayer = br.ReadBoolean();
            data.IsMaster = br.ReadBoolean();
            data.Name = br.ReadString();
            data.CurrentSp = br.ReadInt32();
            data.TowerHp = br.ReadInt32();
            data.Trophy = br.ReadInt32();
            data.SpGrade = br.ReadInt16();
            data.GetDiceCount = br.ReadInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.DiceIdArray = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.DiceIdArray[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }

            length = br.ReadInt32();
            bytes = br.ReadBytes(length * sizeof(short));
            data.DiceLevelArray = new short[length];
            for (var index = 0; index < length; index++)
            {
                data.DiceLevelArray[index] = BitConverter.ToInt16(bytes, index * sizeof(short));
            }
            return data;
        }
    }

    
    [Serializable]
    public class MsgGameDice
    {
        public int DiceId;
        public short Level;
        

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(Level);
        }

        public static MsgGameDice Read(BinaryReader br)
        {
            MsgGameDice data = new MsgGameDice();
            data.DiceId = br.ReadInt32();
            data.Level = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgMonster
    {
        public ushort Id;
        public int DataId;
        public int Hp;
        public short SkillCoolTime;
        public short SkillInterval;
        public short Atk;
        public short SkillAtk;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(DataId);
            bw.Write(Hp);
            bw.Write(SkillCoolTime);
            bw.Write(SkillInterval);
            bw.Write(Atk);
            bw.Write(SkillAtk);
        }

        public static MsgMonster Read(BinaryReader br)
        {
            MsgMonster data = new MsgMonster();
            data.Id = br.ReadUInt16();
            data.DataId = br.ReadInt32();
            data.Hp = br.ReadInt32();
            data.SkillCoolTime = br.ReadInt16();
            data.SkillInterval = br.ReadInt16();
            data.Atk = br.ReadInt16();
            data.SkillAtk = br.ReadInt16();
            return data;
        }
    }

    [Serializable]
    public class MsgSpawnInfo
    {
        public ushort PlayerUId;
        public MsgSpawnMinion[] SpawnMinion;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);

            int length = (SpawnMinion == null) ? 0 : SpawnMinion.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                SpawnMinion[i].Write(bw);
            }
        }

        public static MsgSpawnInfo Read(BinaryReader br)
        {
            MsgSpawnInfo data = new MsgSpawnInfo();
            data.PlayerUId = br.ReadUInt16();

            int length = br.ReadInt32();
            data.SpawnMinion = new MsgSpawnMinion[length];
            for (int i = 0; i < length; i++)
            {
                data.SpawnMinion[i] = MsgSpawnMinion.Read(br);
            }

            return data;
        }
    }

    [Serializable]
    public class MsgSpawnMinion
    {
        public byte SlotIndex;
        public ushort DiceId;
        public short DiceLevel;
        public short DiceInGameUp;
        public ushort[] Id;


        public void Write(BinaryWriter bw)
        {
            bw.Write(SlotIndex);
            bw.Write(DiceId);
            bw.Write(DiceLevel);
            bw.Write(DiceInGameUp);

            bw.Write(Id.Length);
            byte[] bytes = new byte[Id.Length * sizeof(ushort)];
            Buffer.BlockCopy(Id, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgSpawnMinion Read(BinaryReader br)
        {
            MsgSpawnMinion data = new MsgSpawnMinion();
            data.SlotIndex = br.ReadByte();
            data.DiceId = br.ReadUInt16();
            data.DiceLevel = br.ReadInt16();
            data.DiceInGameUp = br.ReadInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(ushort));
            data.Id = new ushort[length];
            for (var index = 0; index < length; index++)
            {
                data.Id[index] = BitConverter.ToUInt16(bytes, index * sizeof(ushort));
            }

            return data;
        }
    }
}