using System;

namespace RandomWarsResource.Data
{
	public enum EPremiumShopListKey : int
	{
		None = -1,

		premiumgoods_01 = 5001,
	}

	public class TDataPremiumShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string premiumName { get; set; }
		public string shopImage { get; set; }
		public int buyType { get; set; }
		public int buyPrice { get; set; }
		public int priceNotice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int itemId { get; set; }
		public int itemValue { get; set; }
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
			premiumName = cols[2].Replace("{#$}", ",");
			shopImage = cols[3].Replace("{#$}", ",");
			buyType = int.Parse(cols[4]);
			buyPrice = int.Parse(cols[5]);
			priceNotice = int.Parse(cols[6]);
			appleProductId = cols[7].Replace("{#$}", ",");
			googleProductId = cols[8].Replace("{#$}", ",");
			itemId = int.Parse(cols[9]);
			itemValue = int.Parse(cols[10]);
			sortIndex = int.Parse(cols[11]);
			tapInfo = int.Parse(cols[12]);
			tapValue = int.Parse(cols[13]);
			isDiscount = bool.Parse(cols[14]);
			discountValue = int.Parse(cols[15]);
			isUnique = bool.Parse(cols[16]);
			isShow = bool.Parse(cols[17]);
		}
	}
}
