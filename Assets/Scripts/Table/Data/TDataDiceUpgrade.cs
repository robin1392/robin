using System;

namespace RandomWarsResource.Data
{
	public enum EDiceUpgradeKey : int
	{
		None = -1,

		normalupgrade01 = 1,
		normalupgrade02 = 2,
		normalupgrade03 = 3,
		normalupgrade04 = 4,
		normalupgrade05 = 5,
		normalupgrade06 = 6,
		normalupgrade07 = 7,
		normalupgrade08 = 8,
		normalupgrade09 = 9,
		normalupgrade10 = 10,
		normalupgrade11 = 11,
		normalupgrade12 = 12,
		normalupgrade13 = 13,
		normalupgrade14 = 14,
		normalupgrade15 = 15,
		magicupgrade01 = 16,
		magicupgrade02 = 17,
		magicupgrade03 = 18,
		magicupgrade04 = 19,
		magicupgrade05 = 20,
		magicupgrade06 = 21,
		magicupgrade07 = 22,
		magicupgrade08 = 23,
		magicupgrade09 = 24,
		magicupgrade10 = 25,
		magicupgrade11 = 26,
		magicupgrade12 = 27,
		magicupgrade13 = 28,
		magicupgrade14 = 29,
		magicupgrade15 = 30,
		epicupgrade01 = 31,
		epicupgrade02 = 32,
		epicupgrade03 = 33,
		epicupgrade04 = 34,
		epicupgrade05 = 35,
		epicupgrade06 = 36,
		epicupgrade07 = 37,
		epicupgrade08 = 38,
		epicupgrade09 = 39,
		epicupgrade10 = 40,
		epicupgrade11 = 41,
		epicupgrade12 = 42,
		epicupgrade13 = 43,
		epicupgrade14 = 44,
		epicupgrade15 = 45,
		legendupgrade01 = 46,
		legendupgrade02 = 47,
		legendupgrade03 = 48,
		legendupgrade04 = 49,
		legendupgrade05 = 50,
		legendupgrade06 = 51,
		legendupgrade07 = 52,
		legendupgrade08 = 53,
		legendupgrade09 = 54,
		legendupgrade10 = 55,
		legendupgrade11 = 56,
		legendupgrade12 = 57,
		legendupgrade13 = 58,
		legendupgrade14 = 59,
		legendupgrade15 = 60,
	}

	public class TDataDiceUpgrade : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public int diceGrade { get; set; }
		public int diceLv { get; set; }
		public int needCard { get; set; }
		public int needGold { get; set; }
		public int getTowerHp { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			diceGrade = int.Parse(cols[2]);
			diceLv = int.Parse(cols[3]);
			needCard = int.Parse(cols[4]);
			needGold = int.Parse(cols[5]);
			getTowerHp = int.Parse(cols[6]);
		}
	}
}
