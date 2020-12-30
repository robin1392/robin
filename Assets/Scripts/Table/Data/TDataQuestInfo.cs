using System;

namespace RandomWarsResource.Data
{
	public enum EQuestInfoKey : int
	{
		None = -1,

		questtype00 = 0,
		questtype01 = 1,
		questtype02 = 2,
		questtype03 = 3,
		questtype04 = 4,
		questtype05 = 5,
		questtype06 = 6,
		questtype07 = 7,
		questtype08 = 8,
		questtype09 = 9,
		questtype10 = 10,
		questtype11 = 11,
		questtype12 = 12,
		questtype13 = 13,
		questtype14 = 14,
		questtype15 = 15,
		questtype16 = 16,
		questtype17 = 17,
		questtype18 = 18,
		questtype19 = 19,
		questtype20 = 20,
		questtype21 = 21,
		questtype22 = 22,
		questtype23 = 23,
		questtype24 = 24,
		questtype25 = 25,
		questtype26 = 26,
		questtype27 = 27,
		questtype28 = 28,
		questtype29 = 29,
		questtype30 = 30,
		questtype31 = 31,
		questtype32 = 32,
		questtype33 = 33,
		questtype1000 = 1000,
	}

	public class TDataQuestInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public bool vipQuest { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			vipQuest = bool.Parse(cols[2]);
		}
	}
}
