using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Account.GameBaseAccount.Common
{
	public class AccountLoginRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 10001;
		// 
		public string platformId;
		// 
		public int platformType;
		// 
		public string guid;
		// 
		public string adid;
		// 
		public string appid;
		// 
		public string version;
		// 
		public string os;
		// 
		public string osVersion;
		// 
		public string device;
		// 
		public string country;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(platformId);
				bw.Write(platformType);
				bw.Write(guid);
				bw.Write(adid);
				bw.Write(appid);
				bw.Write(version);
				bw.Write(os);
				bw.Write(osVersion);
				bw.Write(device);
				bw.Write(country);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.platformId = br.ReadString();
				this.platformType = br.ReadInt32();
				this.guid = br.ReadString();
				this.adid = br.ReadString();
				this.appid = br.ReadString();
				this.version = br.ReadString();
				this.os = br.ReadString();
				this.osVersion = br.ReadString();
				this.device = br.ReadString();
				this.country = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("platformId", platformId);
			json.Add("platformType", platformType);
			json.Add("guid", guid);
			json.Add("adid", adid);
			json.Add("appid", appid);
			json.Add("version", version);
			json.Add("os", os);
			json.Add("osVersion", osVersion);
			json.Add("device", device);
			json.Add("country", country);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.platformId = (string)jObject["platformId"];
			this.platformType = (int)jObject["platformType"];
			this.guid = (string)jObject["guid"];
			this.adid = (string)jObject["adid"];
			this.appid = (string)jObject["appid"];
			this.version = (string)jObject["version"];
			this.os = (string)jObject["os"];
			this.osVersion = (string)jObject["osVersion"];
			this.device = (string)jObject["device"];
			this.country = (string)jObject["country"];
		}

	}

	public class AccountLoginResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 10002;
		// 계정 정보
		public AccountInfo accountInfo;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				accountInfo.BinarySerialize(bw);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.accountInfo = AccountInfo.BinaryDeserialize(br);
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("accountInfo", accountInfo.JsonSerialize());
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.accountInfo = AccountInfo.JsonDeserialize((JObject)jObject["accountInfo"]);
		}

	}

	public class AccountPlatformLinkRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 10011;
		// 요청 플랫폼 아이디
		public string platformId;
		// 요청 플랫폼 타입
		public int platformType;
		// 승인 여부
		public bool isConfirm;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(platformId);
				bw.Write(platformType);
				bw.Write(isConfirm);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.platformId = br.ReadString();
				this.platformType = br.ReadInt32();
				this.isConfirm = br.ReadBoolean();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("platformId", platformId);
			json.Add("platformType", platformType);
			json.Add("isConfirm", isConfirm);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.platformId = (string)jObject["platformId"];
			this.platformType = (int)jObject["platformType"];
			this.isConfirm = (bool)jObject["isConfirm"];
		}

	}

	public class AccountPlatformLinkResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 10012;
		// 계정 정보
		public AccountInfo accountInfo;
		// 승인 필요 여부
		public bool needConfirm;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				accountInfo.BinarySerialize(bw);
				bw.Write(needConfirm);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.accountInfo = AccountInfo.BinaryDeserialize(br);
				this.needConfirm = br.ReadBoolean();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("accountInfo", accountInfo.JsonSerialize());
			json.Add("needConfirm", needConfirm);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.accountInfo = AccountInfo.JsonDeserialize((JObject)jObject["accountInfo"]);
			this.needConfirm = (bool)jObject["needConfirm"];
		}

	}

	public class AccountLogoutRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 10021;

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

	public class AccountLogoutResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 10022;

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

}
