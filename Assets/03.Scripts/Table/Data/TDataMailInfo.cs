using System;

namespace RandomWarsResource.Data
{
	public class TDataMailInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string mailSender_kr { get; set; }
		public string mailTitle_kr { get; set; }
		public string mailText_kr { get; set; }
		public string mailSender_en { get; set; }
		public string mailTitle_en { get; set; }
		public string mailText_en { get; set; }
		public string mailSender_jp { get; set; }
		public string mailTitle_jp { get; set; }
		public string mailText_jp { get; set; }
		public string mailSender_cn { get; set; }
		public string mailTitle_cn { get; set; }
		public string mailText_cn { get; set; }
		public string mailSender_tw { get; set; }
		public string mailTitle_tw { get; set; }
		public string mailText_tw { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			mailSender_kr = cols[2].Replace("{#$}", ",");
			mailTitle_kr = cols[3].Replace("{#$}", ",");
			mailText_kr = cols[4].Replace("{#$}", ",");
			mailSender_en = cols[5].Replace("{#$}", ",");
			mailTitle_en = cols[6].Replace("{#$}", ",");
			mailText_en = cols[7].Replace("{#$}", ",");
			mailSender_jp = cols[8].Replace("{#$}", ",");
			mailTitle_jp = cols[9].Replace("{#$}", ",");
			mailText_jp = cols[10].Replace("{#$}", ",");
			mailSender_cn = cols[11].Replace("{#$}", ",");
			mailTitle_cn = cols[12].Replace("{#$}", ",");
			mailText_cn = cols[13].Replace("{#$}", ",");
			mailSender_tw = cols[14].Replace("{#$}", ",");
			mailTitle_tw = cols[15].Replace("{#$}", ",");
			mailText_tw = cols[16].Replace("{#$}", ",");
		}
	}
}
