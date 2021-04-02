using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service.Template
{
	public class ItemBaseInfo
	{
		// ItemId
		public int ItemId;
		// ItemLevel
		public int ItemLevel;
		// Value
		public int Value;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(ItemId);
			bw.Write(ItemLevel);
			bw.Write(Value);
		}

		public static ItemBaseInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new ItemBaseInfo();
			data.ItemId = br.ReadInt32();
			data.ItemLevel = br.ReadInt32();
			data.Value = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("ItemId", ItemId);
			json.Add("ItemLevel", ItemLevel);
			json.Add("Value", Value);
			return json.ToString();
		}

		public static ItemBaseInfo JsonDeserialize(JObject json)
		{
			var data = new ItemBaseInfo();
			data.ItemId = (int)json["ItemId"];
			data.ItemLevel = (int)json["ItemLevel"];
			data.Value = (int)json["Value"];
			return data;
		}

	}

	public class AdRewardInfo
	{
		// RewardId
		public string RewardId;
		// ItemId
		public int ItemId;
		// Value
		public int Value;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(RewardId);
			bw.Write(ItemId);
			bw.Write(Value);
		}

		public static AdRewardInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new AdRewardInfo();
			data.RewardId = br.ReadString();
			data.ItemId = br.ReadInt32();
			data.Value = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("RewardId", RewardId);
			json.Add("ItemId", ItemId);
			json.Add("Value", Value);
			return json.ToString();
		}

		public static AdRewardInfo JsonDeserialize(JObject json)
		{
			var data = new AdRewardInfo();
			data.RewardId = (string)json["RewardId"];
			data.ItemId = (int)json["ItemId"];
			data.Value = (int)json["Value"];
			return data;
		}

	}

	public class UserBox
	{
		// BoxId
		public int BoxId;
		// Count
		public int Count;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(BoxId);
			bw.Write(Count);
		}

		public static UserBox BinaryDeserialize(BinaryReader br)
		{
			var data = new UserBox();
			data.BoxId = br.ReadInt32();
			data.Count = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("BoxId", BoxId);
			json.Add("Count", Count);
			return json.ToString();
		}

		public static UserBox JsonDeserialize(JObject json)
		{
			var data = new UserBox();
			data.BoxId = (int)json["BoxId"];
			data.Count = (int)json["Count"];
			return data;
		}

	}

	public class UserDeck
	{
		// Index
		public byte Index;
		// DeckInfo
		public List<int> DeckInfo;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(Index);
			int lengthDeckInfo = (DeckInfo == null) ? 0 : DeckInfo.Count;
			bw.Write(lengthDeckInfo);
			for (int i = 0; i < lengthDeckInfo; i++)
				bw.Write(DeckInfo[i]);
		}

		public static UserDeck BinaryDeserialize(BinaryReader br)
		{
			var data = new UserDeck();
			data.Index = br.ReadByte();
			int lengthDeckInfo = br.ReadInt32();
			data.DeckInfo = new List<int>(lengthDeckInfo);
			for (int i = 0; i < lengthDeckInfo; i++)
				data.DeckInfo.Add(br.ReadInt32());
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("Index", Index);
			json.Add("DeckInfo", JsonConvert.SerializeObject(DeckInfo));
			return json.ToString();
		}

		public static UserDeck JsonDeserialize(JObject json)
		{
			var data = new UserDeck();
			data.Index = (byte)json["Index"];
			data.DeckInfo = JsonConvert.DeserializeObject<List<int>>(json["DeckInfo"].ToString());
			return data;
		}

	}

	public class UserDice
	{
		// DiceId
		public int DiceId;
		// Level
		public short Level;
		// Count
		public short Count;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(DiceId);
			bw.Write(Level);
			bw.Write(Count);
		}

		public static UserDice BinaryDeserialize(BinaryReader br)
		{
			var data = new UserDice();
			data.DiceId = br.ReadInt32();
			data.Level = br.ReadInt16();
			data.Count = br.ReadInt16();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("DiceId", DiceId);
			json.Add("Level", Level);
			json.Add("Count", Count);
			return json.ToString();
		}

		public static UserDice JsonDeserialize(JObject json)
		{
			var data = new UserDice();
			data.DiceId = (int)json["DiceId"];
			data.Level = (short)json["Level"];
			data.Count = (short)json["Count"];
			return data;
		}

	}

	public class QuestInfo
	{
		// RemainResetTime
		public int RemainResetTime;
		// listQuestData
		public List<QuestData> listQuestData;
		// DayRewardInfo
		public QuestDayReward DayRewardInfo;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(RemainResetTime);
			int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
			bw.Write(lengthlistQuestData);
			for (int i = 0; i < lengthlistQuestData; i++)
				listQuestData[i].BinarySerialize(bw);
			DayRewardInfo.BinarySerialize(bw);
		}

		public static QuestInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new QuestInfo();
			data.RemainResetTime = br.ReadInt32();
			int lengthlistQuestData = br.ReadInt32();
			data.listQuestData = new List<QuestData>(lengthlistQuestData);
			for (int i = 0; i < lengthlistQuestData; i++)
				data.listQuestData.Add(QuestData.BinaryDeserialize(br));
				data.DayRewardInfo = QuestDayReward.BinaryDeserialize(br);
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("RemainResetTime", RemainResetTime);
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			json.Add("DayRewardInfo", DayRewardInfo.JsonSerialize());
			return json.ToString();
		}

		public static QuestInfo JsonDeserialize(JObject json)
		{
			var data = new QuestInfo();
			data.RemainResetTime = (int)json["RemainResetTime"];
			data.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(json["listQuestData"].ToString());
				data.DayRewardInfo = QuestDayReward.JsonDeserialize((JObject)json["DayRewardInfo"]);
			return data;
		}

	}

	public class QuestData
	{
		// QuestId
		public int QuestId;
		// Value
		public int Value;
		// Status
		public int Status;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(QuestId);
			bw.Write(Value);
			bw.Write(Status);
		}

		public static QuestData BinaryDeserialize(BinaryReader br)
		{
			var data = new QuestData();
			data.QuestId = br.ReadInt32();
			data.Value = br.ReadInt32();
			data.Status = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("QuestId", QuestId);
			json.Add("Value", Value);
			json.Add("Status", Status);
			return json.ToString();
		}

		public static QuestData JsonDeserialize(JObject json)
		{
			var data = new QuestData();
			data.QuestId = (int)json["QuestId"];
			data.Value = (int)json["Value"];
			data.Status = (int)json["Status"];
			return data;
		}

	}

	public class QuestDayReward
	{
		// DayRewardId
		public int DayRewardId;
		// DayRewardState
		public List<bool> DayRewardState;
		// DayRewardRemainTime
		public int DayRewardRemainTime;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(DayRewardId);
			int lengthDayRewardState = (DayRewardState == null) ? 0 : DayRewardState.Count;
			bw.Write(lengthDayRewardState);
			for (int i = 0; i < lengthDayRewardState; i++)
				bw.Write(DayRewardState[i]);
			bw.Write(DayRewardRemainTime);
		}

		public static QuestDayReward BinaryDeserialize(BinaryReader br)
		{
			var data = new QuestDayReward();
			data.DayRewardId = br.ReadInt32();
			int lengthDayRewardState = br.ReadInt32();
			data.DayRewardState = new List<bool>(lengthDayRewardState);
			for (int i = 0; i < lengthDayRewardState; i++)
				data.DayRewardState.Add(br.ReadBoolean());
			data.DayRewardRemainTime = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("DayRewardId", DayRewardId);
			json.Add("DayRewardState", JsonConvert.SerializeObject(DayRewardState));
			json.Add("DayRewardRemainTime", DayRewardRemainTime);
			return json.ToString();
		}

		public static QuestDayReward JsonDeserialize(JObject json)
		{
			var data = new QuestDayReward();
			data.DayRewardId = (int)json["DayRewardId"];
			data.DayRewardState = JsonConvert.DeserializeObject<List<bool>>(json["DayRewardState"].ToString());
			data.DayRewardRemainTime = (int)json["DayRewardRemainTime"];
			return data;
		}

	}

	public class UserSeasonInfo
	{
		// SeasonId
		public int SeasonId;
		// SeasonState
		public int SeasonState;
		// BuySeasonPass
		public bool BuySeasonPass;
		// SeasonTrophy
		public int SeasonTrophy;
		// SeasonResetRemainTime
		public int SeasonResetRemainTime;
		// SeasonPassRewardIds
		public List<int> SeasonPassRewardIds;
		// SeasonPassRewardStep
		public int SeasonPassRewardStep;
		// NeedSeasonReset
		public bool NeedSeasonReset;
		// IsFreeSeason
		public bool IsFreeSeason;
		// RankPoint
		public int RankPoint;
		// Rank
		public int Rank;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(SeasonId);
			bw.Write(SeasonState);
			bw.Write(BuySeasonPass);
			bw.Write(SeasonTrophy);
			bw.Write(SeasonResetRemainTime);
			int lengthSeasonPassRewardIds = (SeasonPassRewardIds == null) ? 0 : SeasonPassRewardIds.Count;
			bw.Write(lengthSeasonPassRewardIds);
			for (int i = 0; i < lengthSeasonPassRewardIds; i++)
				bw.Write(SeasonPassRewardIds[i]);
			bw.Write(SeasonPassRewardStep);
			bw.Write(NeedSeasonReset);
			bw.Write(IsFreeSeason);
			bw.Write(RankPoint);
			bw.Write(Rank);
		}

		public static UserSeasonInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new UserSeasonInfo();
			data.SeasonId = br.ReadInt32();
			data.SeasonState = br.ReadInt32();
			data.BuySeasonPass = br.ReadBoolean();
			data.SeasonTrophy = br.ReadInt32();
			data.SeasonResetRemainTime = br.ReadInt32();
			int lengthSeasonPassRewardIds = br.ReadInt32();
			data.SeasonPassRewardIds = new List<int>(lengthSeasonPassRewardIds);
			for (int i = 0; i < lengthSeasonPassRewardIds; i++)
				data.SeasonPassRewardIds.Add(br.ReadInt32());
			data.SeasonPassRewardStep = br.ReadInt32();
			data.NeedSeasonReset = br.ReadBoolean();
			data.IsFreeSeason = br.ReadBoolean();
			data.RankPoint = br.ReadInt32();
			data.Rank = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("SeasonId", SeasonId);
			json.Add("SeasonState", SeasonState);
			json.Add("BuySeasonPass", BuySeasonPass);
			json.Add("SeasonTrophy", SeasonTrophy);
			json.Add("SeasonResetRemainTime", SeasonResetRemainTime);
			json.Add("SeasonPassRewardIds", JsonConvert.SerializeObject(SeasonPassRewardIds));
			json.Add("SeasonPassRewardStep", SeasonPassRewardStep);
			json.Add("NeedSeasonReset", NeedSeasonReset);
			json.Add("IsFreeSeason", IsFreeSeason);
			json.Add("RankPoint", RankPoint);
			json.Add("Rank", Rank);
			return json.ToString();
		}

		public static UserSeasonInfo JsonDeserialize(JObject json)
		{
			var data = new UserSeasonInfo();
			data.SeasonId = (int)json["SeasonId"];
			data.SeasonState = (int)json["SeasonState"];
			data.BuySeasonPass = (bool)json["BuySeasonPass"];
			data.SeasonTrophy = (int)json["SeasonTrophy"];
			data.SeasonResetRemainTime = (int)json["SeasonResetRemainTime"];
			data.SeasonPassRewardIds = JsonConvert.DeserializeObject<List<int>>(json["SeasonPassRewardIds"].ToString());
			data.SeasonPassRewardStep = (int)json["SeasonPassRewardStep"];
			data.NeedSeasonReset = (bool)json["NeedSeasonReset"];
			data.IsFreeSeason = (bool)json["IsFreeSeason"];
			data.RankPoint = (int)json["RankPoint"];
			data.Rank = (int)json["Rank"];
			return data;
		}

	}

	public class UserInfo
	{
		// UserId
		public string UserId;
		// UserName
		public string UserName;
		// IsNameInit
		public bool IsNameInit;
		// TutorialEnd
		public bool TutorialEnd;
		// UserClass
		public int UserClass;
		// BestTrophy
		public int BestTrophy;
		// Trophy
		public int Trophy;
		// TrophyRewards
		public List<int> TrophyRewards;
		// Win
		public int Win;
		// Lose
		public int Lose;
		// WinStreak
		public int WinStreak;
		// WinningRate
		public int WinningRate;
		// IsVip
		public bool IsVip;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(UserId);
			bw.Write(UserName);
			bw.Write(IsNameInit);
			bw.Write(TutorialEnd);
			bw.Write(UserClass);
			bw.Write(BestTrophy);
			bw.Write(Trophy);
			int lengthTrophyRewards = (TrophyRewards == null) ? 0 : TrophyRewards.Count;
			bw.Write(lengthTrophyRewards);
			for (int i = 0; i < lengthTrophyRewards; i++)
				bw.Write(TrophyRewards[i]);
			bw.Write(Win);
			bw.Write(Lose);
			bw.Write(WinStreak);
			bw.Write(WinningRate);
			bw.Write(IsVip);
		}

		public static UserInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new UserInfo();
			data.UserId = br.ReadString();
			data.UserName = br.ReadString();
			data.IsNameInit = br.ReadBoolean();
			data.TutorialEnd = br.ReadBoolean();
			data.UserClass = br.ReadInt32();
			data.BestTrophy = br.ReadInt32();
			data.Trophy = br.ReadInt32();
			int lengthTrophyRewards = br.ReadInt32();
			data.TrophyRewards = new List<int>(lengthTrophyRewards);
			for (int i = 0; i < lengthTrophyRewards; i++)
				data.TrophyRewards.Add(br.ReadInt32());
			data.Win = br.ReadInt32();
			data.Lose = br.ReadInt32();
			data.WinStreak = br.ReadInt32();
			data.WinningRate = br.ReadInt32();
			data.IsVip = br.ReadBoolean();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("UserId", UserId);
			json.Add("UserName", UserName);
			json.Add("IsNameInit", IsNameInit);
			json.Add("TutorialEnd", TutorialEnd);
			json.Add("UserClass", UserClass);
			json.Add("BestTrophy", BestTrophy);
			json.Add("Trophy", Trophy);
			json.Add("TrophyRewards", JsonConvert.SerializeObject(TrophyRewards));
			json.Add("Win", Win);
			json.Add("Lose", Lose);
			json.Add("WinStreak", WinStreak);
			json.Add("WinningRate", WinningRate);
			json.Add("IsVip", IsVip);
			return json.ToString();
		}

		public static UserInfo JsonDeserialize(JObject json)
		{
			var data = new UserInfo();
			data.UserId = (string)json["UserId"];
			data.UserName = (string)json["UserName"];
			data.IsNameInit = (bool)json["IsNameInit"];
			data.TutorialEnd = (bool)json["TutorialEnd"];
			data.UserClass = (int)json["UserClass"];
			data.BestTrophy = (int)json["BestTrophy"];
			data.Trophy = (int)json["Trophy"];
			data.TrophyRewards = JsonConvert.DeserializeObject<List<int>>(json["TrophyRewards"].ToString());
			data.Win = (int)json["Win"];
			data.Lose = (int)json["Lose"];
			data.WinStreak = (int)json["WinStreak"];
			data.WinningRate = (int)json["WinningRate"];
			data.IsVip = (bool)json["IsVip"];
			return data;
		}

	}

	public class UserBuilding
	{
		// 건물 아이디
		public int BuildingId;
		// 건물 레벨
		public int Level;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(BuildingId);
			bw.Write(Level);
		}

		public static UserBuilding BinaryDeserialize(BinaryReader br)
		{
			var data = new UserBuilding();
			data.BuildingId = br.ReadInt32();
			data.Level = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("BuildingId", BuildingId);
			json.Add("Level", Level);
			return json.ToString();
		}

		public static UserBuilding JsonDeserialize(JObject json)
		{
			var data = new UserBuilding();
			data.BuildingId = (int)json["BuildingId"];
			data.Level = (int)json["Level"];
			return data;
		}

	}

	public class UserMatchResult
	{
		// UserId
		public string UserId;
		// MatchType
		public string MatchType;
		// MatchResult
		public int MatchResult;
		// listReward
		public List<ItemBaseInfo> listReward;
		// LoseReward
		public AdRewardInfo LoseReward;
		// listQuestParam
		public List<QuestCompleteParam> listQuestParam;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(UserId);
			bw.Write(MatchType);
			bw.Write(MatchResult);
			int lengthlistReward = (listReward == null) ? 0 : listReward.Count;
			bw.Write(lengthlistReward);
			for (int i = 0; i < lengthlistReward; i++)
				listReward[i].BinarySerialize(bw);
			LoseReward.BinarySerialize(bw);
			int lengthlistQuestParam = (listQuestParam == null) ? 0 : listQuestParam.Count;
			bw.Write(lengthlistQuestParam);
			for (int i = 0; i < lengthlistQuestParam; i++)
				listQuestParam[i].BinarySerialize(bw);
		}

		public static UserMatchResult BinaryDeserialize(BinaryReader br)
		{
			var data = new UserMatchResult();
			data.UserId = br.ReadString();
			data.MatchType = br.ReadString();
			data.MatchResult = br.ReadInt32();
			int lengthlistReward = br.ReadInt32();
			data.listReward = new List<ItemBaseInfo>(lengthlistReward);
			for (int i = 0; i < lengthlistReward; i++)
				data.listReward.Add(ItemBaseInfo.BinaryDeserialize(br));
				data.LoseReward = AdRewardInfo.BinaryDeserialize(br);
			int lengthlistQuestParam = br.ReadInt32();
			data.listQuestParam = new List<QuestCompleteParam>(lengthlistQuestParam);
			for (int i = 0; i < lengthlistQuestParam; i++)
				data.listQuestParam.Add(QuestCompleteParam.BinaryDeserialize(br));
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("UserId", UserId);
			json.Add("MatchType", MatchType);
			json.Add("MatchResult", MatchResult);
			json.Add("listReward", JsonConvert.SerializeObject(listReward));
			json.Add("LoseReward", LoseReward.JsonSerialize());
			json.Add("listQuestParam", JsonConvert.SerializeObject(listQuestParam));
			return json.ToString();
		}

		public static UserMatchResult JsonDeserialize(JObject json)
		{
			var data = new UserMatchResult();
			data.UserId = (string)json["UserId"];
			data.MatchType = (string)json["MatchType"];
			data.MatchResult = (int)json["MatchResult"];
			data.listReward = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["listReward"].ToString());
				data.LoseReward = AdRewardInfo.JsonDeserialize((JObject)json["LoseReward"]);
			data.listQuestParam = JsonConvert.DeserializeObject<List<QuestCompleteParam>>(json["listQuestParam"].ToString());
			return data;
		}

	}

	public class QuestCompleteParam
	{
		// QuestCompleteType
		public int QuestCompleteType;
		// Value
		public int Value;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(QuestCompleteType);
			bw.Write(Value);
		}

		public static QuestCompleteParam BinaryDeserialize(BinaryReader br)
		{
			var data = new QuestCompleteParam();
			data.QuestCompleteType = br.ReadInt32();
			data.Value = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("QuestCompleteType", QuestCompleteType);
			json.Add("Value", Value);
			return json.ToString();
		}

		public static QuestCompleteParam JsonDeserialize(JObject json)
		{
			var data = new QuestCompleteParam();
			data.QuestCompleteType = (int)json["QuestCompleteType"];
			data.Value = (int)json["Value"];
			return data;
		}

	}

	public class MailInfo
	{
		// 아이디 (식별값)
		public string mailId;
		// 테이블 아이디
		public int mailTableId;
		// 보상 목록
		public List<ItemBaseInfo> mailItems;
		// 만료까지 남은 시간
		public int expireRemainTime;
		// 추가 텍스트 목록
		public List<string> listText;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(mailId);
			bw.Write(mailTableId);
			int lengthmailItems = (mailItems == null) ? 0 : mailItems.Count;
			bw.Write(lengthmailItems);
			for (int i = 0; i < lengthmailItems; i++)
				mailItems[i].BinarySerialize(bw);
			bw.Write(expireRemainTime);
			int lengthlistText = (listText == null) ? 0 : listText.Count;
			bw.Write(lengthlistText);
			for (int i = 0; i < lengthlistText; i++)
				bw.Write(listText[i]);
		}

		public static MailInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new MailInfo();
			data.mailId = br.ReadString();
			data.mailTableId = br.ReadInt32();
			int lengthmailItems = br.ReadInt32();
			data.mailItems = new List<ItemBaseInfo>(lengthmailItems);
			for (int i = 0; i < lengthmailItems; i++)
				data.mailItems.Add(ItemBaseInfo.BinaryDeserialize(br));
			data.expireRemainTime = br.ReadInt32();
			int lengthlistText = br.ReadInt32();
			data.listText = new List<string>(lengthlistText);
			for (int i = 0; i < lengthlistText; i++)
				data.listText.Add(br.ReadString());
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("mailId", mailId);
			json.Add("mailTableId", mailTableId);
			json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
			json.Add("expireRemainTime", expireRemainTime);
			json.Add("listText", JsonConvert.SerializeObject(listText));
			return json.ToString();
		}

		public static MailInfo JsonDeserialize(JObject json)
		{
			var data = new MailInfo();
			data.mailId = (string)json["mailId"];
			data.mailTableId = (int)json["mailTableId"];
			data.mailItems = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["mailItems"].ToString());
			data.expireRemainTime = (int)json["expireRemainTime"];
			data.listText = JsonConvert.DeserializeObject<List<string>>(json["listText"].ToString());
			return data;
		}

	}

}
