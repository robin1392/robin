using System;

namespace RandomWarsResource.Data
{
	public enum ESeasonpassRewardKey : int
	{
		None = -1,

		season_normalreward01 = 1,
		season_normalreward02 = 2,
		season_normalreward03 = 3,
		season_normalreward04 = 4,
		season_normalreward05 = 5,
		season_normalreward06 = 6,
		season_normalreward07 = 7,
		season_normalreward08 = 8,
		season_normalreward09 = 9,
		season_normalreward10 = 10,
		seson_passreward01 = 1001,
		seson_passreward02 = 1002,
		seson_passreward03 = 1003,
		seson_passreward04 = 1004,
		seson_passreward05 = 1005,
		seson_passreward06 = 1006,
		seson_passreward07 = 1007,
		seson_passreward08 = 1008,
		seson_passreward09 = 1009,
		seson_passreward10 = 1010,
	}

	public class TDataSeasonpassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int trophyPoint { get; set; }
		public int rewardType { get; set; }
		public bool effectOn { get; set; }
		public int rewardItem { get; set; }
		public int rewardItemValue { get; set; }
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
			rewardType = int.Parse(cols[3]);
			effectOn = bool.Parse(cols[4]);
			rewardItem = int.Parse(cols[5]);
			rewardItemValue = int.Parse(cols[6]);
			rewardBuyType = int.Parse(cols[7]);
			rewardBuyPrice = int.Parse(cols[8]);
		}
	}
}
