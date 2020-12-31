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
		questlist19 = 19,
		questlist20 = 20,
		questlist21 = 21,
		questlist22 = 22,
		questlist23 = 23,
		questlist24 = 24,
		questlist25 = 25,
		questlist26 = 26,
		questlist27 = 27,
		questlist28 = 28,
		questlist29 = 29,
		questlist30 = 30,
		questlist31 = 31,
		questlist32 = 32,
		questlist33 = 33,
		questlist34 = 34,
		questlist1000 = 1000,
	}

	public class TDataQuestData : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int questGroup { get; set; }
		public int questType { get; set; }
		public int questEndValue { get; set; }
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
			questStringKey = cols[5].Replace("{#$}", ",");
			isUse = bool.Parse(cols[6]);
		}
	}
}