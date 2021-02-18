using System;

namespace RandomWarsResource.Data
{
	public class TDataShopProductList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int shopId { get; set; }
		public int goodsGroupId { get; set; }
		public int itemGroup { get; set; }
		public string packageName { get; set; }
		public string shopImage { get; set; }
		public int buyType { get; set; }
		public float buyPrice { get; set; }
		public string priceNotice { get; set; }
		public string appleProductId { get; set; }
		public string googleProductId { get; set; }
		public int itemId01 { get; set; }
		public int itemValue01 { get; set; }
		public int showProbability01 { get; set; }
		public int itemId02 { get; set; }
		public int itemValue02 { get; set; }
		public int showProbability02 { get; set; }
		public int itemId03 { get; set; }
		public int itemValue03 { get; set; }
		public int showProbability03 { get; set; }
		public int itemId04 { get; set; }
		public int itemValue04 { get; set; }
		public int showProbability04 { get; set; }
		public int itemId05 { get; set; }
		public int itemValue05 { get; set; }
		public int showProbability05 { get; set; }
		public int itemId06 { get; set; }
		public int itemValue06 { get; set; }
		public int showProbability06 { get; set; }
		public int buyLimitCnt { get; set; }
		public int tapInfo { get; set; }
		public int tapValue { get; set; }
		public bool isDiscount { get; set; }
		public int discountValue { get; set; }
		public int sortIndex { get; set; }
		public string guideInfoLink { get; set; }
		public bool isUnique { get; set; }
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
			shopId = int.Parse(cols[2]);
			goodsGroupId = int.Parse(cols[3]);
			itemGroup = int.Parse(cols[4]);
			packageName = cols[5].Replace("{#$}", ",");
			shopImage = cols[6].Replace("{#$}", ",");
			buyType = int.Parse(cols[7]);
			buyPrice = float.Parse(cols[8]);
			priceNotice = cols[9].Replace("{#$}", ",");
			appleProductId = cols[10].Replace("{#$}", ",");
			googleProductId = cols[11].Replace("{#$}", ",");
			itemId01 = int.Parse(cols[12]);
			itemValue01 = int.Parse(cols[13]);
			showProbability01 = int.Parse(cols[14]);
			itemId02 = int.Parse(cols[15]);
			itemValue02 = int.Parse(cols[16]);
			showProbability02 = int.Parse(cols[17]);
			itemId03 = int.Parse(cols[18]);
			itemValue03 = int.Parse(cols[19]);
			showProbability03 = int.Parse(cols[20]);
			itemId04 = int.Parse(cols[21]);
			itemValue04 = int.Parse(cols[22]);
			showProbability04 = int.Parse(cols[23]);
			itemId05 = int.Parse(cols[24]);
			itemValue05 = int.Parse(cols[25]);
			showProbability05 = int.Parse(cols[26]);
			itemId06 = int.Parse(cols[27]);
			itemValue06 = int.Parse(cols[28]);
			showProbability06 = int.Parse(cols[29]);
			buyLimitCnt = int.Parse(cols[30]);
			tapInfo = int.Parse(cols[31]);
			tapValue = int.Parse(cols[32]);
			isDiscount = bool.Parse(cols[33]);
			discountValue = int.Parse(cols[34]);
			sortIndex = int.Parse(cols[35]);
			guideInfoLink = cols[36].Replace("{#$}", ",");
			isUnique = bool.Parse(cols[37]);
			multipleValue = int.Parse(cols[38]);
			buyPreviousItemId = int.Parse(cols[39]);
			isShow = bool.Parse(cols[40]);
		}
	}
}
