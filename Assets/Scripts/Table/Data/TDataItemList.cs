using System;

namespace RandomWarsResource.Data
{
	public enum EItemListKey : int
	{
		None = -1,

		gold = 1,
		dia = 2,
		thropy = 3,
		key = 11,
		vippass = 51,
		seasonpass = 52,
		dice_1000 = 101,
		dice_1001 = 102,
		dice_1002 = 103,
		dice_1003 = 104,
		dice_1004 = 105,
		dice_1005 = 106,
		dice_1006 = 107,
		dice_2000 = 108,
		dice_2001 = 109,
		dice_2002 = 110,
		dice_2003 = 111,
		dice_2004 = 112,
		dice_2005 = 113,
		dice_2006 = 114,
		dice_2007 = 115,
		dice_2008 = 116,
		dice_2009 = 117,
		dice_3000 = 118,
		dice_3001 = 119,
		dice_3002 = 120,
		dice_3003 = 121,
		dice_3004 = 122,
		dice_3005 = 123,
		dice_3006 = 124,
		dice_3007 = 125,
		dice_3008 = 126,
		dice_3009 = 127,
		dice_3010 = 128,
		dice_3011 = 129,
		dice_4000 = 130,
		dice_4001 = 131,
		dice_4002 = 132,
		dice_4003 = 133,
		dice_40031 = 134,
		dice_4004 = 135,
		dice_4005 = 136,
		dice_4006 = 137,
		dice_4007 = 138,
		dice_4008 = 139,
		dice_4009 = 140,
		guardian_20001 = 1001,
		guardian_20002 = 1002,
		guardian_20003 = 1003,
		normalbox = 2001,
		goldbox = 2002,
		diabox = 2003,
		supportbox = 2004,
		boss01box = 2005,
		boss02box = 2006,
		boss03box = 2007,
		boss04box = 2008,
		boss05box = 2009,
		normalbox_reward = 2010,
		goldbox_reward = 2011,
		diabox_reward = 2012,
		boss01box_reward = 2013,
		boss02box_reward = 2014,
		boss03box_reward = 2015,
		boss04box_reward = 2016,
		boss05box_reward = 2017,
	}

	public class TDataItemList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int itemType { get; set; }
		public string itemName_KR { get; set; }
		public string itemName_EN { get; set; }
		public string itemIcon { get; set; }
		public string itemText_KR { get; set; }
		public string itemText_EN { get; set; }
		public int boxOpenType { get; set; }
		public int openKeyValue { get; set; }
		public int[] productId { get; set; }
		public bool isUse { get; set; }
		public int maxValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			itemType = int.Parse(cols[2]);
			itemName_KR = cols[3].Replace("{#$}", ",");
			itemName_EN = cols[4].Replace("{#$}", ",");
			itemIcon = cols[5].Replace("{#$}", ",");
			itemText_KR = cols[6].Replace("{#$}", ",");
			itemText_EN = cols[7].Replace("{#$}", ",");
			boxOpenType = int.Parse(cols[8]);
			openKeyValue = int.Parse(cols[9]);
			productId = Array.ConvertAll(cols[10].Split('|'), s => int.Parse(s));
			isUse = bool.Parse(cols[11]);
			maxValue = int.Parse(cols[12]);
		}
	}
}
