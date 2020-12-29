using System;

namespace RandomWarsResource.Data
{
	public class TDataCoopModeBossInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string bossName_kr { get; set; }
		public string bossName_en { get; set; }
		public string bossEmblem { get; set; }
		public int grade { get; set; }
		public int castType { get; set; }
		public int moveType { get; set; }
		public int targetMoveType { get; set; }
		public int loadType { get; set; }
		public bool enableDice { get; set; }
		public string prefabName { get; set; }
		public string modelName { get; set; }
		public int spawnMultiply { get; set; }
		public string iconName { get; set; }
		public string illustName { get; set; }
		public string cardName { get; set; }
		public int[] color { get; set; }
		public float power { get; set; }
		public float powerUpgrade { get; set; }
		public float powerInGameUp { get; set; }
		public float maxHealth { get; set; }
		public float maxHpUpgrade { get; set; }
		public float maxHpInGameUp { get; set; }
		public float effect { get; set; }
		public float effectUpgrade { get; set; }
		public float effectInGameUp { get; set; }
		public float effectDuration { get; set; }
		public float effectCooltime { get; set; }
		public float attackSpeed { get; set; }
		public float moveSpeed { get; set; }
		public float range { get; set; }
		public float searchRange { get; set; }
		public int skillindex { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{$}", ",");
			bossName_kr = cols[2].Replace("{$}", ",");
			bossName_en = cols[3].Replace("{$}", ",");
			bossEmblem = cols[4].Replace("{$}", ",");
			grade = int.Parse(cols[5]);
			castType = int.Parse(cols[6]);
			moveType = int.Parse(cols[7]);
			targetMoveType = int.Parse(cols[8]);
			loadType = int.Parse(cols[9]);
			enableDice = bool.Parse(cols[10]);
			prefabName = cols[11].Replace("{$}", ",");
			modelName = cols[12].Replace("{$}", ",");
			spawnMultiply = int.Parse(cols[13]);
			iconName = cols[14].Replace("{$}", ",");
			illustName = cols[15].Replace("{$}", ",");
			cardName = cols[16].Replace("{$}", ",");
			color = Array.ConvertAll(cols[17].Split('|'), s => int.Parse(s));
			power = float.Parse(cols[18]);
			powerUpgrade = float.Parse(cols[19]);
			powerInGameUp = float.Parse(cols[20]);
			maxHealth = float.Parse(cols[21]);
			maxHpUpgrade = float.Parse(cols[22]);
			maxHpInGameUp = float.Parse(cols[23]);
			effect = float.Parse(cols[24]);
			effectUpgrade = float.Parse(cols[25]);
			effectInGameUp = float.Parse(cols[26]);
			effectDuration = float.Parse(cols[27]);
			effectCooltime = float.Parse(cols[28]);
			attackSpeed = float.Parse(cols[29]);
			moveSpeed = float.Parse(cols[30]);
			range = float.Parse(cols[31]);
			searchRange = float.Parse(cols[32]);
			skillindex = int.Parse(cols[33]);
		}
	}
}
