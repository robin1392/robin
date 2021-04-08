using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Quest.GameBaseQuest.Common
{
	public class QuestInfoRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 80001;

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

		public static QuestInfoRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestInfoRequest>(json);
		}

	}

	public class QuestInfoResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 80002;
		// 퀘스트 정보
		public QuestInfo questInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				questInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.questInfo = QuestInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static QuestInfoResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestInfoResponse>(json);
		}

	}

	public class QuestRewardRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 80011;
		// 
		public int questId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(questId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.questId = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static QuestRewardRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestRewardRequest>(json);
		}

	}

	public class QuestRewardResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 80012;
		// 
		public List<QuestData> listQuestData;
		// 
		public List<ItemBaseInfo> listRewardInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static QuestRewardResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestRewardResponse>(json);
		}

	}

	public class QuestDailyRewardRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 80021;
		// 
		public int rewardId;
		// 
		public int index;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(rewardId);
				bw.Write(index);
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
				this.index = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static QuestDailyRewardRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestDailyRewardRequest>(json);
		}

	}

	public class QuestDailyRewardResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 80022;
		// 
		public List<QuestData> listQuestData;
		// 
		public List<ItemBaseInfo> listRewardInfo;
		// 
		public QuestDayReward dailyRewardInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				int lengthlistRewardInfo = (listRewardInfo == null) ? 0 : listRewardInfo.Count;
				bw.Write(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					listRewardInfo[i].BinarySerialize(bw);
				dailyRewardInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
				int lengthlistRewardInfo = br.ReadInt32();
				this.listRewardInfo = new List<ItemBaseInfo>(lengthlistRewardInfo);
				for (int i = 0; i < lengthlistRewardInfo; i++)
					this.listRewardInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
				this.dailyRewardInfo = QuestDayReward.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static QuestDailyRewardResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<QuestDailyRewardResponse>(json);
		}

	}

}
