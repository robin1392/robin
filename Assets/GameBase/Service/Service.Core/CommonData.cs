using System;
using System.Collections.Generic;
using System.IO;


namespace Service.Core
{
    [Serializable]
    public class ItemBaseInfo
    {
        public int ItemId;
        public int Value;


        public void Write(BinaryWriter bw)
        {
            bw.Write(ItemId);
            bw.Write(Value);
        }

        public static ItemBaseInfo Read(BinaryReader br)
        {
            ItemBaseInfo data = new ItemBaseInfo();
            data.ItemId = br.ReadInt32();
            data.Value = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class UserBox
    {
        public int BoxId;
        public int Count;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BoxId);
            bw.Write(Count);
        }

        public static UserBox Read(BinaryReader br)
        {
            UserBox data = new UserBox();
            data.BoxId = br.ReadInt32();
            data.Count = br.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public class UserDeck
    {
        // �� �ε���
        public sbyte Index;
        // �� ����(�ֻ��� ���̵� �迭)
        public int[] DeckInfo;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Index);
            bw.Write(DeckInfo.Length);
            byte[] bytes = new byte[DeckInfo.Length * sizeof(int)];
            Buffer.BlockCopy(DeckInfo, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static UserDeck Read(BinaryReader br)
        {
            UserDeck data = new UserDeck();
            data.Index = br.ReadSByte();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.DeckInfo = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.DeckInfo[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }
            return data;
        }
    }

    [Serializable]
    public class UserDice
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

        public static UserDice Read(BinaryReader br)
        {
            UserDice data = new UserDice();
            data.DiceId = br.ReadInt32();
            data.Level = br.ReadInt16();
            data.Count = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class QuestInfo
    {
        // �ʱ�ȭ ���� �ð�(�ʴ���)
        public int RemainResetTime;
        // ����Ʈ ������
        public QuestData[] QuestData;
        // ���� ���� ����
        public QuestDayReward DayRewardInfo;
    }


    [Serializable]
    public class QuestData
    {
        public int QuestId;
        public int Value;
        public int Status;

        public void Write(BinaryWriter bw)
        {
            bw.Write(QuestId);
            bw.Write(Value);
            bw.Write(Status);
        }

        public static QuestData Read(BinaryReader br)
        {
            QuestData data = new QuestData();
            data.QuestId = br.ReadInt32();
            data.Value = br.ReadInt32();
            data.Status = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class QuestDayReward
    {
        // ���� ���̵�
        public int DayRewardId;
        // ���� ȹ�� ����
        public bool[] DayRewardState;
        // ���� ���� ȹ����� ���� �ð�(�ʴ���)
        public int DayRewardRemainTime;
    }


    [Serializable]
    public class UserSeasonInfo
    {
        // ���� ���̵�
        public int SeasonId;
        // ���� ����
        public int SeasonState;
        // ���� �н� ���� ����
        public bool BuySeasonPass;
        // ���� Ʈ����
        public int SeasonTrophy;
        // ���� �ʱ�ȭ ���� �ð�(�ʴ���)
        public int SeasonResetRemainTime;
        // ���� �н� ���� ȹ�� ���̵� �迭
        public int[] SeasonPassRewardIds;
        // ���� �н� ���� �ܰ�
        public int SeasonPassRewardStep;
        // ���� �ʱ�ȭ �ʿ俩��
        public bool NeedSeasonReset;
        // ���� ���� ����
        public bool IsFreeSeason;
        // ��ŷ ����
        public int RankPoint;
        // ��ŷ ����
        public int Rank;
    }

}