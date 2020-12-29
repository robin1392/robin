using System;

namespace Table.Data
{
	public class TDataSeasonpassInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public bool seasonPassState { get; set; }
		public int seasonPassStartDate { get; set; }
		public int seasonPassEndDate { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1];
			seasonPassState = bool.Parse(cols[2]);
			seasonPassStartDate = int.Parse(cols[3]);
			seasonPassEndDate = int.Parse(cols[4]);
		}
	}
}
