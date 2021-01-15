using System;

namespace RandomWarsResource.Data
{
	public enum EDiaShopListKey : int
	{
		None = -1,

		diashopgoods_01 = 7001,
		diashopgoods_02 = 7002,
		diashopgoods_03 = 7003,
		diashopgoods_04 = 7004,
		diashopgoods_05 = 7005,
		diashopgoods_06 = 7006,
		diashopgoods_07 = 7007,
		diashopgoods_08 = 7008,
		diashopgoods_09 = 7009,
		diashopgoods_10 = 7010,
		diashopgoods_11 = 7011,
		diashopgoods_12 = 7012,
	}

	public class TDataDiaShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string goodsName { get; set; }
		public string goodsImage { get; set; }
		public int itemId { get; set; }
		public int itemValue { get; set; }
		public int buyType { get; set; }
		public int buyPrice { get; set; }
		public int priceNotice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int sortIndex { get; set; }
		public int tapInfo { get; set; }
		public int tapValue { get; set; }
		public bool isDiscount { get; set; }
		public int discountValue { get; set; }
		public int multipleValue { get; set; }
		public bool isFirstBuy { get; set; }
		public bool isShow { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			goodsName = cols[2].Replace("{#$}", ",");
			goodsImage = cols[3].Replace("{#$}", ",");
			itemId = int.Parse(cols[4]);
			itemValue = int.Parse(cols[5]);
			buyType = int.Parse(cols[6]);
			buyPrice = int.Parse(cols[7]);
			priceNotice = int.Parse(cols[8]);
			appleProductId = cols[9].Replace("{#$}", ",");
			googleProductId = cols[10].Replace("{#$}", ",");
			sortIndex = int.Parse(cols[11]);
			tapInfo = int.Parse(cols[12]);
			tapValue = int.Parse(cols[13]);
			isDiscount = bool.Parse(cols[14]);
			discountValue = int.Parse(cols[15]);
			multipleValue = int.Parse(cols[16]);
			isFirstBuy = bool.Parse(cols[17]);
			isShow = bool.Parse(cols[18]);
		}
	}
}
