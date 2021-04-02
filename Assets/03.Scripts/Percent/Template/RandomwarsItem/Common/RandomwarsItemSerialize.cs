using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Item.RandomwarsItem.Common
{
	public class BoxOpenRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 40001;
		// 
		public int boxId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(boxId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.boxId = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("boxId", boxId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.boxId = (int)jObject["boxId"];
		}

	}

	public class BoxOpenResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 40002;
		// 
		public List<ItemBaseInfo> listDeleteItemInfo;
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
				int lengthlistDeleteItemInfo = (listDeleteItemInfo == null) ? 0 : listDeleteItemInfo.Count;
				bw.Write(lengthlistDeleteItemInfo);
				for (int i = 0; i < lengthlistDeleteItemInfo; i++)
					listDeleteItemInfo[i].BinarySerialize(bw);
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
				int lengthlistDeleteItemInfo = br.ReadInt32();
				this.listDeleteItemInfo = new List<ItemBaseInfo>(lengthlistDeleteItemInfo);
				for (int i = 0; i < lengthlistDeleteItemInfo; i++)
					this.listDeleteItemInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
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
			json.Add("listDeleteItemInfo", JsonConvert.SerializeObject(listDeleteItemInfo));
			json.Add("listRewardInfo", JsonConvert.SerializeObject(listRewardInfo));
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listDeleteItemInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listDeleteItemInfo"].ToString());
			this.listRewardInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listRewardInfo"].ToString());
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
		}

	}

	public class EmoticonEquipRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 40011;
		// 
		public List<int> listItemId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistItemId = (listItemId == null) ? 0 : listItemId.Count;
				bw.Write(lengthlistItemId);
				for (int i = 0; i < lengthlistItemId; i++)
					bw.Write(listItemId[i]);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistItemId = br.ReadInt32();
				this.listItemId = new List<int>(lengthlistItemId);
				for (int i = 0; i < lengthlistItemId; i++)
					this.listItemId.Add(br.ReadInt32());
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listItemId", JsonConvert.SerializeObject(listItemId));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listItemId = JsonConvert.DeserializeObject<List<int>>(jObject["listItemId"].ToString());
		}

	}

	public class EmoticonEquipResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 40012;
		// 
		public List<int> listItemId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistItemId = (listItemId == null) ? 0 : listItemId.Count;
				bw.Write(lengthlistItemId);
				for (int i = 0; i < lengthlistItemId; i++)
					bw.Write(listItemId[i]);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistItemId = br.ReadInt32();
				this.listItemId = new List<int>(lengthlistItemId);
				for (int i = 0; i < lengthlistItemId; i++)
					this.listItemId.Add(br.ReadInt32());
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listItemId", JsonConvert.SerializeObject(listItemId));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listItemId = JsonConvert.DeserializeObject<List<int>>(jObject["listItemId"].ToString());
		}

	}

}
