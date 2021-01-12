using System;

namespace RandomWarsResource.Data
{
	public enum ELangKOKey : int
	{
		None = -1,

		Gamemoney_Gold = 1,
		Gamemoney_Diamond = 2,
		Minioninfo_Type = 3,
		Minioninfo_Hp = 4,
		Minioninfo_Atk = 5,
		Minioninfo_Atkspd = 6,
		Minioninfo_Movespd = 7,
		Minioninfo_Atkrange = 8,
		Minioninfo_None01 = 9,
		Minioninfo_None02 = 10,
		Gameui_Unit = 11,
		Gameui_Magic = 12,
		Gameui_Install = 13,
		Gameui_Hero = 14,
		Miniongrade_Normal = 15,
		Miniongrade_Magic = 16,
		Miniongrade_Epic = 17,
		Miniongrade_Legend = 18,
		Boxitem_Normalbox = 19,
		Boxitem_Coopbox = 20,
		Submenu_Notice = 21,
		Submenu_Mailbox = 22,
		Submenu_Ranking = 23,
		Tutorial_Guide01 = 201,
		Tutorial_Guide02 = 202,
		Tutorial_Guide03 = 203,
		Tutorial_Guide04 = 204,
		Tutorial_Guide05 = 205,
		Tutorial_Guide06 = 206,
		Tutorial_Guide07 = 207,
		Tutorial_Guide08 = 208,
		Tutorial_Guide09 = 209,
		Tutorial_Guide10 = 210,
		Tutorial_Guide11 = 211,
		Tutorial_Guide12 = 212,
		Ranking_Title = 301,
		Ranking_State_Open = 302,
		Ranking_Myranking = 303,
		Ranking_Myreward = 304,
		Ranking_Rewardinfo = 305,
		Ranking_Rewardguide = 306,
		Ranking_State_Account = 307,
		Ranking_State_Closed = 308,
		Ranking_Myranking_None = 309,
		Ranking_Rewardinfo_None = 310,
		Ranking_Rewardlist_None = 311,
		Option_Title = 401,
		Option_Pid = 402,
		Option_Pidcopy = 403,
		Option_Music = 404,
		Option_Sound = 405,
		Option_On = 406,
		Option_Off = 407,
		Option_Login = 408,
		Option_Logout = 409,
		Option_GuestLogin = 410,
		Option_Changenickname = 411,
		Option_Support = 412,
		Option_Youtube = 413,
		Option_More111percent = 414,
		Option_Credit = 415,
		Option_Credittext = 416,
		Seasonpass_Title = 501,
		Seasonpass_Normal = 502,
		Seasonpass_Special = 503,
		Seasonpass_Period = 504,
		Seasonpass_Buypass = 505,
		Seasonpass_Info = 506,
		Seasonpass_Needseasonpass = 507,
		Seasonpass_Needtrophy = 508,
		Quest_Title = 601,
		Quest_Dayreward = 602,
		Quest_Text1 = 603,
		Quest_Text2 = 604,
		Quest_Text3 = 605,
		Quest_Text4 = 606,
		Quest_Text5 = 607,
		Quest_Text6 = 608,
		Quest_Text7 = 609,
		Quest_Text8 = 610,
		Quest_Text9 = 611,
		Quest_Text10 = 612,
		Quest_Text11 = 613,
		Quest_Text12 = 614,
		Quest_Text13 = 615,
		Quest_Text14 = 616,
		Quest_Text15 = 617,
		Quest_Text16 = 618,
		Quest_Text17 = 619,
		Quest_Text18 = 620,
		Quest_Text101 = 621,
		Quest_Text102 = 622,
		Quest_Text103 = 623,
		Quest_Text104 = 624,
		Quest_Text105 = 625,
		Quest_Text106 = 626,
		Quest_Text107 = 627,
		Quest_Text108 = 628,
		Quest_Text109 = 629,
		Quest_Text110 = 630,
		Quest_Text111 = 631,
		Quest_Text112 = 632,
		Quest_Text113 = 633,
		Quest_Text114 = 634,
		Quest_Text115 = 635,
		Quest_Text116 = 636,
		Quest_Text117 = 637,
		Quest_Text118 = 638,
		Quest_Text1000 = 639,
		Quest_Rewardbutton = 642,
		Quest_Allcomplete = 643,
		Gameinvite_Titlepvp = 701,
		Gameinvite_Modeguidepvp = 702,
		Gameinvite_Btnwithfriendpvp = 703,
		Gameinvite_Btnsolopvp = 704,
		Gameinvite_Titlecoop = 705,
		Gameinvite_Modeguidecoop = 706,
		Gameinvite_Btnwithfriendcoop = 707,
		Gameinvite_Btnsolocoop = 708,
		Gameinvite_Titlewithfriend = 709,
		Gameinvite_Modeguidewithfriend = 710,
		Gameinvite_Btsmakeroom = 711,
		Gameinvite_Btsjoinroom = 712,
		Gameinvite_Titlemakeroom = 713,
		Gameinvite_Modeguidemakeroom = 714,
		Gameinvite_Btscancle = 715,
		Gameinvite_Titlejoinroom = 716,
		Gameinvite_Modeguidejoinroom = 717,
		Gameinvite_Entercode = 718,
		Gameinvite_Btsconfirm = 719,
		Gameinvite_Codecopyfin = 720,
		Gameinvite_Noroom = 721,
		Gameinvite_Overwaitingtime = 722,
		Gameinvite_Nocopycode = 723,
		Gameinvite_Keyonlynumber = 724,
		Gameinvite_Keyerror = 725,
		Gameinvite_Servererror = 726,
		Gameinvite_Networkerror = 727,
		Shop_Menutext = 801,
		Shop_ResetTime = 802,
		Shop_Taghot = 803,
		Shop_Tagbest = 804,
		Shop_Tagnew = 805,
		Shop_Tagoneplusone = 806,
		Shop_TagValue = 807,
		Shop_Buyfree = 808,
		Shop_Resetshoplist = 809,
		Shop_Resetguide = 810,
		Shop_Restorebuyhistory = 811,
		Shop_Refundguide = 812,
		Shop_Viewdetail = 813,
		Shop_Showitemprobability = 814,
		Shop_Titleeventshop = 850,
		Shop_Titlepackageshop = 851,
		Shop_Titleonedayshop = 852,
		Shop_Titleboxshop = 853,
		Shop_Titlepremiumshop = 854,
		Shop_Titlediashop = 855,
		Shop_Titlegoldshop = 856,
		Itemname_Gold = 10001,
		Itemname_Dia = 10002,
		Itemname_Thropy = 10003,
		Itemname_Seasonthropy = 10004,
		Itemname_Rankthropy = 10005,
		Itemname_Key = 10011,
		Itemname_Vippass = 10051,
		Itemname_Seasonpass = 10052,
		Itemname_Supportbox = 10101,
		Itemname_Normalbox = 10111,
		Itemname_Bronzebox = 10112,
		Itemname_Silverbox = 10113,
		Itemname_Goldbox = 10114,
		Itemname_Platinumbox = 10115,
		Itemname_Diabox = 10116,
		Itemname_Boss01box = 10201,
		Itemname_Boss02box = 10202,
		Itemname_Boss03box = 10203,
		Itemname_Boss04box = 10204,
		Itemname_Boss05box = 10205,
		Itemname_Boss01box_reward = 10206,
		Itemname_Boss02box_reward = 10207,
		Itemname_Boss03box_reward = 10208,
		Itemname_Boss04box_reward = 10209,
		Itemname_Boss05box_reward = 10210,
		Itemname_Dice_1000 = 11000,
		Itemname_Dice_1001 = 11001,
		Itemname_Dice_1002 = 11002,
		Itemname_Dice_1003 = 11003,
		Itemname_Dice_1004 = 11004,
		Itemname_Dice_1005 = 11005,
		Itemname_Dice_1006 = 11006,
		Itemname_Dice_2000 = 12000,
		Itemname_Dice_2001 = 12001,
		Itemname_Dice_2002 = 12002,
		Itemname_Dice_2003 = 12003,
		Itemname_Dice_2004 = 12004,
		Itemname_Dice_2005 = 12005,
		Itemname_Dice_2006 = 12006,
		Itemname_Dice_2007 = 12007,
		Itemname_Dice_2008 = 12008,
		Itemname_Dice_2009 = 12009,
		Itemname_Dice_3000 = 13000,
		Itemname_Dice_3001 = 13001,
		Itemname_Dice_3002 = 13002,
		Itemname_Dice_3003 = 13003,
		Itemname_Dice_3004 = 13004,
		Itemname_Dice_3005 = 13005,
		Itemname_Dice_3006 = 13006,
		Itemname_Dice_3007 = 13007,
		Itemname_Dice_3008 = 13008,
		Itemname_Dice_3009 = 13009,
		Itemname_Dice_3010 = 13010,
		Itemname_Dice_3011 = 13011,
		Itemname_Dice_4000 = 14000,
		Itemname_Dice_4001 = 14001,
		Itemname_Dice_4002 = 14002,
		Itemname_Dice_4003 = 14003,
		Itemname_Dice_4004 = 14004,
		Itemname_Dice_4005 = 14005,
		Itemname_Dice_4006 = 14006,
		Itemname_Dice_4007 = 14007,
		Itemname_Dice_4008 = 14008,
		Itemname_Dice_4009 = 14009,
		Itemname_Dice_4010 = 14010,
		Itemname_Guardian_5001 = 15001,
		Itemname_Guardian_5002 = 15002,
		Itemname_Guardian_5003 = 15003,
		Boss_01 = 16001,
		Boss_02 = 16002,
		Boss_03 = 16003,
		Boss_04 = 16004,
		Boss_05 = 16005,
		Skill_Archer = 21000,
		Skill_Skeleton = 21001,
		Skill_Warrior = 21002,
		Skill_Healer = 21003,
		Skill_Fireball = 21004,
		Skill_Mine = 21005,
		Skill_Fly = 21006,
		Skill_Ninja = 22000,
		Skill_Corrupteddragon = 22001,
		Skill_Shield = 22002,
		Skill_Firemage = 22003,
		Skill_Rifeman = 22004,
		Skill_Wraith = 22005,
		Skill_Stone = 22006,
		Skill_Iceball = 22007,
		Skill_AAgun = 22008,
		Skill_Buffflag = 22009,
		Skill_Mortar = 23000,
		Skill_Skullsummoner = 23001,
		Skill_Rogue = 23002,
		Skill_Redbull = 23003,
		Skill_Volunteer = 23004,
		Skill_Berserker = 23005,
		Skill_Goliath = 23006,
		Skill_Valkyrie = 23007,
		Skill_Corruptedhero = 23008,
		Skill_Lazer = 23009,
		Skill_Rocket = 23010,
		Skill_Sniper = 23011,
		Skill_Babydragon = 24000,
		Skill_Invincible = 24001,
		Skill_Furyoftower = 24002,
		Skill_Golem = 24003,
		Skill_MiniGolem = 24004,
		Skill_Robot = 24005,
		Skill_Magician = 24006,
		Skill_Saint = 24007,
		Skill_Bomber = 24008,
		Skill_Arbiter = 24009,
		Skill_Lightning = 24010,
		Skill_Guardian_30001 = 25001,
		Skill_Guardian_30002 = 25002,
		Skill_Guardian_30003 = 25003,
		Skill_Boss_40001 = 26001,
		Skill_Boss_40002 = 26002,
		Skill_Boss_40003 = 26003,
		Skill_Boss_40004 = 26004,
		Skill_Boss_40005 = 26005,
		Goods_Name_31001 = 31001,
		Goods_Name_31002 = 31002,
		Goods_Name_31003 = 31003,
		Goods_Name_31004 = 31004,
		Goods_Name_32001 = 32001,
		Goods_Name_32002 = 32002,
		Goods_Name_32003 = 32003,
		Goods_Name_32004 = 32004,
		Goods_Name_32005 = 32005,
		Goods_Name_35001 = 35001,
		Goods_Name_37001 = 37001,
		Goods_Name_37002 = 37002,
		Goods_Name_37003 = 37003,
		Goods_Name_37004 = 37004,
		Goods_Name_37005 = 37005,
		Goods_Name_37006 = 37006,
		Goods_Name_37007 = 37007,
		Goods_Name_37008 = 37008,
		Goods_Name_37009 = 37009,
		Goods_Name_37010 = 37010,
		Goods_Name_37011 = 37011,
		Goods_Name_37012 = 37012,
		Goods_Name_38001 = 38001,
		Goods_Name_38002 = 38002,
		Goods_Name_38003 = 38003,
		Goods_Name_38004 = 38004,
		Goods_Name_38005 = 38005,
		Goods_Name_38006 = 38006,
	}

	public class TDataLangKO : ITableData<int>
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
