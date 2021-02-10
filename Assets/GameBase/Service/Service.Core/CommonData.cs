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
        // 덱 인덱스
        public sbyte Index;
        // 덱 정보(주사위 아이디 배열)
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
        // 초기화 남은 시간(초단위)
        public int RemainResetTime;
        // 퀘스트 데이터
        public MsgQuestData[] QuestData;
        // 일일 보상 정보
        public MsgQuestDayReward DayRewardInfo;
    }


    [Serializable]
    public class MsgQuestDayReward
    {
        // 보상 아이디
        public int DayRewardId;
        // 보상 획득 여부
        public bool[] DayRewardState;
        // 일일 보상 획득까지 남은 시간(초단위)
        public int DayRewardRemainTime;
    }


    [Serializable]
    public class MsgSeasonInfo
    {
        // 시즌 아이디
        public int SeasonId;
        // 시즌 상태
        public int SeasonState;
        // 시즌 패스 구매 여부
        public bool BuySeasonPass;
        // 시즌 트로피
        public int SeasonTrophy;
        // 시즌 초기화 남은 시간(초단위)
        public int SeasonResetRemainTime;
        // 시즌 패스 보상 획득 아이디 배열
        public int[] SeasonPassRewardIds;
        // 시즌 패스 보상 단계
        public int SeasonPassRewardStep;
        // 시즌 초기화 필요여부
        public bool NeedSeasonReset;
        // 프리 시즌 여부
        public bool IsFreeSeason;
        // 랭킹 점수
        public int RankPoint;
        // 랭킹 순위
        public int Rank;
    }

}