using System;

namespace RandomWarsResource.Data
{
	public enum EBuyTypeKey : int
	{
		None = -1,

		buy_gold = 1,
		buy_dia = 2,
		buy_realmoney = 3,
		buy_free = 4,
		buy_adfree = 5,
	}

	public class TDataBuyType : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
		}
	}
}
