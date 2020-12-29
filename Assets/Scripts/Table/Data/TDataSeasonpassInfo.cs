using System;

namespace RandomWarsResource.Data
{
	public class TDataSeasonpassInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public bool seasonPassState { get; set; }
		public DateTime seasonPassStartDate { get; set; }
		public DateTime seasonPassEndDate { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{$}", ",");
			seasonPassState = bool.Parse(cols[2]);
			seasonPassStartDate = DateTime.Parse(cols[3]);
			seasonPassEndDate = DateTime.Parse(cols[4]);
		}
	}
}
