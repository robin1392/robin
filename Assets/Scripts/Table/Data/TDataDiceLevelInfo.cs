using System;

namespace RandomWarsResource.Data
{
	public class TDataDiceLevelInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int baseLevel { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{$}", ",");
			baseLevel = int.Parse(cols[2]);
		}
	}
}
