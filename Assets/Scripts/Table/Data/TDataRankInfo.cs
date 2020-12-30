using System;

namespace RandomWarsResource.Data
{
	public enum ERankInfoKey : int
	{
		None = -1,

		rank1 = 1,
		rank2 = 2,
		rank3 = 3,
		rank4 = 4,
		rank5 = 5,
		rank6 = 6,
		rank7 = 7,
		rank8 = 8,
		rank9 = 9,
		rank10 = 10,
		rank11 = 11,
	}

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
			name = cols[1].Replace("{#$}", ",");
			rankingPointMinMax = Array.ConvertAll(cols[2].Split('|'), s => int.Parse(s));
		}
	}
}
