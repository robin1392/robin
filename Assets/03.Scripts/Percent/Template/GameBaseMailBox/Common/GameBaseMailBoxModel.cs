using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.MailBox.GameBaseMailBox.Common
{
	public class SystemMailInfo
	{
		// 아이디 (식별값)
		public string mailId;
		// 테이블 아이디
		public int mailTableId;
		// 보상 목록
		public List<ItemBaseInfo> mailItems;
		// 추가 텍스트 목록
		public List<string> listText;
		// 전송 시간
		public DateTime sendTime;
		// 보관일
		public int storeDay;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(mailId);
			bw.Write(mailTableId);
			int lengthmailItems = (mailItems == null) ? 0 : mailItems.Count;
			bw.Write(lengthmailItems);
			for (int i = 0; i < lengthmailItems; i++)
				mailItems[i].BinarySerialize(bw);
			int lengthlistText = (listText == null) ? 0 : listText.Count;
			bw.Write(lengthlistText);
			for (int i = 0; i < lengthlistText; i++)
				bw.Write(listText[i]);
			bw.Write(sendTime.ToString());
			bw.Write(storeDay);
		}

		public static SystemMailInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new SystemMailInfo();
			data.mailId = br.ReadString();
			data.mailTableId = br.ReadInt32();
			int lengthmailItems = br.ReadInt32();
			data.mailItems = new List<ItemBaseInfo>(lengthmailItems);
			for (int i = 0; i < lengthmailItems; i++)
				data.mailItems.Add(ItemBaseInfo.BinaryDeserialize(br));
			int lengthlistText = br.ReadInt32();
			data.listText = new List<string>(lengthlistText);
			for (int i = 0; i < lengthlistText; i++)
				data.listText.Add(br.ReadString());
			data.sendTime = DateTime.Parse(br.ReadString());
			data.storeDay = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("mailId", mailId);
			json.Add("mailTableId", mailTableId);
			json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
			json.Add("listText", JsonConvert.SerializeObject(listText));
			json.Add("sendTime", sendTime);
			json.Add("storeDay", storeDay);
			return json.ToString();
		}

		public static SystemMailInfo JsonDeserialize(JObject json)
		{
			var data = new SystemMailInfo();
			data.mailId = (string)json["mailId"];
			data.mailTableId = (int)json["mailTableId"];
			data.mailItems = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["mailItems"].ToString());
			data.listText = JsonConvert.DeserializeObject<List<string>>(json["listText"].ToString());
			data.sendTime = DateTime.Parse(json["sendTime"].ToString());
			data.storeDay = (int)json["storeDay"];
			return data;
		}

	}

}
