using System;

namespace RandomWarsResource.Data
{
	public class TDataErrorMessageKO : ITableData<int>
	{
		public string stingKey { get; set; }
		public int id { get; set; }
		public string textDesc { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			stingKey = cols[0].Replace("{$}", ",");
			id = int.Parse(cols[1]);
			textDesc = cols[2].Replace("{$}", ",");
		}
	}
}
