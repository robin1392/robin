using System;

namespace RandomWarsResource.Data
{
	public class TDataRakingReward : ITableData<int>
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
			name = cols[1];
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
