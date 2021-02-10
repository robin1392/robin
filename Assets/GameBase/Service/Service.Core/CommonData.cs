using System;
using System.Collections.Generic;
using System.IO;


namespace Service.Core
{
    [Serializable]
    public class MsgReward
    {
        public int ItemId;
        public int Value;


        public void Write(BinaryWriter bw)
        {
            bw.Write(ItemId);
            bw.Write(Value);
        }

        public static MsgReward Read(BinaryReader br)
        {
            MsgReward data = new MsgReward();
            data.ItemId = br.ReadInt32();
            data.Value = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgRewardMultiple
    {
        public int ItemId;
        public MsgReward[] arrayReward;
    }


    [Serializable]
    public class MsgQuestData
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

        public static MsgQuestData Read(BinaryReader br)
        {
            MsgQuestData data = new MsgQuestData();
            data.QuestId = br.ReadInt32();
            data.Value = br.ReadInt32();
            data.Status = br.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public class MsgUserBox
    {
        public int BoxId;
        public int Count;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BoxId);
            bw.Write(Count);
        }

        public static MsgUserBox Read(BinaryReader br)
        {
            MsgUserBox data = new MsgUserBox();
            data.BoxId = br.ReadInt32();
            data.Count = br.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public class MsgUserDeck
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

        public static MsgUserDeck Read(BinaryReader br)
        {
            MsgUserDeck data = new MsgUserDeck();
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


    [Serializable]
    public class MsgQuestInfo
    {
        // �ʱ�ȭ ���� �ð�(�ʴ���)
        public int RemainResetTime;
        // ����Ʈ ������
        public MsgQuestData[] QuestData;
        // ���� ���� ����
        public MsgQuestDayReward DayRewardInfo;
    }


    [Serializable]
    public class MsgQuestDayReward
    {
        // ���� ���̵�
        public int DayRewardId;
        // ���� ȹ�� ����
        public bool[] DayRewardState;
        // ���� ���� ȹ����� ���� �ð�(�ʴ���)
        public int DayRewardRemainTime;
    }


    [Serializable]
    public class MsgSeasonInfo
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