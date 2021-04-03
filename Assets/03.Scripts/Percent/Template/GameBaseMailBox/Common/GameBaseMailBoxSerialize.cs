using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.MailBox.GameBaseMailBox.Common
{
	public class MailBoxInfoRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60001;

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

	public class MailBoxInfoResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60002;
		// 
		public List<MailInfo> listMailInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistMailInfo = (listMailInfo == null) ? 0 : listMailInfo.Count;
				bw.Write(lengthlistMailInfo);
				for (int i = 0; i < lengthlistMailInfo; i++)
					listMailInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistMailInfo = br.ReadInt32();
				this.listMailInfo = new List<MailInfo>(lengthlistMailInfo);
				for (int i = 0; i < lengthlistMailInfo; i++)
					this.listMailInfo.Add(MailInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listMailInfo", JsonConvert.SerializeObject(listMailInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listMailInfo = JsonConvert.DeserializeObject<List<MailInfo>>(jObject["listMailInfo"].ToString());
		}

	}

	public class MailBoxRefreshRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60011;

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

	public class MailBoxRefreshResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60012;
		// 
		public List<MailInfo> listMailInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistMailInfo = (listMailInfo == null) ? 0 : listMailInfo.Count;
				bw.Write(lengthlistMailInfo);
				for (int i = 0; i < lengthlistMailInfo; i++)
					listMailInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistMailInfo = br.ReadInt32();
				this.listMailInfo = new List<MailInfo>(lengthlistMailInfo);
				for (int i = 0; i < lengthlistMailInfo; i++)
					this.listMailInfo.Add(MailInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listMailInfo", JsonConvert.SerializeObject(listMailInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listMailInfo = JsonConvert.DeserializeObject<List<MailInfo>>(jObject["listMailInfo"].ToString());
		}

	}

	public class MailReceiveRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60021;
		// 
		public string mailId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(mailId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.mailId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("mailId", mailId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.mailId = (string)jObject["mailId"];
		}

	}

	public class MailReceiveResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60022;
		// 
		public List<ItemBaseInfo> listMailItemInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistMailItemInfo = (listMailItemInfo == null) ? 0 : listMailItemInfo.Count;
				bw.Write(lengthlistMailItemInfo);
				for (int i = 0; i < lengthlistMailItemInfo; i++)
					listMailItemInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistMailItemInfo = br.ReadInt32();
				this.listMailItemInfo = new List<ItemBaseInfo>(lengthlistMailItemInfo);
				for (int i = 0; i < lengthlistMailItemInfo; i++)
					this.listMailItemInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listMailItemInfo", JsonConvert.SerializeObject(listMailItemInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listMailItemInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listMailItemInfo"].ToString());
		}

	}

	public class MailReceiveAllRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60031;

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

	public class MailReceiveAllResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60032;
		// 
		public List<ItemBaseInfo> listMailItemInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				int lengthlistMailItemInfo = (listMailItemInfo == null) ? 0 : listMailItemInfo.Count;
				bw.Write(lengthlistMailItemInfo);
				for (int i = 0; i < lengthlistMailItemInfo; i++)
					listMailItemInfo[i].BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				int lengthlistMailItemInfo = br.ReadInt32();
				this.listMailItemInfo = new List<ItemBaseInfo>(lengthlistMailItemInfo);
				for (int i = 0; i < lengthlistMailItemInfo; i++)
					this.listMailItemInfo.Add(ItemBaseInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("listMailItemInfo", JsonConvert.SerializeObject(listMailItemInfo));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.listMailItemInfo = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(jObject["listMailItemInfo"].ToString());
		}

	}

	public class MailSendRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60041;
		// 
		public string userId;
		// 
		public int mailTableId;
		// 
		public List<string> listCustomText;
		// 
		public List<Dictionary<string, int>> mailItems;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(userId);
				bw.Write(mailTableId);
				int lengthlistCustomText = (listCustomText == null) ? 0 : listCustomText.Count;
				bw.Write(lengthlistCustomText);
				for (int i = 0; i < lengthlistCustomText; i++)
					bw.Write(listCustomText[i]);
				int lengthmailItems = (mailItems == null) ? 0 : mailItems.Count;
				bw.Write(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					;
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.userId = br.ReadString();
				this.mailTableId = br.ReadInt32();
				int lengthlistCustomText = br.ReadInt32();
				this.listCustomText = new List<string>(lengthlistCustomText);
				for (int i = 0; i < lengthlistCustomText; i++)
					this.listCustomText.Add(br.ReadString());
				int lengthmailItems = br.ReadInt32();
				this.mailItems = new List<Dictionary<string, int>>(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					;
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("userId", userId);
			json.Add("mailTableId", mailTableId);
			json.Add("listCustomText", JsonConvert.SerializeObject(listCustomText));
			json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.userId = (string)jObject["userId"];
			this.mailTableId = (int)jObject["mailTableId"];
			this.listCustomText = JsonConvert.DeserializeObject<List<string>>(jObject["listCustomText"].ToString());
			this.mailItems = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(jObject["mailItems"].ToString());
		}

	}

	public class MailSendResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60042;
		// 
		public MailInfo mailInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				mailInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.mailInfo = MailInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("mailInfo", mailInfo.JsonSerialize());
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.mailInfo = MailInfo.JsonDeserialize((JObject)jObject["mailInfo"]);
		}

	}

	public class SystemMailSendRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 60051;
		// 
		public int mailTableId;
		// 
		public List<string> listCustomText;
		// 
		public List<Dictionary<string, int>> mailItems;
		// 
		public string sendTime;
		// 
		public string endTime;
		// 
		public int storeDay;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(mailTableId);
				int lengthlistCustomText = (listCustomText == null) ? 0 : listCustomText.Count;
				bw.Write(lengthlistCustomText);
				for (int i = 0; i < lengthlistCustomText; i++)
					bw.Write(listCustomText[i]);
				int lengthmailItems = (mailItems == null) ? 0 : mailItems.Count;
				bw.Write(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					;
				bw.Write(sendTime);
				bw.Write(endTime);
				bw.Write(storeDay);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.mailTableId = br.ReadInt32();
				int lengthlistCustomText = br.ReadInt32();
				this.listCustomText = new List<string>(lengthlistCustomText);
				for (int i = 0; i < lengthlistCustomText; i++)
					this.listCustomText.Add(br.ReadString());
				int lengthmailItems = br.ReadInt32();
				this.mailItems = new List<Dictionary<string, int>>(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					;
				this.sendTime = br.ReadString();
				this.endTime = br.ReadString();
				this.storeDay = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("mailTableId", mailTableId);
			json.Add("listCustomText", JsonConvert.SerializeObject(listCustomText));
			json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
			json.Add("sendTime", sendTime);
			json.Add("endTime", endTime);
			json.Add("storeDay", storeDay);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.mailTableId = (int)jObject["mailTableId"];
			this.listCustomText = JsonConvert.DeserializeObject<List<string>>(jObject["listCustomText"].ToString());
			this.mailItems = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(jObject["mailItems"].ToString());
			this.sendTime = (string)jObject["sendTime"];
			this.endTime = (string)jObject["endTime"];
			this.storeDay = (int)jObject["storeDay"];
		}

	}

	public class SystemMailSendResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 60052;
		// 
		public bool result;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(result);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.result = br.ReadBoolean();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("result", result);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.result = (bool)jObject["result"];
		}

	}

}
