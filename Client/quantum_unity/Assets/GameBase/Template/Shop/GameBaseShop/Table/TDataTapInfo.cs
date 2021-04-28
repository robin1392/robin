using System;
using Service.Template.Table;

namespace Template.Shop.GameBaseShop.Table
{
	public enum ETapInfoKey : int
	{
		None = -1,

		tap_hot = 1,
		tap_best = 2,
		tap_new = 3,
		tap_oneplusone = 4,
		tap_value = 5,
	}

	public class TDataTapInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string taptext { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			taptext = cols[2].Replace("{#$}", ",");
		}
	}
}
