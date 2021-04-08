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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailBoxInfoRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailBoxInfoRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailBoxInfoResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailBoxInfoResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailBoxRefreshRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailBoxRefreshRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailBoxRefreshResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailBoxRefreshResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailReceiveRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailReceiveRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailReceiveResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailReceiveResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailReceiveAllRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailReceiveAllRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailReceiveAllResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailReceiveAllResponse>(json);
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
		public List<ItemBaseInfo> mailItems;

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
					mailItems[i].BinarySerialize(bw);
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
				this.mailItems = new List<ItemBaseInfo>(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					this.mailItems.Add(ItemBaseInfo.BinaryDeserialize(br));
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailSendRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailSendRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MailSendResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MailSendResponse>(json);
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
		public List<ItemBaseInfo> mailItems;
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
					mailItems[i].BinarySerialize(bw);
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
				this.mailItems = new List<ItemBaseInfo>(lengthmailItems);
				for (int i = 0; i < lengthmailItems; i++)
					this.mailItems.Add(ItemBaseInfo.BinaryDeserialize(br));
				this.sendTime = br.ReadString();
				this.endTime = br.ReadString();
				this.storeDay = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SystemMailSendRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SystemMailSendRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static SystemMailSendResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<SystemMailSendResponse>(json);
		}

	}

}
