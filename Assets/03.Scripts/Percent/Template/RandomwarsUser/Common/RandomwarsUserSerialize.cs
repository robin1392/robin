using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.User.RandomwarsUser.Common
{
	public class UserInfoRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20001;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
		}

	}

	public class UserInfoResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20002;
		// 유저 정보
		public MsgUserInfo userInfo;
		// 유저 덱정보
		public List<UserDeck> listUserDeck;
		// 유저 주사위 정보
		public List<UserDice> listUserDice;
		// 유저 아이템 정보
		public UserItemInfo userItemInfo;
		// 유저 퀘스트 정보
		public QuestInfo questInfo;
		// 유저 시즌 정보
		public UserSeasonInfo seasonInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				userInfo.BinarySerialize(bw);
				int lengthlistUserDeck = (listUserDeck == null) ? 0 : listUserDeck.Count;
				bw.Write(lengthlistUserDeck);
				for (int i = 0; i < lengthlistUserDeck; i++)
					listUserDeck[i].BinarySerialize(bw);
				int lengthlistUserDice = (listUserDice == null) ? 0 : listUserDice.Count;
				bw.Write(lengthlistUserDice);
				for (int i = 0; i < lengthlistUserDice; i++)
					listUserDice[i].BinarySerialize(bw);
				userItemInfo.BinarySerialize(bw);
				questInfo.BinarySerialize(bw);
				seasonInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userInfo = MsgUserInfo.BinaryDeserialize(br);
				int lengthlistUserDeck = br.ReadInt32();
				this.listUserDeck = new List<UserDeck>(lengthlistUserDeck);
				for (int i = 0; i < lengthlistUserDeck; i++)
					this.listUserDeck.Add(UserDeck.BinaryDeserialize(br));
				int lengthlistUserDice = br.ReadInt32();
				this.listUserDice = new List<UserDice>(lengthlistUserDice);
				for (int i = 0; i < lengthlistUserDice; i++)
					this.listUserDice.Add(UserDice.BinaryDeserialize(br));
				this.userItemInfo = UserItemInfo.BinaryDeserialize(br);
				this.questInfo = QuestInfo.BinaryDeserialize(br);
				this.seasonInfo = UserSeasonInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userInfo", userInfo.JsonSerialize());
			json.Add("listUserDeck", JsonConvert.SerializeObject(listUserDeck));
			json.Add("listUserDice", JsonConvert.SerializeObject(listUserDice));
			json.Add("userItemInfo", userItemInfo.JsonSerialize());
			json.Add("questInfo", questInfo.JsonSerialize());
			json.Add("seasonInfo", seasonInfo.JsonSerialize());
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userInfo = MsgUserInfo.JsonDeserialize((JObject)jObject["userInfo"]);
			this.listUserDeck = JsonConvert.DeserializeObject<List<UserDeck>>(jObject["listUserDeck"].ToString());
			this.listUserDice = JsonConvert.DeserializeObject<List<UserDice>>(jObject["listUserDice"].ToString());
			this.userItemInfo = UserItemInfo.JsonDeserialize((JObject)jObject["userItemInfo"]);
			this.questInfo = QuestInfo.JsonDeserialize((JObject)jObject["questInfo"]);
			this.seasonInfo = UserSeasonInfo.JsonDeserialize((JObject)jObject["seasonInfo"]);
		}

	}

	public class UserTutorialEndRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20011;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
		}

	}

	public class UserTutorialEndResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20012;
		// 튜토리얼 종료 여부
		public bool endTutorial;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(endTutorial);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.endTutorial = br.ReadBoolean();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("endTutorial", endTutorial);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.endTutorial = (bool)jObject["endTutorial"];
		}

	}

	public class UserNameInitRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20021;
		// 유저명
		public string userName;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(userName);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userName = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userName", userName);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userName = (string)jObject["userName"];
		}

	}

	public class UserNameInitResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20022;
		// 유저명
		public string userName;
		// 초기화 여부
		public bool isNameInit;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(userName);
				bw.Write(isNameInit);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userName = br.ReadString();
				this.isNameInit = br.ReadBoolean();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userName", userName);
			json.Add("isNameInit", isNameInit);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userName = (string)jObject["userName"];
			this.isNameInit = (bool)jObject["isNameInit"];
		}

	}

	public class UserNameChangeRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20031;
		// 유저명
		public string userName;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(userName);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userName = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userName", userName);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userName = (string)jObject["userName"];
		}

	}

	public class UserNameChangeResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20032;
		// 유저명
		public string userName;
		// 소모 아이템 정보
		public List<ItemBaseInfo> listDeleteItemInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(userName);
				int lengthlistDeleteItemInfo = (listDeleteItemInfo == null) ? 0 : listDeleteItemInfo.Count;
				bw.Write(lengthlistDeleteItemInfo);
				for (int i = 0; i < lengthlistDeleteItemInfo; i++)
					listDeleteItemInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userName = br.ReadString();
				int lengthlistDeleteItemInfo = br.ReadInt32();
				this.listDeleteItemInfo = new List<ItemBaseInfo>(lengthlistDeleteItemInfo);
				for (int i = 0; i < lengthlistDeleteItemInfo; i++)
					this.listDeleteItemInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userName", userName);
			json.Add("listDeleteItemInfo", JsonConvert.SerializeObject(listDeleteItemInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userName = (string)jObject["userName"];
			this.listDeleteItemInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listDeleteItemInfo"].ToString());
		}

	}

	public class UserTrophyRewardRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20041;
		// 보상 아이디
		public int rewardId;
		// 보상 타입
		public int targetType;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(rewardId);
				bw.Write(targetType);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.rewardId = br.ReadInt32();
				this.targetType = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("rewardId", rewardId);
			json.Add("targetType", targetType);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.rewardId = (int)jObject["rewardId"];
			this.targetType = (int)jObject["targetType"];
		}

	}

	public class UserTrophyRewardResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20042;
		// 보상 타입
		public List<int> listRewardId;
		// 보상 아이템 정보
		public List<ItemBaseInfo> listRewardInfo;
		// 퀘스트 데이터 정보
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistRewardId = (listRewardId == null) ? 0 : listRewardId.Count;
				bw.Write(lengthlistRewardId);
				for (int i = 0; i < lengthlistRewardId; i++)
					bw.Write(listRewardId[i]);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistRewardId = br.ReadInt32();
				this.listRewardId = new List<int>(lengthlistRewardId);
				for (int i = 0; i < lengthlistRewardId; i++)
					this.listRewardId.Add(br.ReadInt32());
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listRewardId", JsonConvert.SerializeObject(listRewardId));
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listRewardId = JsonConvert.DeserializeObject<List<int>>(jObject["listRewardId"].ToString());
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

	public class UserAdRewardRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 20051;
		// 보상 아이디
		public string rewardId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(rewardId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.rewardId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("rewardId", rewardId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.rewardId = (string)jObject["rewardId"];
		}

	}

	public class UserAdRewardResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 20052;
		// 보상 아이템 정보
		public List<ItemBaseInfo> listRewardInfo;
		// 퀘스트 데이터 정보
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

}
