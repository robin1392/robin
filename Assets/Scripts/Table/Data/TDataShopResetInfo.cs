using System;

namespace RandomWarsResource.Data
{
	public enum EShopResetInfoKey : int
	{
		None = -1,

		resetTime = 1,
		adResetLimitCnt = 2,
		adResetBuyType = 3,
		adResetBuyValue = 4,
		resetBuyLimitCnt = 5,
		resetBuyType = 6,
		resetBuyValue01 = 7,
		resetBuyValue02 = 8,
		resetBuyValue03 = 9,
		resetBuyValue04 = 10,
	}

	public class TDataShopResetInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int settingValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			settingValue = int.Parse(cols[2]);
		}
	}
}
