using System;
using Service.Template.Table;

namespace Template.Shop.GameBaseShop.Table
{
	public enum EShopInfoKey : int
	{
		None = -1,

		event_shop = 1,
		package_shop = 2,
		oneday_shop = 3,
		box_shop = 4,
		premium_shop = 5,
		emotion_shop = 6,
		dia_shop = 7,
		gold_shop = 8,
	}

	public class TDataShopInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string shopTitle { get; set; }
		public string linksheet { get; set; }
		public int maxCount { get; set; }
		public int chooseType { get; set; }
		public int sortIndex { get; set; }
		public int[] shopGroupId { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public bool isReset { get; set; }
		public bool isShow { get; set; }
		public int resetTime { get; set; }
		public int[] resetBuyType { get; set; }
		public int[] resetBuyValue { get; set; }
		public int resetAdType { get; set; }
		public int resetAdValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			shopTitle = cols[2].Replace("{#$}", ",");
			linksheet = cols[3].Replace("{#$}", ",");
			maxCount = int.Parse(cols[4]);
			chooseType = int.Parse(cols[5]);
			sortIndex = int.Parse(cols[6]);
			shopGroupId = (cols[7] == "-1") ? null : Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
			startDate = (cols[8] == "-1") ? default(DateTime) : DateTime.Parse(cols[8]);
			endDate = (cols[9] == "-1") ? default(DateTime) : DateTime.Parse(cols[9]);
			isReset = bool.Parse(cols[10]);
			isShow = bool.Parse(cols[11]);
			resetTime = int.Parse(cols[12]);
			resetBuyType = (cols[13] == "-1") ? null : Array.ConvertAll(cols[13].Split('|'), s => int.Parse(s));
			resetBuyValue = (cols[14] == "-1") ? null : Array.ConvertAll(cols[14].Split('|'), s => int.Parse(s));
			resetAdType = int.Parse(cols[15]);
			resetAdValue = int.Parse(cols[16]);
		}
	}
}
