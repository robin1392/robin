using System;

namespace RandomWarsResource.Data
{
	public enum EOnedayShopListKey : int
	{
		None = -1,

		onedayshopgoods_01 = 3001,
		onedayshopgoods_02 = 3002,
		onedayshopgoods_03 = 3003,
		onedayshopgoods_04 = 3004,
		onedayshopgoods_05 = 3005,
		onedayshopgoods_06 = 3006,
		onedayshopgoods_07 = 3007,
		onedayshopgoods_08 = 3008,
		onedayshopgoods_09 = 3009,
		onedayshopgoods_10 = 3010,
		onedayshopgoods_11 = 3011,
		onedayshopgoods_12 = 3012,
		onedayshopgoods_13 = 3013,
		onedayshopgoods_14 = 3014,
		onedayshopgoods_15 = 3015,
		onedayshopgoods_16 = 3016,
		onedayshopgoods_17 = 3017,
		onedayshopgoods_18 = 3018,
		onedayshopgoods_19 = 3019,
		onedayshopgoods_20 = 3020,
		onedayshopgoods_21 = 3021,
		onedayshopgoods_22 = 3022,
		onedayshopgoods_23 = 3023,
		onedayshopgoods_24 = 3024,
		onedayshopgoods_25 = 3025,
		onedayshopgoods_26 = 3026,
	}

	public class TDataOnedayShopList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int goodsGroupId { get; set; }
		public int itemId { get; set; }
		public int itemValue { get; set; }
		public float showProbability { get; set; }
		public int buyType { get; set; }
		public int buyPrice { get; set; }
		public int sortIndex { get; set; }
		public bool isShow { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			goodsGroupId = int.Parse(cols[2]);
			itemId = int.Parse(cols[3]);
			itemValue = int.Parse(cols[4]);
			showProbability = float.Parse(cols[5]);
			buyType = int.Parse(cols[6]);
			buyPrice = int.Parse(cols[7]);
			sortIndex = int.Parse(cols[8]);
			isShow = bool.Parse(cols[9]);
		}
	}
}
