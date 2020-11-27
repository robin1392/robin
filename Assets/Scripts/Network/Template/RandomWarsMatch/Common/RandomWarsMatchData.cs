using System;
using System.IO;

namespace Template.Stage.RandomWarsMatch.Common
{
    [Serializable]
    public class MsgPlayerInfo
    {
        public ushort PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public int Trophy;
        public short SpGrade;
        public short GetDiceCount;
        public MsgUserDice[] DiceInfo;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(Name);
            bw.Write(CurrentSp);
            bw.Write(TowerHp);
            bw.Write(Trophy);
            bw.Write(SpGrade);
            bw.Write(GetDiceCount);

            int length = (DiceInfo == null) ? 0 : DiceInfo.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                DiceInfo[i].Write(bw);
            }
        }

        public static MsgPlayerInfo Read(BinaryReader br)
        {
            MsgPlayerInfo data = new MsgPlayerInfo();
            data.PlayerUId = br.ReadUInt16();
            data.IsBottomPlayer = br.ReadBoolean();
            data.Name = br.ReadString();
            data.CurrentSp = br.ReadInt32();
            data.TowerHp = br.ReadInt32();
            data.Trophy = br.ReadInt32();
            data.SpGrade = br.ReadInt16();
            data.GetDiceCount = br.ReadInt16();

            int length = br.ReadInt32();
            data.DiceInfo = new MsgUserDice[length];
            for (int i = 0; i < length; i++)
            {
                data.DiceInfo[i] = MsgUserDice.Read(br);
            }
            return data;
        }
    }

    
    [Serializable]
    public class MsgUserDice
    {
        public int DiceId;
        public short Level;
        public short Count;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(Level);
            bw.Write(Count);
        }

        public static MsgUserDice Read(BinaryReader br)
        {
            MsgUserDice data = new MsgUserDice();
            data.DiceId = br.ReadInt32();
            data.Level = br.ReadInt16();
            data.Count = br.ReadInt16();
            return data;
        }
    }
}