using System;

namespace RandomWarsResource.Data
{
	public class TDataDiceInfo : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
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
			grade = int.Parse(cols[2]);
			castType = int.Parse(cols[3]);
			moveType = int.Parse(cols[4]);
			targetMoveType = int.Parse(cols[5]);
			loadType = int.Parse(cols[6]);
			enableDice = bool.Parse(cols[7]);
			prefabName = cols[8].Replace("{$}", ",");
			modelName = cols[9].Replace("{$}", ",");
			spawnMultiply = int.Parse(cols[10]);
			iconName = cols[11].Replace("{$}", ",");
			illustName = cols[12].Replace("{$}", ",");
			cardName = cols[13].Replace("{$}", ",");
			color = Array.ConvertAll(cols[14].Split('|'), s => int.Parse(s));
			power = float.Parse(cols[15]);
			powerUpgrade = float.Parse(cols[16]);
			powerInGameUp = float.Parse(cols[17]);
			maxHealth = float.Parse(cols[18]);
			maxHpUpgrade = float.Parse(cols[19]);
			maxHpInGameUp = float.Parse(cols[20]);
			effect = float.Parse(cols[21]);
			effectUpgrade = float.Parse(cols[22]);
			effectInGameUp = float.Parse(cols[23]);
			effectDuration = float.Parse(cols[24]);
			effectCooltime = float.Parse(cols[25]);
			attackSpeed = float.Parse(cols[26]);
			moveSpeed = float.Parse(cols[27]);
			range = float.Parse(cols[28]);
			searchRange = float.Parse(cols[29]);
			skillindex = int.Parse(cols[30]);
		}
	}
}
