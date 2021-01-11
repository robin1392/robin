using System;

namespace RandomWarsResource.Data
{
	public enum EQuestDataKey : int
	{
		None = -1,

		questlist01 = 1,
		questlist02 = 2,
		questlist03 = 3,
		questlist04 = 4,
		questlist05 = 5,
		questlist06 = 6,
		questlist07 = 7,
		questlist08 = 8,
		questlist09 = 9,
		questlist10 = 10,
		questlist11 = 11,
		questlist12 = 12,
		questlist13 = 13,
		questlist14 = 14,
		questlist15 = 15,
		questlist16 = 16,
		questlist17 = 17,
		questlist18 = 18,
		questlist19 = 101,
		questlist20 = 102,
		questlist21 = 103,
		questlist22 = 104,
		questlist23 = 105,
		questlist24 = 106,
		questlist25 = 107,
		questlist26 = 108,
		questlist27 = 109,
		questlist28 = 110,
		questlist29 = 111,
		questlist30 = 112,
		questlist31 = 113,
		questlist32 = 114,
		questlist33 = 115,
		questlist34 = 116,
		questlist35 = 117,
		questlist36 = 118,
		questlist1000 = 1000,
	}

	public class TDataQuestData : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int questGroup { get; set; }
		public int questType { get; set; }
		public int questEndValue { get; set; }
		public int ItemId { get; set; }
		public int ItemValue { get; set; }
		public string questStringKey { get; set; }
		public bool isUse { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			questGroup = int.Parse(cols[2]);
			questType = int.Parse(cols[3]);
			questEndValue = int.Parse(cols[4]);
			ItemId = int.Parse(cols[5]);
			ItemValue = int.Parse(cols[6]);
			questStringKey = cols[7].Replace("{#$}", ",");
			isUse = bool.Parse(cols[8]);
		}
	}
}
