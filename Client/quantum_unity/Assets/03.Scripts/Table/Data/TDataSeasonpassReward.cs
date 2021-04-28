using System;

namespace RandomWarsResource.Data
{
	public enum ESeasonpassRewardKey : int
	{
		None = -1,

		season_reward01 = 1,
		season_reward02 = 2,
		season_reward03 = 3,
		season_reward04 = 4,
		season_reward05 = 5,
		season_reward06 = 6,
		season_reward07 = 7,
		season_reward08 = 8,
		season_reward09 = 9,
		season_reward10 = 10,
		season_reward11 = 11,
	}

	public class TDataSeasonpassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int trophyPoint { get; set; }
		public int rewardTargetType01 { get; set; }
		public int rewardItem01 { get; set; }
		public int rewardItemValue01 { get; set; }
		public bool effectOn01 { get; set; }
		public int rewardTargetType02 { get; set; }
		public int rewardItem02 { get; set; }
		public int rewardItemValue02 { get; set; }
		public bool effectOn02 { get; set; }
		public int rewardBuyType { get; set; }
		public int rewardBuyPrice { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			trophyPoint = int.Parse(cols[2]);
			rewardTargetType01 = int.Parse(cols[3]);
			rewardItem01 = int.Parse(cols[4]);
			rewardItemValue01 = int.Parse(cols[5]);
			effectOn01 = bool.Parse(cols[6]);
			rewardTargetType02 = int.Parse(cols[7]);
			rewardItem02 = int.Parse(cols[8]);
			rewardItemValue02 = int.Parse(cols[9]);
			effectOn02 = bool.Parse(cols[10]);
			rewardBuyType = int.Parse(cols[11]);
			rewardBuyPrice = int.Parse(cols[12]);
		}
	}
}
