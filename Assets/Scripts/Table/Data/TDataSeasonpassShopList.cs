using System;

namespace RandomWarsResource.Data
{
	public enum ESeasonpassShopListKey : int
	{
		None = -1,

		seasonpassgoods_01 = 9001,
	}

	public class TDataSeasonpassShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int itemId { get; set; }
		public int itemValue { get; set; }
		public int buyType { get; set; }
		public int buyPrice { get; set; }
		public int priceNptice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int sortIndex { get; set; }
		public int tapInfo { get; set; }
		public int tapValue { get; set; }
		public bool isDiscount { get; set; }
		public int discountValue { get; set; }
		public bool isUnique { get; set; }
		public bool isShow { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			itemId = int.Parse(cols[2]);
			itemValue = int.Parse(cols[3]);
			buyType = int.Parse(cols[4]);
			buyPrice = int.Parse(cols[5]);
			priceNptice = int.Parse(cols[6]);
			appleProductId = cols[7].Replace("{#$}", ",");
			googleProductId = cols[8].Replace("{#$}", ",");
			sortIndex = int.Parse(cols[9]);
			tapInfo = int.Parse(cols[10]);
			tapValue = int.Parse(cols[11]);
			isDiscount = bool.Parse(cols[12]);
			discountValue = int.Parse(cols[13]);
			isUnique = bool.Parse(cols[14]);
			isShow = bool.Parse(cols[15]);
		}
	}
}
