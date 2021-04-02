using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service.Net
{
    public interface ISerializer
    {
        byte[] BinarySerialize();
        void BinaryDeserialize(byte[] buffer);
        string JsonSerialize();
        void JsonDeserialize(string json);
    }


    public class BaseRequest
    {
        public string accessToken = string.Empty;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(accessToken);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			this.accessToken = br.ReadString();
		}

		public void JsonSerialize(JObject json)
		{
			json.Add("accessToken", accessToken);
		}

		public void JsonDeserialize(JObject json)
		{
			this.accessToken = json["accessToken"].ToString();
		}        
    }


    public class BaseResponse
    {
        public int errorCode = 0;

        public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(errorCode);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			this.errorCode = br.ReadInt32();
		}

		public void JsonSerialize(JObject json)
		{
			json.Add("errorCode", errorCode);
		}

		public void JsonDeserialize(JObject json)
		{
			this.errorCode = (int)json["errorCode"];
		}       
    }


    public class ProtocolBase
    {
		protected readonly string _serverAddr = string.Empty;
		protected ISender _sender;
        internal Dictionary<int, ControllerDelegate> MessageControllers { get; set; }

        public ProtocolBase(ISender sender, string serverAddr)
        {
            _sender = sender;
			_serverAddr = serverAddr;
        }
    }
}