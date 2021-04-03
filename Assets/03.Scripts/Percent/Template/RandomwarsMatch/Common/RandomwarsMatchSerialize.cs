using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Service.Core;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Match.RandomwarsMatch.Common
{
	public class MatchRequestRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 70001;
		// 
		public int gameMode;
		// 
		public int deckIndex;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(gameMode);
				bw.Write(deckIndex);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.gameMode = br.ReadInt32();
				this.deckIndex = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("gameMode", gameMode);
			json.Add("deckIndex", deckIndex);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.gameMode = (int)jObject["gameMode"];
			this.deckIndex = (int)jObject["deckIndex"];
		}

	}

	public class MatchRequestResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 70002;
		// 
		public string ticketId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(ticketId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.ticketId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("ticketId", ticketId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.ticketId = (string)jObject["ticketId"];
		}

	}

	public class MatchStatusRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 70011;
		// 
		public string ticketId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(ticketId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.ticketId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("ticketId", ticketId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.ticketId = (string)jObject["ticketId"];
		}

	}

	public class MatchStatusResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 70012;
		// 
		public string playerSessionId;
		// 
		public string ipAddress;
		// 
		public int port;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(playerSessionId);
				bw.Write(ipAddress);
				bw.Write(port);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.playerSessionId = br.ReadString();
				this.ipAddress = br.ReadString();
				this.port = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("playerSessionId", playerSessionId);
			json.Add("ipAddress", ipAddress);
			json.Add("port", port);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.playerSessionId = (string)jObject["playerSessionId"];
			this.ipAddress = (string)jObject["ipAddress"];
			this.port = (int)jObject["port"];
		}

	}

	public class MatchCancelRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 70021;
		// 
		public string ticketId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(ticketId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.ticketId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("ticketId", ticketId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.ticketId = (string)jObject["ticketId"];
		}

	}

	public class MatchCancelResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 70022;

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

	public class MatchInviteRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 70031;
		// 
		public int gameMode;
		// 
		public int deckIndex;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(gameMode);
				bw.Write(deckIndex);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.gameMode = br.ReadInt32();
				this.deckIndex = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("gameMode", gameMode);
			json.Add("deckIndex", deckIndex);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.gameMode = (int)jObject["gameMode"];
			this.deckIndex = (int)jObject["deckIndex"];
		}

	}

	public class MatchInviteResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 70032;
		// 
		public string ticketId;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(ticketId);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.ticketId = br.ReadString();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("ticketId", ticketId);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.ticketId = (string)jObject["ticketId"];
		}

	}

	public class MatchJoinRequest : BaseRequest, ISerializer
	{
		public static readonly int ProtocolId = 70041;
		// 
		public string ticketId;
		// 
		public int gameMode;
		// 
		public int deckIndex;

		public byte[] BinarySerialize()
		{
			using (var ms = new MemoryStream())
			{
				BinaryWriter bw = new BinaryWriter(ms);
				base.BinarySerialize(bw);
				bw.Write(ticketId);
				bw.Write(gameMode);
				bw.Write(deckIndex);
				return ms.ToArray();
			}
		}

		public void BinaryDeserialize(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				BinaryReader br = new BinaryReader(ms);
				base.BinaryDeserialize(br);
				this.ticketId = br.ReadString();
				this.gameMode = br.ReadInt32();
				this.deckIndex = br.ReadInt32();
			}
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			base.JsonSerialize(json);
			json.Add("ticketId", ticketId);
			json.Add("gameMode", gameMode);
			json.Add("deckIndex", deckIndex);
			return json.ToString();
		}

		public void JsonDeserialize(string json)
		{
			JObject jObject = JObject.Parse(json);
			base.JsonDeserialize(jObject);
			this.ticketId = (string)jObject["ticketId"];
			this.gameMode = (int)jObject["gameMode"];
			this.deckIndex = (int)jObject["deckIndex"];
		}

	}

	public class MatchJoinResponse : BaseResponse, ISerializer
	{
		public static readonly int ProtocolId = 70042;

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
