using System;

namespace RandomWarsResource.Data
{
	public enum ECoopModeKey : int
	{
		None = -1,

		coopmodePlayCntMax = 1,
		coopmodeADCnt = 2,
		serachuserTropy = 3,
		serachuserTropyMaxCnt = 4,
		maxWave = 5,
		coopTowerHp = 6,
		bossID_1st = 7,
		bossID_2nd = 8,
		bossID_3rd = 9,
		bossID_4th = 10,
		bossID_5th = 11,
		callGuardianTowerHp_1st = 12,
		callGuardianTowerHp_2nd = 13,
		get1stBoss_Wave = 14,
		get2ndBoss_Wave = 15,
		get3rdBoss_Wave = 16,
		get4thBoss_Wave = 17,
		get5thBoss_Wave = 18,
		eggtobossCoolTime = 19,
		basicrewardWave = 20,
		basicrewardID = 21,
		basicrewardValue = 22,
		bossRewardID01 = 23,
		bossRewardID02 = 24,
		bossRewardID03 = 25,
		bossRewardID04 = 26,
		bossRewardID05 = 27,
		GetStartSP = 28,
		AddSP = 29,
		GetStartDiceCost = 30,
		DiceCostUp = 31,
		DefaultSp = 32,
		UpgradeSp = 33,
		WaveSp = 34,
	}

	public class TDataCoopMode : ITableData<int>
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
