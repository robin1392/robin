using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Season.GameBaseSeason.Common
{
	public class RankUserInfo
	{
		// 
		public int Ranking;
		// 
		public string Name;
		// 
		public short Class;
		// 
		public List<int> listDiceId;
		// 
		public int Trophy;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(Ranking);
			bw.Write(Name);
			bw.Write(Class);
			int lengthlistDiceId = (listDiceId == null) ? 0 : listDiceId.Count;
			bw.Write(lengthlistDiceId);
			for (int i = 0; i < lengthlistDiceId; i++)
				bw.Write(listDiceId[i]);
			bw.Write(Trophy);
		}

		public static RankUserInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new RankUserInfo();
			data.Ranking = br.ReadInt32();
			data.Name = br.ReadString();
			data.Class = br.ReadInt16();
			int lengthlistDiceId = br.ReadInt32();
			data.listDiceId = new List<int>(lengthlistDiceId);
			for (int i = 0; i < lengthlistDiceId; i++)
				data.listDiceId.Add(br.ReadInt32());
			data.Trophy = br.ReadInt32();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("Ranking", Ranking);
			json.Add("Name", Name);
			json.Add("Class", Class);
			json.Add("listDiceId", JsonConvert.SerializeObject(listDiceId));
			json.Add("Trophy", Trophy);
			return json.ToString();
		}

		public static RankUserInfo JsonDeserialize(JObject json)
		{
			var data = new RankUserInfo();
			data.Ranking = (int)json["Ranking"];
			data.Name = (string)json["Name"];
			data.Class = (short)json["Class"];
			data.listDiceId = JsonConvert.DeserializeObject<List<int>>(json["listDiceId"].ToString());
			data.Trophy = (int)json["Trophy"];
			return data;
		}

	}

}
