using System;

namespace RandomWarsResource.Data
{
	public enum EClassRewardKey : int
	{
		None = -1,

		classreward01 = 1,
		classreward02 = 2,
		classreward03 = 3,
		classreward04 = 4,
		classreward05 = 5,
		classreward06 = 6,
		classreward07 = 7,
		classreward08 = 8,
		classreward09 = 9,
		classreward10 = 10,
		classreward11 = 11,
		classreward12 = 12,
		classreward13 = 13,
		classreward14 = 14,
		classreward15 = 15,
		classreward16 = 16,
		classreward17 = 17,
		classreward18 = 18,
		classreward19 = 19,
		classreward20 = 20,
		classreward21 = 21,
		classreward22 = 22,
		classreward23 = 23,
		classreward24 = 24,
		classreward25 = 25,
		classreward26 = 26,
		classreward27 = 27,
		classreward28 = 28,
		classreward29 = 29,
		classreward30 = 30,
		classreward31 = 31,
		classreward32 = 32,
		classreward33 = 33,
		classreward34 = 34,
		classreward35 = 35,
		classreward36 = 36,
		classreward37 = 37,
		classreward38 = 38,
		classreward39 = 39,
		classreward40 = 40,
		classreward41 = 41,
		classreward42 = 42,
		classreward43 = 43,
		classreward44 = 44,
		classreward45 = 45,
		classreward46 = 46,
		classreward47 = 47,
		classreward48 = 48,
		classreward49 = 49,
		classreward50 = 50,
		classreward51 = 51,
		classreward52 = 52,
		classreward53 = 53,
		classreward54 = 54,
		classreward55 = 55,
		classreward56 = 56,
		classreward57 = 57,
		classreward58 = 58,
		classreward59 = 59,
		classreward60 = 60,
		classreward61 = 61,
		classreward62 = 62,
		classreward63 = 63,
		classreward64 = 64,
		classreward65 = 65,
		classreward66 = 66,
		classreward67 = 67,
		classreward68 = 68,
		classreward69 = 69,
		classreward70 = 70,
	}

	public class TDataClassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int rankPoint { get; set; }
		public int rewardTargetType01 { get; set; }
		public int rewardItem01 { get; set; }
		public int rewardItemValue01 { get; set; }
		public int rewardTargetType02 { get; set; }
		public int rewardItem02 { get; set; }
		public int rewardItemValue02 { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			rankPoint = int.Parse(cols[2]);
			rewardTargetType01 = int.Parse(cols[3]);
			rewardItem01 = int.Parse(cols[4]);
			rewardItemValue01 = int.Parse(cols[5]);
			rewardTargetType02 = int.Parse(cols[6]);
			rewardItem02 = int.Parse(cols[7]);
			rewardItemValue02 = int.Parse(cols[8]);
		}
	}
}
