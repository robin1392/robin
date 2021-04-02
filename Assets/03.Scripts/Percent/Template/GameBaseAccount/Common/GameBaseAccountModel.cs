using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Account.GameBaseAccount.Common
{
	public class AccountInfo
	{
		// 엑세스 토큰
		public string accessToken;
		// 플랫폼 아이디
		public string platformId;
		// 플랫폼 타입(GUEST, Android, IOS, Guest 등)
		public int platformType;
		// 계정 상태(정상, 탈퇴, 제재 등)
		public int status;
		// 상태 남은 시간(정상이면 0)
		public int statusRemainTime;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(accessToken);
			bw.Write(platformId);
			bw.Write(platformType);
			bw.Write(status);
			bw.Write(statusRemainTime);
		}

		public static AccountInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new AccountInfo();
			data.accessToken = br.ReadString();
			data.platformId = br.ReadString();
			data.platformType = br.ReadInt32();
			data.status = br.ReadInt32();
			data.statusRemainTime = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("accessToken", accessToken);
			json.Add("platformId", platformId);
			json.Add("platformType", platformType);
			json.Add("status", status);
			json.Add("statusRemainTime", statusRemainTime);
			return json.ToString();
		}

		public static AccountInfo JsonDeserialize(JObject json)
		{
			var data = new AccountInfo();
			data.accessToken = (string)json["accessToken"];
			data.platformId = (string)json["platformId"];
			data.platformType = (int)json["platformType"];
			data.status = (int)json["status"];
			data.statusRemainTime = (int)json["statusRemainTime"];
			return data;
		}

	}

}
