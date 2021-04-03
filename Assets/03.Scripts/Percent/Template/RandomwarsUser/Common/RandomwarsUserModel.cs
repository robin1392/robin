using System;
using System.Collections.Generic;
using System.IO;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.User.RandomwarsUser.Common
{
	public class MsgUserInfo
	{
		// 
		public string UserId;
		// 
		public string UserName;
		// 
		public short Class;
		// 
		public byte WinStreak;
		// Gold
		public int Gold;
		// Diamond
		public int Diamond;
		// Key
		public int Key;
		// Trophy
		public int Trophy;
		// TrophyRewardIds
		public List<int> TrophyRewardIds;
		// IsBuyVipPass
		public bool IsBuyVipPass;
		// TroWinCountphy
		public int WinCount;
		// DefeatCount
		public int DefeatCount;
		// HighTrophy
		public int HighTrophy;
		// EndTutorial
		public bool EndTutorial;

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(UserId);
			bw.Write(UserName);
			bw.Write(Class);
			bw.Write(WinStreak);
			bw.Write(Gold);
			bw.Write(Diamond);
			bw.Write(Key);
			bw.Write(Trophy);
			int lengthTrophyRewardIds = (TrophyRewardIds == null) ? 0 : TrophyRewardIds.Count;
			bw.Write(lengthTrophyRewardIds);
			for (int i = 0; i < lengthTrophyRewardIds; i++)
				bw.Write(TrophyRewardIds[i]);
			bw.Write(IsBuyVipPass);
			bw.Write(WinCount);
			bw.Write(DefeatCount);
			bw.Write(HighTrophy);
			bw.Write(EndTutorial);
		}

		public static MsgUserInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new MsgUserInfo();
			data.UserId = br.ReadString();
			data.UserName = br.ReadString();
			data.Class = br.ReadInt16();
			data.WinStreak = br.ReadByte();
			data.Gold = br.ReadInt32();
			data.Diamond = br.ReadInt32();
			data.Key = br.ReadInt32();
			data.Trophy = br.ReadInt32();
			int lengthTrophyRewardIds = br.ReadInt32();
			data.TrophyRewardIds = new List<int>(lengthTrophyRewardIds);
			for (int i = 0; i < lengthTrophyRewardIds; i++)
				data.TrophyRewardIds.Add(br.ReadInt32());
			data.IsBuyVipPass = br.ReadBoolean();
			data.WinCount = br.ReadInt32();
			data.DefeatCount = br.ReadInt32();
			data.HighTrophy = br.ReadInt32();
			data.EndTutorial = br.ReadBoolean();
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("UserId", UserId);
			json.Add("UserName", UserName);
			json.Add("Class", Class);
			json.Add("WinStreak", WinStreak);
			json.Add("Gold", Gold);
			json.Add("Diamond", Diamond);
			json.Add("Key", Key);
			json.Add("Trophy", Trophy);
			json.Add("TrophyRewardIds", JsonConvert.SerializeObject(TrophyRewardIds));
			json.Add("IsBuyVipPass", IsBuyVipPass);
			json.Add("WinCount", WinCount);
			json.Add("DefeatCount", DefeatCount);
			json.Add("HighTrophy", HighTrophy);
			json.Add("EndTutorial", EndTutorial);
			return json.ToString();
		}

		public static MsgUserInfo JsonDeserialize(JObject json)
		{
			var data = new MsgUserInfo();
			data.UserId = (string)json["UserId"];
			data.UserName = (string)json["UserName"];
			data.Class = (short)json["Class"];
			data.WinStreak = (byte)json["WinStreak"];
			data.Gold = (int)json["Gold"];
			data.Diamond = (int)json["Diamond"];
			data.Key = (int)json["Key"];
			data.Trophy = (int)json["Trophy"];
			data.TrophyRewardIds = JsonConvert.DeserializeObject<List<int>>(json["TrophyRewardIds"].ToString());
			data.IsBuyVipPass = (bool)json["IsBuyVipPass"];
			data.WinCount = (int)json["WinCount"];
			data.DefeatCount = (int)json["DefeatCount"];
			data.HighTrophy = (int)json["HighTrophy"];
			data.EndTutorial = (bool)json["EndTutorial"];
			return data;
		}

	}

	public class UserItemInfo
	{
		// listBox
		public List<ItemBaseInfo> listBox;
		// listPass
		public List<ItemBaseInfo> listPass;
		// listEmoticon
		public List<ItemBaseInfo> listEmoticon;
		// listEmoticonSlot
		public List<int> listEmoticonSlot;

		public void BinarySerialize(BinaryWriter bw)
		{
			int lengthlistBox = (listBox == null) ? 0 : listBox.Count;
			bw.Write(lengthlistBox);
			for (int i = 0; i < lengthlistBox; i++)
				listBox[i].BinarySerialize(bw);
			int lengthlistPass = (listPass == null) ? 0 : listPass.Count;
			bw.Write(lengthlistPass);
			for (int i = 0; i < lengthlistPass; i++)
				listPass[i].BinarySerialize(bw);
			int lengthlistEmoticon = (listEmoticon == null) ? 0 : listEmoticon.Count;
			bw.Write(lengthlistEmoticon);
			for (int i = 0; i < lengthlistEmoticon; i++)
				listEmoticon[i].BinarySerialize(bw);
			int lengthlistEmoticonSlot = (listEmoticonSlot == null) ? 0 : listEmoticonSlot.Count;
			bw.Write(lengthlistEmoticonSlot);
			for (int i = 0; i < lengthlistEmoticonSlot; i++)
				bw.Write(listEmoticonSlot[i]);
		}

		public static UserItemInfo BinaryDeserialize(BinaryReader br)
		{
			var data = new UserItemInfo();
			int lengthlistBox = br.ReadInt32();
			data.listBox = new List<ItemBaseInfo>(lengthlistBox);
			for (int i = 0; i < lengthlistBox; i++)
				data.listBox.Add(ItemBaseInfo.BinaryDeserialize(br));
			int lengthlistPass = br.ReadInt32();
			data.listPass = new List<ItemBaseInfo>(lengthlistPass);
			for (int i = 0; i < lengthlistPass; i++)
				data.listPass.Add(ItemBaseInfo.BinaryDeserialize(br));
			int lengthlistEmoticon = br.ReadInt32();
			data.listEmoticon = new List<ItemBaseInfo>(lengthlistEmoticon);
			for (int i = 0; i < lengthlistEmoticon; i++)
				data.listEmoticon.Add(ItemBaseInfo.BinaryDeserialize(br));
			int lengthlistEmoticonSlot = br.ReadInt32();
			data.listEmoticonSlot = new List<int>(lengthlistEmoticonSlot);
			for (int i = 0; i < lengthlistEmoticonSlot; i++)
				data.listEmoticonSlot.Add(br.ReadInt32());
			return data;
		}

		public string JsonSerialize()
		{
			JObject json = new JObject();
			json.Add("listBox", JsonConvert.SerializeObject(listBox));
			json.Add("listPass", JsonConvert.SerializeObject(listPass));
			json.Add("listEmoticon", JsonConvert.SerializeObject(listEmoticon));
			json.Add("listEmoticonSlot", JsonConvert.SerializeObject(listEmoticonSlot));
			return json.ToString();
		}

		public static UserItemInfo JsonDeserialize(JObject json)
		{
			var data = new UserItemInfo();
			data.listBox = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["listBox"].ToString());
			data.listPass = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["listPass"].ToString());
			data.listEmoticon = JsonConvert.DeserializeObject<List<ItemBaseInfo>>(json["listEmoticon"].ToString());
			data.listEmoticonSlot = JsonConvert.DeserializeObject<List<int>>(json["listEmoticonSlot"].ToString());
			return data;
		}

	}

}
