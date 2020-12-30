using System;

namespace RandomWarsResource.Data
{
	public enum EBoxProductInfoKey : int
	{
		None = -1,

		normalboxgoods_01 = 11001,
		normalboxgoods_02 = 11002,
		normalboxgoods_03 = 11003,
		goldboxgoods_01 = 12001,
		goldboxgoods_02 = 12002,
		goldboxgoods_03 = 12003,
		diaboxgoods_01 = 13001,
		diaboxgoods_02 = 13002,
		diaboxgoods_03 = 13003,
		supportboxgoods_01 = 20001,
		supportboxgoods_02 = 20002,
		supportboxgoods_03 = 20003,
		boss01boxgoods_01 = 30001,
		boss01boxgoods_02 = 30002,
		boss01boxgoods_03 = 30003,
		boss02boxgoods_01 = 31001,
		boss02boxgoods_02 = 31002,
		boss02boxgoods_03 = 31003,
		boss03boxgoods_01 = 32001,
		boss03boxgoods_02 = 32002,
		boss03boxgoods_03 = 32003,
		boss04boxgoods_01 = 33001,
		boss04boxgoods_02 = 33002,
		boss04boxgoods_03 = 33003,
		boss05boxgoods_01 = 34001,
		boss05boxgoods_02 = 34002,
		boss05boxgoods_03 = 34003,
	}

	public class TDataBoxProductInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int BoxListId { get; set; }
		public int[] userRankGradeRange { get; set; }
		public int itemId01 { get; set; }
		public int[] itemValue01 { get; set; }
		public int itemId02 { get; set; }
		public int[] itemValue02 { get; set; }
		public int rewardCardGradeType1 { get; set; }
		public int rewardCardGradeCnt1 { get; set; }
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
			userRankGradeRange = Array.ConvertAll(cols[3].Split('|'), s => int.Parse(s));
			itemId01 = int.Parse(cols[4]);
			itemValue01 = Array.ConvertAll(cols[5].Split('|'), s => int.Parse(s));
			itemId02 = int.Parse(cols[6]);
			itemValue02 = Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
			rewardCardGradeType1 = int.Parse(cols[8]);
			rewardCardGradeCnt1 = int.Parse(cols[9]);
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
