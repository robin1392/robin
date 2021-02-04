using System;

namespace RandomWarsResource.Data
{
	public enum EEmotionShopListKey : int
	{
		None = -1,

		emotiongoods_01 = 6001,
		emotiongoods_02 = 6002,
		emotiongoods_03 = 6003,
		emotiongoods_04 = 6004,
		emotiongoods_05 = 6005,
		emotiongoods_06 = 6006,
	}

	public class TDataEmotionShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
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
			priceNotice = int.Parse(cols[6]);
			sortIndex = int.Parse(cols[7]);
			tapInfo = int.Parse(cols[8]);
			tapValue = int.Parse(cols[9]);
			isDiscount = bool.Parse(cols[10]);
			discountValue = int.Parse(cols[11]);
			isUnique = bool.Parse(cols[12]);
			isShow = bool.Parse(cols[13]);
		}
	}
}
