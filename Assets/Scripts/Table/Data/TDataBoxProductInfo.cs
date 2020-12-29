using System;

namespace RandomWarsResource.Data
{
	public class TDataBoxProductInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int BoxListId { get; set; }
		public int[] userRankGradeRange { get; set; }
		public int[] goldRange { get; set; }
		public int[] diaRange { get; set; }
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
			name = cols[1].Replace("{$}", ",");
			BoxListId = int.Parse(cols[2]);
			userRankGradeRange = Array.ConvertAll(cols[3].Split('|'), s => int.Parse(s));
			goldRange = Array.ConvertAll(cols[4].Split('|'), s => int.Parse(s));
			diaRange = Array.ConvertAll(cols[5].Split('|'), s => int.Parse(s));
			rewardCardGradeType1 = int.Parse(cols[6]);
			rewardCardGradeCnt1 = int.Parse(cols[7]);
			rewardCardValue1 = Array.ConvertAll(cols[8].Split('|'), s => int.Parse(s));
			rewardIsProbability1 = bool.Parse(cols[9]);
			rewardProbability1 = int.Parse(cols[10]);
			rewardCardGradeType2 = int.Parse(cols[11]);
			rewardCardGradeCnt2 = int.Parse(cols[12]);
			rewardCardValue2 = Array.ConvertAll(cols[13].Split('|'), s => int.Parse(s));
			rewardIsProbability2 = bool.Parse(cols[14]);
			rewardProbability2 = int.Parse(cols[15]);
			rewardCardGradeType3 = int.Parse(cols[16]);
			rewardCardGradeCnt3 = int.Parse(cols[17]);
			rewardCardValue3 = Array.ConvertAll(cols[18].Split('|'), s => int.Parse(s));
			rewardIsProbability3 = bool.Parse(cols[19]);
			rewardProbability3 = int.Parse(cols[20]);
			rewardCardGradeType4 = int.Parse(cols[21]);
			rewardCardGradeCnt4 = int.Parse(cols[22]);
			rewardCardValue4 = Array.ConvertAll(cols[23].Split('|'), s => int.Parse(s));
			rewardIsProbability4 = bool.Parse(cols[24]);
			rewardProbability4 = int.Parse(cols[25]);
			rewardCardGradeType5 = int.Parse(cols[26]);
			rewardCardGradeCnt5 = int.Parse(cols[27]);
			rewardCardValue5 = Array.ConvertAll(cols[28].Split('|'), s => int.Parse(s));
			rewardIsProbability5 = bool.Parse(cols[29]);
			rewardProbability5 = int.Parse(cols[30]);
			isUse = bool.Parse(cols[31]);
		}
	}
}
