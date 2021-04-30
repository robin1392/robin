using System;

namespace RandomWarsResource.Data
{
	public class TDataClassInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int[] trophyPointMinMax { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			trophyPointMinMax = (cols[2] == "-1") ? null : Array.ConvertAll(cols[2].Split('|'), s => int.Parse(s));
		}
	}
}
