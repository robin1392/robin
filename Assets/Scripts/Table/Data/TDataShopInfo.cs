using System;

namespace RandomWarsResource.Data
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
		public int chooseType { get; set; }
		public int sortIndex { get; set; }
		public int[] shopGroupId { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public bool isReset { get; set; }
		public bool isShow { get; set; }


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
			chooseType = int.Parse(cols[4]);
			sortIndex = int.Parse(cols[5]);
			shopGroupId = Array.ConvertAll(cols[6].Split('|'), s => int.Parse(s));
			startDate = (cols[7]== " -1") ? default(DateTime) : DateTime.Parse(cols[7]);
			endDate = (cols[8]== " -1") ? default(DateTime) : DateTime.Parse(cols[8]);
			isReset = bool.Parse(cols[9]);
			isShow = bool.Parse(cols[10]);
		}
	}
}
