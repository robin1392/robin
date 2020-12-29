using System;

namespace RandomWarsResource.Data
{
	public class TDataClassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int rankPoint { get; set; }
		public int rewardType { get; set; }
		public int rewardGold { get; set; }
		public int rewardDia { get; set; }
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
			rankPoint = int.Parse(cols[2]);
			rewardType = int.Parse(cols[3]);
			rewardGold = int.Parse(cols[4]);
			rewardDia = int.Parse(cols[5]);
			rewardItem = int.Parse(cols[6]);
			rewardItemValue = int.Parse(cols[7]);
		}
	}
}
