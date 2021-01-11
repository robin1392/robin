using System;

namespace RandomWarsResource.Data
{
	public enum ERankingSeasonInfoKey : int
	{
		None = -1,

		seasonperiod01 = 1,
		seasonperiod02 = 2,
		seasonperiod03 = 3,
	}

	public class TDataRankingSeasonInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public bool ispreSeason { get; set; }
		public int nextSeasonId { get; set; }
		public DateTime rankingSeasonStartDate { get; set; }
		public DateTime rankingSeasonEndDate { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			ispreSeason = bool.Parse(cols[2]);
			nextSeasonId = int.Parse(cols[3]);
			rankingSeasonStartDate = DateTime.Parse(cols[4]);
			rankingSeasonEndDate = DateTime.Parse(cols[5]);
		}
	}
}
