using System;

namespace RandomWarsResource.Data
{
	public class TDataSeasonpassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int trophyPoint { get; set; }
		public int rewardType { get; set; }
		public bool effectOn { get; set; }
		public int rewardItem { get; set; }
		public int rewardItemValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{$}", ",");
			trophyPoint = int.Parse(cols[2]);
			rewardType = int.Parse(cols[3]);
			effectOn = bool.Parse(cols[4]);
			rewardItem = int.Parse(cols[5]);
			rewardItemValue = int.Parse(cols[6]);
		}
	}
}
