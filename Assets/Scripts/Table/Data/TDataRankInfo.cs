using System;

namespace RandomWarsResource.Data
{
	public class TDataRankInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int[] rankingPointMinMax { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{$}", ",");
			rankingPointMinMax = Array.ConvertAll(cols[2].Split('|'), s => int.Parse(s));
		}
	}
}
