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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonInfoRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonInfoRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonInfoResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonInfoResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonResetRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonResetRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonResetResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonResetResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonRankRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonRankRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonRankResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonRankResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonPassRewardRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonPassRewardRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonPassRewardResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonPassRewardResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonPassStepRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonPassStepRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SeasonPassStepResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SeasonPassStepResponse>(json);
		}

	}

}
