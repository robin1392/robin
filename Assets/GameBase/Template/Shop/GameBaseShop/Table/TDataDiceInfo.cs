using System;
using Service.Template.Table;

namespace Template.Shop.GameBaseShop.Table
{
	public enum EDiceInfoKey : int
	{
		None = -1,

		Dice_Ranger = 1000,
		Dice_Zergling = 1001,
		Dice_Knight = 1002,
		Dice_Healer = 1003,
		Dice_Fireball = 1004,
		Dice_Mine = 1005,
		Dice_Plane = 1006,
		Dice_Ninja = 2000,
		Dice_Sinzed = 2001,
		Dice_Shielder = 2002,
		Dice_Fireman = 2003,
		Dice_Posu = 2004,
		Dice_Race = 2005,
		Dice_StoneBall = 2006,
		Dice_IceBall = 2007,
		Dice_Turret = 2008,
		Dice_FlagOfWar = 2009,
		Dice_Mortar = 3000,
		Dice_Necromancer = 3001,
		Dice_Raider = 3002,
		Dice_Rush = 3003,
		Dice_Support = 3004,
		Dice_Berserker = 3005,
		Dice_Goliath = 3006,
		Dice_FighterPlane = 3007,
		Dice_Zombie = 3008,
		Dice_Layzer = 3009,
		Dice_Rocket = 3010,
		Dice_Sniper = 3011,
		Dice_BabyDragon = 4000,
		Dice_Invincibility = 4001,
		Dice_RazyOfTower = 4002,
		Dice_Golem = 4003,
		Dice_MiniGolem = 4004,
		Dice_Robot = 4005,
		Dice_Magician = 4006,
		Dice_Saint = 4007,
		Dice_Bomber = 4008,
		Dice_Arbiter = 4009,
		Dice_LightningRod = 4010,
		Dice_Ranger_Coop = 30001,
		Dice_Zergling_Coop = 30002,
		Dice_Knight_Coop = 30003,
		Dice_Healer_Coop = 30004,
		Dice_Fireball_Coop = 30005,
		Dice_Mine_Coop = 30006,
		Dice_Plane_Coop = 30007,
		Dice_Ninja_Coop = 32000,
		Dice_Sinzed_Coop = 32001,
		Dice_Shielder_Coop = 32002,
		Dice_Fireman_Coop = 32003,
		Dice_Posu_Coop = 32004,
		Dice_Race_Coop = 32005,
		Dice_ShtoneBall_Coop = 32006,
		Dice_IceBall_Coop = 32007,
		Dice_Turret_Coop = 32008,
		Dice_FlagOfWar_Coop = 32009,
		Dice_Mortar_Coop = 33000,
		Dice_Necromancer_Coop = 33001,
		Dice_Raider_Coop = 33002,
		Dice_Rush_Coop = 33003,
		Dice_Support_Coop = 33004,
		Dice_Berserker_Coop = 33005,
		Dice_Goliath_Coop = 33006,
		Dice_FighterPlane_Coop = 33007,
		Dice_Zombie_Coop = 33008,
		Dice_Layzer_Coop = 33009,
		Dice_Rocket_Coop = 33010,
		Dice_Sniper_Coop = 33011,
		Dice_BabyDragon_Coop = 34000,
		Dice_Invincibility_Coop = 34001,
		Dice_RazyOfTower_Coop = 34002,
		Dice_Golem_Coop = 34003,
		Dice_MiniGolem_Coop = 34004,
		Dice_Robot_Coop = 34005,
		Dice_Magician_Coop = 34006,
		Dice_Saint_Coop = 34007,
		Dice_Bomber_Coop = 34008,
		Dice_Arbiter_Coop = 34009,
		Dice_LightningRod_Coop = 34010,
	}

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
		public int skillIndex { get; set; }
		public bool attackType { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			grade = int.Parse(cols[2]);
			castType = int.Parse(cols[3]);
			moveType = int.Parse(cols[4]);
			targetMoveType = int.Parse(cols[5]);
			loadType = int.Parse(cols[6]);
			enableDice = bool.Parse(cols[7]);
			prefabName = cols[8].Replace("{#$}", ",");
			modelName = cols[9].Replace("{#$}", ",");
			spawnMultiply = int.Parse(cols[10]);
			iconName = cols[11].Replace("{#$}", ",");
			illustName = cols[12].Replace("{#$}", ",");
			cardName = cols[13].Replace("{#$}", ",");
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
			skillIndex = int.Parse(cols[30]);
			attackType = bool.Parse(cols[31]);
		}
	}
}
