using System;

namespace RandomWarsResource.Data
{
	public enum EQuestDayRewardKey : int
	{
		None = -1,

		dayreward01 = 0,
		dayreward02 = 1,
		dayreward03 = 2,
		dayreward04 = 3,
		dayreward05 = 4,
		dayreward06 = 5,
		dayreward07 = 6,
		dayreward08 = 7,
		dayreward09 = 8,
		dayreward10 = 9,
		dayreward11 = 10,
		dayreward12 = 11,
		dayreward13 = 12,
		dayreward14 = 13,
		dayreward15 = 14,
		dayreward16 = 15,
		dayreward17 = 16,
		dayreward18 = 17,
		dayreward19 = 18,
		dayreward20 = 19,
	}

	public class TDataQuestDayReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int userRank { get; set; }
		public int rewardItem01 { get; set; }
		public int rewardItemValue01 { get; set; }
		public int rewardItem02 { get; set; }
		public int rewardItemValue02 { get; set; }
		public int[] rewardItem03 { get; set; }
		public int rewardItemValue03 { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			userRank = int.Parse(cols[2]);
			rewardItem01 = int.Parse(cols[3]);
			rewardItemValue01 = int.Parse(cols[4]);
			rewardItem02 = int.Parse(cols[5]);
			rewardItemValue02 = int.Parse(cols[6]);
			rewardItem03 = Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
			rewardItemValue03 = int.Parse(cols[8]);
		}
	}
}
