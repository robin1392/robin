using System;

namespace RandomWarsResource.Data
{
	public enum EErrorMessageKOKey : int
	{
		None = -1,

		ERROR_GAMELIFT_ACCEPT_PLAYER_SESSION = 10001,
		ERROR_GAMELIFT_REMOVE_PLAYER_SESSION = 10002,
		ERROR_GAMELIFT_MATCH_PLACING = 10003,
		ERROR_DATABASE_UNEXPECTED = 10011,
		ERROR_USER_NOT_FOUND = 10001,
		ERROR_USER_NAME_DUPLICATED = 10001,
		ERROR_BOX_NOT_FOUND = 10101,
		ERROR_BOX_COUNT_LACK = 10102,
		ERROR_DICE_LEVELUP_DATA_NOT_FOUND = 10201,
		ERROR_DICE_LEVELUP_LACK_GOLD = 10202,
		ERROR_DICE_LEVELUP_LACK_DICE = 10203,
		ERROR_GAME_ROOM_NOT_FOUND = 20101,
		ERROR_GAME_ROOM_PLAYER_JOIN = 20102,
		ERROR_GAME_ROOM_PLAYER_LEAVE = 20103,
		ERROR_GAME_PLAYER_NOT_FOUND = 20104,
		ERROR_GAME_PLAYER_INVALID_STATE = 20105,
		ERROR_GET_DICE_FAILED = 20106,
		ERROR_LEVELUP_DICE_FAILED = 20107,
		ERROR_INGAME_UP_DICE_FAILED = 20108,
	}

	public class TDataErrorMessageKO : ITableData<int>
	{
		public int id { get; set; }
		public string name { get; set; }
		public string textDesc { get; set; }


		public int PK()
		{
			return id;
		}


		public void Serialize(string[] cols)
		{
			id = int.Parse(cols[0]);
			name = cols[1].Replace("{#$}", ",");
			textDesc = cols[2].Replace("{#$}", ",");
		}
	}
}
