using System;

namespace RandomWarsResource.Data
{
	public enum EClassRewardKey : int
	{
		None = -1,

		normalreward01 = 1,
		normalreward02 = 2,
		normalreward03 = 3,
		normalreward04 = 4,
		normalreward05 = 5,
		normalreward06 = 6,
		normalreward07 = 7,
		normalreward08 = 8,
		normalreward09 = 9,
		normalreward10 = 10,
		normalreward11 = 11,
		normalreward12 = 12,
		normalreward13 = 13,
		normalreward14 = 14,
		normalreward15 = 15,
		normalreward16 = 16,
		normalreward17 = 17,
		normalreward18 = 18,
		normalreward19 = 19,
		normalreward20 = 20,
		normalreward21 = 21,
		normalreward22 = 22,
		normalreward23 = 23,
		normalreward24 = 24,
		normalreward25 = 25,
		normalreward26 = 26,
		normalreward27 = 27,
		normalreward28 = 28,
		normalreward29 = 29,
		normalreward30 = 30,
		normalreward31 = 31,
		normalreward32 = 32,
		normalreward33 = 33,
		normalreward34 = 34,
		normalreward35 = 35,
		normalreward36 = 36,
		normalreward37 = 37,
		normalreward38 = 38,
		normalreward39 = 39,
		normalreward40 = 40,
		normalreward41 = 41,
		normalreward42 = 42,
		normalreward43 = 43,
		normalreward44 = 44,
		normalreward45 = 45,
		normalreward46 = 46,
		normalreward47 = 47,
		normalreward48 = 48,
		normalreward49 = 49,
		normalreward50 = 50,
		normalreward51 = 51,
		normalreward52 = 52,
		normalreward53 = 53,
		normalreward54 = 54,
		normalreward55 = 55,
		normalreward56 = 56,
		normalreward57 = 57,
		normalreward58 = 58,
		normalreward59 = 59,
		normalreward60 = 60,
		normalreward61 = 61,
		normalreward62 = 62,
		normalreward63 = 63,
		normalreward64 = 64,
		normalreward65 = 65,
		normalreward66 = 66,
		normalreward67 = 67,
		normalreward68 = 68,
		normalreward69 = 69,
		normalreward70 = 70,
		royalreward01 = 1001,
		royalreward02 = 1002,
		royalreward03 = 1003,
		royalreward04 = 1004,
		royalreward05 = 1005,
		royalreward06 = 1006,
		royalreward07 = 1007,
		royalreward08 = 1008,
		royalreward09 = 1009,
		royalreward10 = 1010,
		royalreward11 = 1011,
		royalreward12 = 1012,
		royalreward13 = 1013,
		royalreward14 = 1014,
		royalreward15 = 1015,
		royalreward16 = 1016,
		royalreward17 = 1017,
		royalreward18 = 1018,
		royalreward19 = 1019,
		royalreward20 = 1020,
		royalreward21 = 1021,
		royalreward22 = 1022,
		royalreward23 = 1023,
		royalreward24 = 1024,
		royalreward25 = 1025,
		royalreward26 = 1026,
		royalreward27 = 1027,
		royalreward28 = 1028,
		royalreward29 = 1029,
		royalreward30 = 1030,
		royalreward31 = 1031,
		royalreward32 = 1032,
		royalreward33 = 1033,
		royalreward34 = 1034,
		royalreward35 = 1035,
		royalreward36 = 1036,
		royalreward37 = 1037,
		royalreward38 = 1038,
		royalreward39 = 1039,
		royalreward40 = 1040,
		royalreward41 = 1041,
		royalreward42 = 1042,
		royalreward43 = 1043,
		royalreward44 = 1044,
		royalreward45 = 1045,
		royalreward46 = 1046,
		royalreward47 = 1047,
		royalreward48 = 1048,
		royalreward49 = 1049,
		royalreward50 = 1050,
		royalreward51 = 1051,
		royalreward52 = 1052,
		royalreward53 = 1053,
		royalreward54 = 1054,
		royalreward55 = 1055,
		royalreward56 = 1056,
		royalreward57 = 1057,
		royalreward58 = 1058,
		royalreward59 = 1059,
		royalreward60 = 1060,
		royalreward61 = 1061,
		royalreward62 = 1062,
		royalreward63 = 1063,
		royalreward64 = 1064,
		royalreward65 = 1065,
		royalreward66 = 1066,
		royalreward67 = 1067,
		royalreward68 = 1068,
		royalreward69 = 1069,
		royalreward70 = 1070,
	}

	public class TDataClassReward : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int rankPoint { get; set; }
		public int rewardType { get; set; }
		public int ItemId { get; set; }
		public int ItemValue { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			rankPoint = int.Parse(cols[2]);
			rewardType = int.Parse(cols[3]);
			ItemId = int.Parse(cols[4]);
			ItemValue = int.Parse(cols[5]);
		}
	}
}
