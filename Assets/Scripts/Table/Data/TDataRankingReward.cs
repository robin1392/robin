using System;

namespace RandomWarsResource.Data
{
	public enum ERankingRewardKey : int
	{
		None = -1,

		rankingreward01 = 1,
		rankingreward02 = 2,
		rankingreward03 = 3,
		rankingreward04 = 4,
		rankingreward05 = 5,
		rankingreward06 = 6,
		rankingreward07 = 7,
		rankingreward08 = 8,
		rankingreward09 = 9,
		rankingreward10 = 10,
		rankingreward11 = 11,
		rankingreward12 = 12,
		rankingreward13 = 13,
		rankingreward14 = 14,
		rankingreward15 = 15,
		rankingreward16 = 16,
		rankingreward17 = 17,
		rankingreward18 = 18,
		rankingreward19 = 19,
		rankingreward20 = 20,
		rankingreward21 = 21,
		rankingreward22 = 22,
	}

	public class TDataRankingReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int rankRewardType { get; set; }
		public int rankMin { get; set; }
		public int rankMax { get; set; }
		public int rankRewardItem01 { get; set; }
		public int rankRewardIValue01 { get; set; }
		public int rankRewardItem02 { get; set; }
		public int rankRewardIValue02 { get; set; }
		public int rankRewardItem03 { get; set; }
		public int rankRewardIValue03 { get; set; }
		public int rankRewardItem04 { get; set; }
		public int rankRewardIValue04 { get; set; }
		public int rankRewardItem05 { get; set; }
		public int rankRewardIValue05 { get; set; }
		public int rankRestartPoint { get; set; }
		public int rankRangeGroup { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			rankRewardType = int.Parse(cols[2]);
			rankMin = int.Parse(cols[3]);
			rankMax = int.Parse(cols[4]);
			rankRewardItem01 = int.Parse(cols[5]);
			rankRewardIValue01 = int.Parse(cols[6]);
			rankRewardItem02 = int.Parse(cols[7]);
			rankRewardIValue02 = int.Parse(cols[8]);
			rankRewardItem03 = int.Parse(cols[9]);
			rankRewardIValue03 = int.Parse(cols[10]);
			rankRewardItem04 = int.Parse(cols[11]);
			rankRewardIValue04 = int.Parse(cols[12]);
			rankRewardItem05 = int.Parse(cols[13]);
			rankRewardIValue05 = int.Parse(cols[14]);
			rankRestartPoint = int.Parse(cols[15]);
			rankRangeGroup = int.Parse(cols[16]);
		}
	}
}
