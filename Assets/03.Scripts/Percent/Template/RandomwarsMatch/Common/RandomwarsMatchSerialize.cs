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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchRequestRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchRequestRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchRequestResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchRequestResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchStatusRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchStatusRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchStatusResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchStatusResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchCancelRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchCancelRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchCancelResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchCancelResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchInviteRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchInviteRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchInviteResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchInviteResponse>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchJoinRequest JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchJoinRequest>(json);
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
			return JsonConvert.SerializeObject(this).ToString();
		}

		public static MatchJoinResponse JsonDeserialize(string json)
		{
			return JsonConvert.DeserializeObject<MatchJoinResponse>(json);
		}

	}

}
