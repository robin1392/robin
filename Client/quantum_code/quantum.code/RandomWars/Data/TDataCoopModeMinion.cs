using System;

namespace RandomWarsResource.Data
{
	public class TDataCoopModeMinion : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int minionId01 { get; set; }
		public int classLv01 { get; set; }
		public int diceLv01 { get; set; }
		public int diceSpot01 { get; set; }
		public int minionId02 { get; set; }
		public int classLv02 { get; set; }
		public int diceLv02 { get; set; }
		public int diceSpot02 { get; set; }
		public int minionId03 { get; set; }
		public int classLv03 { get; set; }
		public int diceLv03 { get; set; }
		public int diceSpot03 { get; set; }
		public int minionId04 { get; set; }
		public int classLv04 { get; set; }
		public int diceLv04 { get; set; }
		public int diceSpot04 { get; set; }
		public int minionId05 { get; set; }
		public int classLv05 { get; set; }
		public int diceLv05 { get; set; }
		public int diceSpot05 { get; set; }
		public int minionId06 { get; set; }
		public int classLv06 { get; set; }
		public int diceLv06 { get; set; }
		public int diceSpot06 { get; set; }
		public int minionId07 { get; set; }
		public int classLv07 { get; set; }
		public int diceLv07 { get; set; }
		public int diceSpot07 { get; set; }
		public int minionId08 { get; set; }
		public int classLv08 { get; set; }
		public int diceLv08 { get; set; }
		public int diceSpot08 { get; set; }
		public int minionId09 { get; set; }
		public int classLv09 { get; set; }
		public int diceLv09 { get; set; }
		public int diceSpot09 { get; set; }
		public int minionId10 { get; set; }
		public int classLv10 { get; set; }
		public int diceLv10 { get; set; }
		public int diceSpot10 { get; set; }
		public int minionId11 { get; set; }
		public int classLv11 { get; set; }
		public int diceLv11 { get; set; }
		public int diceSpot11 { get; set; }
		public int minionId12 { get; set; }
		public int classLv12 { get; set; }
		public int diceLv12 { get; set; }
		public int diceSpot12 { get; set; }
		public int minionId13 { get; set; }
		public int classLv13 { get; set; }
		public int diceLv13 { get; set; }
		public int diceSpot13 { get; set; }
		public int minionId14 { get; set; }
		public int classLv14 { get; set; }
		public int diceLv14 { get; set; }
		public int diceSpot14 { get; set; }
		public int minionId15 { get; set; }
		public int classLv15 { get; set; }
		public int diceLv15 { get; set; }
		public int diceSpot15 { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			minionId01 = int.Parse(cols[2]);
			classLv01 = int.Parse(cols[3]);
			diceLv01 = int.Parse(cols[4]);
			diceSpot01 = int.Parse(cols[5]);
			minionId02 = int.Parse(cols[6]);
			classLv02 = int.Parse(cols[7]);
			diceLv02 = int.Parse(cols[8]);
			diceSpot02 = int.Parse(cols[9]);
			minionId03 = int.Parse(cols[10]);
			classLv03 = int.Parse(cols[11]);
			diceLv03 = int.Parse(cols[12]);
			diceSpot03 = int.Parse(cols[13]);
			minionId04 = int.Parse(cols[14]);
			classLv04 = int.Parse(cols[15]);
			diceLv04 = int.Parse(cols[16]);
			diceSpot04 = int.Parse(cols[17]);
			minionId05 = int.Parse(cols[18]);
			classLv05 = int.Parse(cols[19]);
			diceLv05 = int.Parse(cols[20]);
			diceSpot05 = int.Parse(cols[21]);
			minionId06 = int.Parse(cols[22]);
			classLv06 = int.Parse(cols[23]);
			diceLv06 = int.Parse(cols[24]);
			diceSpot06 = int.Parse(cols[25]);
			minionId07 = int.Parse(cols[26]);
			classLv07 = int.Parse(cols[27]);
			diceLv07 = int.Parse(cols[28]);
			diceSpot07 = int.Parse(cols[29]);
			minionId08 = int.Parse(cols[30]);
			classLv08 = int.Parse(cols[31]);
			diceLv08 = int.Parse(cols[32]);
			diceSpot08 = int.Parse(cols[33]);
			minionId09 = int.Parse(cols[34]);
			classLv09 = int.Parse(cols[35]);
			diceLv09 = int.Parse(cols[36]);
			diceSpot09 = int.Parse(cols[37]);
			minionId10 = int.Parse(cols[38]);
			classLv10 = int.Parse(cols[39]);
			diceLv10 = int.Parse(cols[40]);
			diceSpot10 = int.Parse(cols[41]);
			minionId11 = int.Parse(cols[42]);
			classLv11 = int.Parse(cols[43]);
			diceLv11 = int.Parse(cols[44]);
			diceSpot11 = int.Parse(cols[45]);
			minionId12 = int.Parse(cols[46]);
			classLv12 = int.Parse(cols[47]);
			diceLv12 = int.Parse(cols[48]);
			diceSpot12 = int.Parse(cols[49]);
			minionId13 = int.Parse(cols[50]);
			classLv13 = int.Parse(cols[51]);
			diceLv13 = int.Parse(cols[52]);
			diceSpot13 = int.Parse(cols[53]);
			minionId14 = int.Parse(cols[54]);
			classLv14 = int.Parse(cols[55]);
			diceLv14 = int.Parse(cols[56]);
			diceSpot14 = int.Parse(cols[57]);
			minionId15 = int.Parse(cols[58]);
			classLv15 = int.Parse(cols[59]);
			diceLv15 = int.Parse(cols[60]);
			diceSpot15 = int.Parse(cols[61]);
		}
	}
}
