using System;

namespace RandomWarsResource.Data
{
	public enum EBoxProductInfoKey : int
	{
		None = -1,

		supportboxgoods01 = 1101,
		supportboxgoods02 = 1102,
		supportboxgoods03 = 1103,
		normalboxgoods01 = 1201,
		normalboxgoods02 = 1202,
		normalboxgoods03 = 1203,
		bronzeboxgoods01 = 1301,
		bronzeboxgoods02 = 1302,
		bronzeboxgoods03 = 1303,
		silverboxgoods01 = 1401,
		silverboxgoods02 = 1402,
		silverboxgoods03 = 1403,
		goldboxgoods01 = 1501,
		goldboxgoods02 = 1502,
		goldboxgoods03 = 1503,
		platinumboxgoods01 = 1601,
		platinumboxgoods02 = 1602,
		platinumboxgoods03 = 1603,
		diaboxgoods01 = 1701,
		diaboxgoods02 = 1702,
		diaboxgoods03 = 1703,
		boss01boxgoods01 = 1801,
		boss01boxgoods02 = 1802,
		boss01boxgoods03 = 1803,
		boss02boxgoods01 = 1901,
		boss02boxgoods02 = 1902,
		boss02boxgoods03 = 1903,
		boss03boxgoods01 = 2001,
		boss03boxgoods02 = 2002,
		boss03boxgoods03 = 2003,
		boss04boxgoods01 = 2101,
		boss04boxgoods02 = 2102,
		boss04boxgoods03 = 2103,
		boss05boxgoods01 = 2201,
		boss05boxgoods02 = 2202,
		boss05boxgoods03 = 2203,
		boss01box_rewardgoods01 = 2301,
		boss01box_rewardgoods02 = 2302,
		boss01box_rewardgoods03 = 2303,
		boss02box_rewardgoods01 = 2401,
		boss02box_rewardgoods02 = 2402,
		boss02box_rewardgoods03 = 2403,
		boss03box_rewardgoods01 = 2501,
		boss03box_rewardgoods02 = 2502,
		boss03box_rewardgoods03 = 2503,
		boss04box_rewardgoods01 = 2601,
		boss04box_rewardgoods02 = 2602,
		boss04box_rewardgoods03 = 2603,
		boss05box_rewardgoods01 = 2701,
		boss05box_rewardgoods02 = 2702,
		boss05box_rewardgoods03 = 2703,
	}

	public class TDataBoxProductInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int BoxListId { get; set; }
		public int[] userClassGradeRange { get; set; }
		public int itemId01 { get; set; }
		public int[] itemValue01 { get; set; }
		public int itemId02 { get; set; }
		public int[] itemValue02 { get; set; }
		public int rewardCardGradeType1 { get; set; }
		public int rewardCardGradeCnt1  { get; set; }
		public int[] rewardCardValue1 { get; set; }
		public bool rewardIsProbability1 { get; set; }
		public int rewardProbability1 { get; set; }
		public int rewardCardGradeType2 { get; set; }
		public int rewardCardGradeCnt2 { get; set; }
		public int[] rewardCardValue2 { get; set; }
		public bool rewardIsProbability2 { get; set; }
		public int rewardProbability2 { get; set; }
		public int rewardCardGradeType3 { get; set; }
		public int rewardCardGradeCnt3 { get; set; }
		public int[] rewardCardValue3 { get; set; }
		public bool rewardIsProbability3 { get; set; }
		public int rewardProbability3 { get; set; }
		public int rewardCardGradeType4 { get; set; }
		public int rewardCardGradeCnt4 { get; set; }
		public int[] rewardCardValue4 { get; set; }
		public bool rewardIsProbability4 { get; set; }
		public int rewardProbability4 { get; set; }
		public int rewardCardGradeType5 { get; set; }
		public int rewardCardGradeCnt5 { get; set; }
		public int[] rewardCardValue5 { get; set; }
		public bool rewardIsProbability5 { get; set; }
		public int rewardProbability5 { get; set; }
		public bool isUse { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			BoxListId = int.Parse(cols[2]);
			userClassGradeRange = Array.ConvertAll(cols[3].Split('|'), s => int.Parse(s));
			itemId01 = int.Parse(cols[4]);
			itemValue01 = Array.ConvertAll(cols[5].Split('|'), s => int.Parse(s));
			itemId02 = int.Parse(cols[6]);
			itemValue02 = Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
			rewardCardGradeType1 = int.Parse(cols[8]);
			rewardCardGradeCnt1  = int.Parse(cols[9]);
			rewardCardValue1 = Array.ConvertAll(cols[10].Split('|'), s => int.Parse(s));
			rewardIsProbability1 = bool.Parse(cols[11]);
			rewardProbability1 = int.Parse(cols[12]);
			rewardCardGradeType2 = int.Parse(cols[13]);
			rewardCardGradeCnt2 = int.Parse(cols[14]);
			rewardCardValue2 = Array.ConvertAll(cols[15].Split('|'), s => int.Parse(s));
			rewardIsProbability2 = bool.Parse(cols[16]);
			rewardProbability2 = int.Parse(cols[17]);
			rewardCardGradeType3 = int.Parse(cols[18]);
			rewardCardGradeCnt3 = int.Parse(cols[19]);
			rewardCardValue3 = Array.ConvertAll(cols[20].Split('|'), s => int.Parse(s));
			rewardIsProbability3 = bool.Parse(cols[21]);
			rewardProbability3 = int.Parse(cols[22]);
			rewardCardGradeType4 = int.Parse(cols[23]);
			rewardCardGradeCnt4 = int.Parse(cols[24]);
			rewardCardValue4 = Array.ConvertAll(cols[25].Split('|'), s => int.Parse(s));
			rewardIsProbability4 = bool.Parse(cols[26]);
			rewardProbability4 = int.Parse(cols[27]);
			rewardCardGradeType5 = int.Parse(cols[28]);
			rewardCardGradeCnt5 = int.Parse(cols[29]);
			rewardCardValue5 = Array.ConvertAll(cols[30].Split('|'), s => int.Parse(s));
			rewardIsProbability5 = bool.Parse(cols[31]);
			rewardProbability5 = int.Parse(cols[32]);
			isUse = bool.Parse(cols[33]);
		}
	}
}
