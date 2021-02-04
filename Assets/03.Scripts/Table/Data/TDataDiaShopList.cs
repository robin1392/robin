using System;

namespace RandomWarsResource.Data
{
	public enum EDiaShopListKey : int
	{
		None = -1,

		diashopgoods01 = 7001,
		diashopgoods02 = 7002,
		diashopgoods03 = 7003,
		diashopgoods04 = 7004,
		diashopgoods05 = 7005,
		diashopgoods06 = 7006,
		diashopgoods07 = 7007,
		diashopgoods08 = 7008,
		diashopgoods09 = 7009,
		diashopgoods10 = 7010,
		diashopgoods11 = 7011,
		diashopgoods12 = 7012,
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
		public float buyPrice { get; set; }
		public int priceNotice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int sortIndex { get; set; }
		public int tapInfo { get; set; }
		public int tapValue { get; set; }
		public bool isDiscount { get; set; }
		public int discountValue { get; set; }
		public int multipleValue { get; set; }
		public int buyPreviousItemId { get; set; }
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
			buyPrice = float.Parse(cols[7]);
			priceNotice = int.Parse(cols[8]);
			appleProductId = cols[9].Replace("{#$}", ",");
			googleProductId = cols[10].Replace("{#$}", ",");
			sortIndex = int.Parse(cols[11]);
			tapInfo = int.Parse(cols[12]);
			tapValue = int.Parse(cols[13]);
			isDiscount = bool.Parse(cols[14]);
			discountValue = int.Parse(cols[15]);
			multipleValue = int.Parse(cols[16]);
			buyPreviousItemId = int.Parse(cols[17]);
			isShow = bool.Parse(cols[18]);
		}
	}
}
