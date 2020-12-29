using System;

namespace Table.Data
{
	public class TDataItemList : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int itemType { get; set; }
		public string itemName_KR { get; set; }
		public string itemName_EN { get; set; }
		public string itemIcon { get; set; }
		public string itemText_KR { get; set; }
		public string itemText_EN { get; set; }
		public int boxOpenType { get; set; }
		public int openKeyValue { get; set; }
		public int[] productId { get; set; }
		public bool isUse { get; set; }
		public int maxValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1];
			itemType = int.Parse(cols[2]);
			itemName_KR = cols[3];
			itemName_EN = cols[4];
			itemIcon = cols[5];
			itemText_KR = cols[6];
			itemText_EN = cols[7];
			boxOpenType = int.Parse(cols[8]);
			openKeyValue = int.Parse(cols[9]);
			productId = Array.ConvertAll(cols[10].Split('|'), s => int.Parse(s));
			isUse = bool.Parse(cols[11]);
			maxValue = int.Parse(cols[12]);
		}
	}
}
