using System;

namespace RandomWarsResource.Data
{
	public enum EVsmodeKey : int
	{
		None = -1,

		StartCoolTime = 1,
		WaveTime = 2,
		GetStartSP = 3,
		AddSP = 4,
		GetStartDiceCost = 5,
		DiceCostUp = 6,
		TowerHp = 7,
		GetDefenderTowerHp = 8,
		DicePowerUpCost01 = 9,
		DicePowerUpCost02 = 10,
		DicePowerUpCost03 = 11,
		DicePowerUpCost04 = 12,
		DicePowerUpCost05 = 13,
		StartSuddenDeathWave = 14,
		SuddenDeathAtkUp = 15,
		SuddenDeathDefUp = 16,
		GetSPPlusLevelupCost01 = 17,
		GetSPPlusLevelupCost02 = 18,
		GetSPPlusLevelupCost03 = 19,
		GetSPPlusLevelupCost04 = 20,
		GetSPPlusLevelupCost05 = 21,
		NormalWinReward_Gold = 22,
		NormalWinReward_Key = 23,
		PerfectReward_Gold = 24,
		PerfectWinReward_Key = 25,
		EndWave = 26,
		LoseUserRewardId = 27,
		LoseUserRewardValue = 28,
		LoseUserCondition01 = 29,
		LoseUserCondition02 = 30,
		LoseUserCondition03 = 31,
	}

	public class TDataVsmode : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int value { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			value = int.Parse(cols[2]);
		}
	}
}
