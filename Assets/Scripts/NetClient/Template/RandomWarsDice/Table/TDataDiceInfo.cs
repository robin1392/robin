using System;
using System.Collections.Generic;
using Service.Template.Table;
 

namespace Template.Item.RandomWarsDice.Table
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
			name = cols[1];
			grade = int.Parse(cols[2]);
			castType = int.Parse(cols[3]);
			moveType = int.Parse(cols[4]);
			targetMoveType = int.Parse(cols[5]);
			loadType = int.Parse(cols[6]);
			enableDice = bool.Parse(cols[7]);
			prefabName = cols[8];
			modelName = cols[9];
			spawnMultiply = int.Parse(cols[10]);
			iconName = cols[11];
			illustName = cols[12];
			cardName = cols[13];
			color = Array.ConvertAll(cols[14].Split(','), delegate (string s) { return int.Parse(s); });
			power = int.Parse(cols[15]);
			powerUpgrade = int.Parse(cols[16]);
			powerInGameUp = int.Parse(cols[17]);
			maxHealth = int.Parse(cols[18]);
			maxHpUpgrade = int.Parse(cols[19]);
			maxHpInGameUp = int.Parse(cols[20]);
			effect = int.Parse(cols[21]);
			effectUpgrade = int.Parse(cols[22]);
			effectInGameUp = int.Parse(cols[23]);
			effectDuration = int.Parse(cols[24]);
			effectCooltime = int.Parse(cols[25]);
			attackSpeed = int.Parse(cols[26]);
			moveSpeed = int.Parse(cols[27]);
			range = int.Parse(cols[28]);
			searchRange = int.Parse(cols[29]);
			skillindex = int.Parse(cols[30]);
		}
	}
}