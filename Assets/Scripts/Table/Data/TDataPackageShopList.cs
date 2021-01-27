using System;

namespace RandomWarsResource.Data
{
	public enum EPackageShopListKey : int
	{
		None = -1,

		packageshopgoods01 = 2001,
		packageshopgoods02 = 2002,
		packageshopgoods03 = 2003,
		packageshopgoods04 = 2004,
		packageshopgoods05 = 2005,
		packageshopgoods06 = 2006,
		packageshopgoods07 = 2007,
	}

	public class TDataPackageShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string packageName { get; set; }
		public string shopImage { get; set; }
		public int buyType { get; set; }
		public float buyPrice { get; set; }
		public int priceNotice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int itemId01 { get; set; }
		public int itemValue01 { get; set; }
		public int itemId02 { get; set; }
		public int itemValue02 { get; set; }
		public int itemId03 { get; set; }
		public int itemValue03 { get; set; }
		public int itemId04 { get; set; }
		public int itemValue04 { get; set; }
		public int itemId05 { get; set; }
		public int itemValue05 { get; set; }
		public int itemId06 { get; set; }
		public int itemValue06 { get; set; }
		public int buyPreviousItemId { get; set; }
		public int buyLimitCnt { get; set; }
		public int tapInfo { get; set; }
		public int tapValue { get; set; }
		public bool isDiscount { get; set; }
		public int discountValue { get; set; }
		public bool isShow { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			packageName = cols[2].Replace("{#$}", ",");
			shopImage = cols[3].Replace("{#$}", ",");
			buyType = int.Parse(cols[4]);
			buyPrice = float.Parse(cols[5]);
			priceNotice = int.Parse(cols[6]);
			appleProductId = cols[7].Replace("{#$}", ",");
			googleProductId = cols[8].Replace("{#$}", ",");
			itemId01 = int.Parse(cols[9]);
			itemValue01 = int.Parse(cols[10]);
			itemId02 = int.Parse(cols[11]);
			itemValue02 = int.Parse(cols[12]);
			itemId03 = int.Parse(cols[13]);
			itemValue03 = int.Parse(cols[14]);
			itemId04 = int.Parse(cols[15]);
			itemValue04 = int.Parse(cols[16]);
			itemId05 = int.Parse(cols[17]);
			itemValue05 = int.Parse(cols[18]);
			itemId06 = int.Parse(cols[19]);
			itemValue06 = int.Parse(cols[20]);
			buyPreviousItemId = int.Parse(cols[21]);
			buyLimitCnt = int.Parse(cols[22]);
			tapInfo = int.Parse(cols[23]);
			tapValue = int.Parse(cols[24]);
			isDiscount = bool.Parse(cols[25]);
			discountValue = int.Parse(cols[26]);
			isShow = bool.Parse(cols[27]);
		}
	}
}
