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
		normalbox = 101,
		goldbox = 102,
		diabox = 103,
		supportbox = 104,
		boss01box = 105,
		boss02box = 106,
		boss03box = 107,
		boss04box = 108,
		boss05box = 109,
		boss01box_reward = 110,
		boss02box_reward = 111,
		boss03box_reward = 112,
		boss04box_reward = 113,
		boss05box_reward = 114,
		dice_1000 = 1000,
		dice_1001 = 1001,
		dice_1002 = 1002,
		dice_1003 = 1003,
		dice_1004 = 1004,
		dice_1005 = 1005,
		dice_1006 = 1006,
		dice_2000 = 2000,
		dice_2001 = 2001,
		dice_2002 = 2002,
		dice_2003 = 2003,
		dice_2004 = 2004,
		dice_2005 = 2005,
		dice_2006 = 2006,
		dice_2007 = 2007,
		dice_2008 = 2008,
		dice_2009 = 2009,
		dice_3000 = 3000,
		dice_3001 = 3001,
		dice_3002 = 3002,
		dice_3003 = 3003,
		dice_3004 = 3004,
		dice_3005 = 3005,
		dice_3006 = 3006,
		dice_3007 = 3007,
		dice_3008 = 3008,
		dice_3009 = 3009,
		dice_3010 = 3010,
		dice_3011 = 3011,
		dice_4000 = 4000,
		dice_4001 = 4001,
		dice_4002 = 4002,
		dice_4003 = 4003,
		dice_4004 = 4004,
		dice_4005 = 4005,
		dice_4006 = 4006,
		dice_4007 = 4007,
		dice_4008 = 4008,
		dice_4009 = 4009,
		dice_4010 = 4010,
		guardian_5001 = 5001,
		guardian_5002 = 5002,
		guardian_5003 = 5003,
	}

	public class TDataItemList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int itemType { get; set; }
		public int itemName_langId { get; set; }
		public string itemIcon { get; set; }
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
			itemName_langId = int.Parse(cols[3]);
			itemIcon = cols[4].Replace("{#$}", ",");
			boxOpenType = int.Parse(cols[5]);
			openKeyValue = int.Parse(cols[6]);
			productId = Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
			isUse = bool.Parse(cols[8]);
			maxValue = int.Parse(cols[9]);
		}
	}
}
