using System;

namespace RandomWarsResource.Data
{
	public enum EDiceLevelInfoKey : int
	{
		None = -1,

		NORMAL = 0,
		MAGIC = 1,
		EPIC  = 2,
		LEGEND = 3,
	}

	public class TDataDiceLevelInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int baseLevel { get; set; }
		public string fontColor { get; set; }
		public string diceGradeImage { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			baseLevel = int.Parse(cols[2]);
			fontColor = cols[3].Replace("{#$}", ",");
			diceGradeImage = cols[4].Replace("{#$}", ",");
		}
	}
}
