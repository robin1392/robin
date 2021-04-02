using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Season.GameBaseSeason.Common
{
	public class SeasonInfoRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 90001;

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

	public class SeasonInfoResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 90002;
		// 시즌 정보
		public UserSeasonInfo seasonInfo;
		// 랭크 정보
		public List<RankUserInfo> listRankInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				seasonInfo.BinarySerialize(bw);
				int lengthlistRankInfo = (listRankInfo == null) ? 0 : listRankInfo.Count;
				bw.Write(lengthlistRankInfo);
				for (int i = 0; i < lengthlistRankInfo; i++)
					listRankInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.seasonInfo = UserSeasonInfo.BinaryDeserialize(br);
				int lengthlistRankInfo = br.ReadInt32();
				this.listRankInfo = new List<RankUserInfo>(lengthlistRankInfo);
				for (int i = 0; i < lengthlistRankInfo; i++)
					this.listRankInfo.Add(RankUserInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("seasonInfo", seasonInfo.JsonSerialize());
			json.Add("listRankInfo", JsonConvert.SerializeObject(listRankInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.seasonInfo = UserSeasonInfo.JsonDeserialize((JObject)jObject["seasonInfo"]);
			this.listRankInfo = JsonConvert.DeserializeObject<List<RankUserInfo>>(jObject["listRankInfo"].ToString());
		}

	}

	public class SeasonResetRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 90011;

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

	public class SeasonResetResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 90012;
		// 
		public UserSeasonInfo seasonInfo;
		// 
		public List<ItemBaseInfo> listRewardInfo;
		// 
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				seasonInfo.BinarySerialize(bw);
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
				this.seasonInfo = UserSeasonInfo.BinaryDeserialize(br);
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
			json.Add("seasonInfo", seasonInfo.JsonSerialize());
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.seasonInfo = UserSeasonInfo.JsonDeserialize((JObject)jObject["seasonInfo"]);
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

	public class SeasonRankRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 90021;
		// 
		public int pageNo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(pageNo);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.pageNo = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("pageNo", pageNo);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.pageNo = (int)jObject["pageNo"];
		}

	}

	public class SeasonRankResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 90022;
		// 
		public int pageNo;
		// 
		public List<RankUserInfo> listRankInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(pageNo);
				int lengthlistRankInfo = (listRankInfo == null) ? 0 : listRankInfo.Count;
				bw.Write(lengthlistRankInfo);
				for (int i = 0; i < lengthlistRankInfo; i++)
					listRankInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.pageNo = br.ReadInt32();
				int lengthlistRankInfo = br.ReadInt32();
				this.listRankInfo = new List<RankUserInfo>(lengthlistRankInfo);
				for (int i = 0; i < lengthlistRankInfo; i++)
					this.listRankInfo.Add(RankUserInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("pageNo", pageNo);
			json.Add("listRankInfo", JsonConvert.SerializeObject(listRankInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.pageNo = (int)jObject["pageNo"];
			this.listRankInfo = JsonConvert.DeserializeObject<List<RankUserInfo>>(jObject["listRankInfo"].ToString());
		}

	}

	public class SeasonPassRewardRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 90031;
		// 
		public int rewardId;
		// 
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

	public class SeasonPassRewardResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 90032;
		// 
		public List<int> listRewardId;
		// 
		public List<ItemBaseInfo> listRewardInfo;
		// 
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

	public class SeasonPassStepRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 90041;
		// 
		public int rewardId;

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
				this.rewardId = br.ReadInt32();
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
			this.rewardId = (int)jObject["rewardId"];
		}

	}

	public class SeasonPassStepResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 90042;
		// 
		public int rewardId;
		// 
		public ItemBaseInfo deleteItemInfo;
		// 
		public ItemBaseInfo rewardInfo;
		// 
		public List<QuestData> listQuestData;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(rewardId);
				deleteItemInfo.BinarySerialize(bw);
				rewardInfo.BinarySerialize(bw);
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
				this.rewardId = br.ReadInt32();
				this.deleteItemInfo = ItemBaseInfo.BinaryDeserialize(br);
				this.rewardInfo = ItemBaseInfo.BinaryDeserialize(br);
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
			json.Add("rewardId", rewardId);
			json.Add("deleteItemInfo", deleteItemInfo.JsonSerialize());
			json.Add("rewardInfo", rewardInfo.JsonSerialize());
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.rewardId = (int)jObject["rewardId"];
			this.deleteItemInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["deleteItemInfo"]);
			this.rewardInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["rewardInfo"]);
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

}
