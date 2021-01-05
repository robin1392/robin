using System;

namespace RandomWarsResource.Data
{
	public enum ERakingSeasonInfoKey : int
	{
		None = -1,

		beforeseason = 1,
		seasonend = 2,
		seasongoing = 3,
		seasonaccount = 4,
		preseason = 5,
	}

	public class TDataRakingSeasonInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int seasonState { get; set; }
		public bool rankingGameOn { get; set; }
		public bool rankingRewardOn { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			seasonState = int.Parse(cols[2]);
			rankingGameOn = bool.Parse(cols[3]);
			rankingRewardOn = bool.Parse(cols[4]);
		}
	}
}
