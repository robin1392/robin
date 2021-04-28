using System;
using Service.Template.Table;

namespace Template.Shop.GameBaseShop.Table
{
	public enum EGoldShopListKey : int
	{
		None = -1,

		goldshopgoods_01 = 8001,
		goldshopgoods_02 = 8002,
		goldshopgoods_03 = 8003,
		goldshopgoods_04 = 8004,
		goldshopgoods_05 = 8005,
		goldshopgoods_06 = 8006,
	}

	public class TDataGoldShopList : ITableData<int>
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
			sortIndex = int.Parse(cols[9]);
			tapInfo = int.Parse(cols[10]);
			tapValue = int.Parse(cols[11]);
			isDiscount = bool.Parse(cols[12]);
			discountValue = int.Parse(cols[13]);
			multipleValue = int.Parse(cols[14]);
			isFirstBuy = bool.Parse(cols[15]);
			isShow = bool.Parse(cols[16]);
		}
	}
}
