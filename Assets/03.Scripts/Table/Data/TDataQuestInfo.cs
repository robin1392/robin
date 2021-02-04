using System;

namespace RandomWarsResource.Data
{
	public enum EQuestInfoKey : int
	{
		None = -1,

		playDeathMatch = 0,
		playCoopMatch = 1,
		playAllMatch = 2,
		winAllMatch = 3,
		openBox = 11,
		buyShop = 12,
		useGold = 13,
		useDiamond = 14,
		upgradeDice = 15,
		spawnGuardian = 16,
		getKey = 17,
		getBox = 18,
		killAllBoss = 31,
		killBoss_1 = 32,
		killBoss_2 = 33,
		killBoss_3 = 34,
		killBoss_4 = 35,
		killBoss_5 = 36,
		viewAd = 999,
		completeAllQuest = 1000,
	}

	public class TDataQuestInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
		}
	}
}
