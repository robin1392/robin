using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Character.RandomwarsDice.Common
{
	public class DiceUpgradeRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 30001;
		// diceId
		public int diceId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(diceId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.diceId = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("diceId", diceId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.diceId = (int)jObject["diceId"];
		}

	}

	public class DiceUpgradeResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 30002;
		// diceInfo
		public UserDice diceInfo;
		// listQuestData
		public List<QuestData> listQuestData;
		// deleteItemInfo
		public ItemBaseInfo deleteItemInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				diceInfo.BinarySerialize(bw);
				int lengthlistQuestData = (listQuestData == null) ? 0 : listQuestData.Count;
				bw.Write(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					listQuestData[i].BinarySerialize(bw);
				deleteItemInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.diceInfo = UserDice.BinaryDeserialize(br);
				int lengthlistQuestData = br.ReadInt32();
				this.listQuestData = new List<QuestData>(lengthlistQuestData);
				for (int i = 0; i < lengthlistQuestData; i++)
					this.listQuestData.Add(QuestData.BinaryDeserialize(br));
				this.deleteItemInfo = ItemBaseInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("diceInfo", diceInfo.JsonSerialize());
			json.Add("listQuestData", JsonConvert.SerializeObject(listQuestData));
			json.Add("deleteItemInfo", deleteItemInfo.JsonSerialize());
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.diceInfo = UserDice.JsonDeserialize((JObject)jObject["diceInfo"]);
			this.listQuestData = JsonConvert.DeserializeObject<List<QuestData>>(jObject["listQuestData"].ToString());
			this.deleteItemInfo = ItemBaseInfo.JsonDeserialize((JObject)jObject["deleteItemInfo"]);
		}

	}

	public class DiceChangeDeckRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 30011;
		// index
		public int index;
		// listDiceId
		public List<int> listDiceId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(index);
				int lengthlistDiceId = (listDiceId == null) ? 0 : listDiceId.Count;
				bw.Write(lengthlistDiceId);
				for (int i = 0; i < lengthlistDiceId; i++)
					bw.Write(listDiceId[i]);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.index = br.ReadInt32();
				int lengthlistDiceId = br.ReadInt32();
				this.listDiceId = new List<int>(lengthlistDiceId);
				for (int i = 0; i < lengthlistDiceId; i++)
					this.listDiceId.Add(br.ReadInt32());
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("index", index);
			json.Add("listDiceId", JsonConvert.SerializeObject(listDiceId));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.index = (int)jObject["index"];
			this.listDiceId = JsonConvert.DeserializeObject<List<int>>(jObject["listDiceId"].ToString());
		}

	}

	public class DiceChangeDeckResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 30012;
		// index
		public int index;
		// listDiceId
		public List<int> listDiceId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(index);
				int lengthlistDiceId = (listDiceId == null) ? 0 : listDiceId.Count;
				bw.Write(lengthlistDiceId);
				for (int i = 0; i < lengthlistDiceId; i++)
					bw.Write(listDiceId[i]);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.index = br.ReadInt32();
				int lengthlistDiceId = br.ReadInt32();
				this.listDiceId = new List<int>(lengthlistDiceId);
				for (int i = 0; i < lengthlistDiceId; i++)
					this.listDiceId.Add(br.ReadInt32());
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("index", index);
			json.Add("listDiceId", JsonConvert.SerializeObject(listDiceId));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.index = (int)jObject["index"];
			this.listDiceId = JsonConvert.DeserializeObject<List<int>>(jObject["listDiceId"].ToString());
		}

	}

}
