using System;

namespace RandomWarsResource.Data
{
	public enum ESeasonpassInfoKey : int
	{
		None = -1,

	}

	public class TDataSeasonpassInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int nextSeasonId { get; set; }
		public bool ispreSeason { get; set; }
		public DateTime seasonPassStartDate { get; set; }
		public DateTime seasonPassEndDate { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			nextSeasonId = int.Parse(cols[2]);
			ispreSeason = bool.Parse(cols[3]);
			seasonPassStartDate = DateTime.Parse(cols[4]);
			seasonPassEndDate = DateTime.Parse(cols[5]);
		}
	}
}
