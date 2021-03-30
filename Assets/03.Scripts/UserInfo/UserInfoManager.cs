using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using ED;
using UnityEngine;
using Service.Core;
using Template.User.RandomwarsUser.Common;
using RandomWarsResource.Data;
using Debug = UnityEngine.Debug;

public class UserInfo
{
    private const string UserIdKey = "UserKey";
    private const string UserNickNameKey = "UserNickNameKey";
    private Action<string> _onSetUserId;
    private Action<string> _onSetUserNickName;

    #region user info variable

    private string _platformID = string.Empty;
    public string platformID
    {
        get => _platformID;
        private set => _platformID = value;
    }

    private string _userID = string.Empty;
    public string userID
    {
        get => _userID;
        private set => _userID = value;
    }
    
    private string _userNickName;
    public string userNickName
    {
        get => _userNickName;
        private set => _userNickName = value;
    }

    public bool isEndTutorial = true;
    public int trophy;
    public int highTrophy;
    public int winCount;
    public int defeatCount;
    public int nClass;
    public int diamond;
    public int gold;
    public int key;
    public int winStreak;

    public int seasonPassId;
    public bool buyVIP;
    public bool buySeasonPass;
    public bool needSeasonReset;
    public bool isFreeSeason;
    public int seasonTrophy;
    public int rankPoint;
    public List<int> seasonPassRewardIds;
    public List<int> trophyRewardIds;
    public List<int> emotionIds;
    public List<int> emotionDeck;
    public DateTime seasonEndTime;
    
    private string _ticketId;
    public string ticketId
    {
        get => _ticketId;
        private set => _ticketId = value;
    }

    public int activateDeckIndex
    {
        get => ObscuredPrefs.GetInt("SelectedDeckNum", 0);
        private set => ObscuredPrefs.SetInt("SelectedDeckNum", value);
    }

    // 서버가 정식으로 붙기 전까진 playerpref 에 저장해두고 가져오고 쓰자
    // private string[] _slotDeck = new string[Global.g_countDeck];
    // public string[] slotDeck
    // {
    //     get => _slotDeck;
    //     private set => _slotDeck = value;
    // }
    private int[][] _arrDeck = new int[3][];
    public int seasonPassRewardStep;

    public int[][] arrDeck
    {
        get => _arrDeck;
        private set => _arrDeck = value;
    }

    public int[] GetActiveDeck => arrDeck[activateDeckIndex];

    /// <summary>
    /// second array
    /// 0 = dice level
    /// 1 = dice count
    /// </summary>
    public Dictionary<int, int[]> dicGettedDice
    {
        get;
        private set;
    }

    public Dictionary<int, int> dicBox
    {
        get;
        private set;
    }
    
    #endregion
    
    
    #region init func

    public UserInfo()
    {
        arrDeck[0] = new int[6] {1000, 1001, 1002, 1003, 1004, 5001};
        arrDeck[1] = new int[6] {1000, 1001, 1002, 1003, 1004, 5001};
        arrDeck[2] = new int[6] {1000, 1001, 1002, 1003, 1004, 5001};

        
        // _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004" );
        // _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004" );
        // _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004" );

        //FixDeckOld();
        
        _platformID = ObscuredPrefs.GetString(UserIdKey, "" );
        _userNickName = ObscuredPrefs.GetString(UserNickNameKey, "" );
        
        // if (_slotDeck[0].Length < 20 || _slotDeck[1].Length < 20 || _slotDeck[2].Length < 20)
        // {
        //
        //     ObscuredPrefs.SetString("Deck", "1000/1001/1002/1003/1004");
        //     ObscuredPrefs.SetString("Deck2", "1000/1001/1002/1003/1004");
        //     ObscuredPrefs.SetString("Deck3", "1000/1001/1002/1003/1004");
        //     ObscuredPrefs.Save();
        //     
        //     _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
        //     _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004");
        //     _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004");
        // }

        if (_userID == "")
        {
            ObscuredPrefs.SetString(UserIdKey, "");
            ObscuredPrefs.Save();    
        }
        
        dicGettedDice = new Dictionary<int, int[]>();
        dicBox = new Dictionary<int, int>();
    }
    
    #endregion

    // public void FixDeckOld()
    // {
    //     string[] deckstr =_slotDeck[0].Split('/');
    //     if (int.Parse(deckstr[0]) < 1000)
    //     {
    //         ObscuredPrefs.SetString("Deck", "1000/1001/1002/1003/1004" );
    //     }
    //     
    //     deckstr =_slotDeck[1].Split('/');
    //     if (int.Parse(deckstr[0]) < 1000)
    //     {
    //         ObscuredPrefs.SetString("Deck2", "1000/1001/1002/1003/1004" );
    //     }
    //     
    //     deckstr =_slotDeck[2].Split('/');
    //     if (int.Parse(deckstr[0]) < 1000)
    //     {
    //         ObscuredPrefs.SetString("Deck3", "1000/1001/1002/1003/1004" );
    //     }
    //     
    //     ObscuredPrefs.Save();
    // }
    //
    //
    
    #region set

    public void SetPlatformID(string id)
    {
        _platformID = id;
        
        ObscuredPrefs.SetString(UserIdKey, _platformID);
        ObscuredPrefs.Save();    
        
        _onSetUserId?.Invoke(id);
    }

    public void SetUserId(string id)
    {
        _userID = id;
    }


    public void SetNickName(string nickname)
    {
        _userNickName = nickname;
        ObscuredPrefs.SetString(UserNickNameKey, _userNickName);
        ObscuredPrefs.Save();
        
        _onSetUserNickName?.Invoke(nickname);
    }

    public void SetTicketId(string ticket)
    {
        _ticketId = ticket;
    }

    public void SetActiveDeck(int index)
    {
        activateDeckIndex = index;
    }

    // public void SetDeck(string deck)
    // {
    //     _slotDeck[activateDeckIndex] = deck;
    //
    //     switch (activateDeckIndex)
    //     {
    //         case 0:
    //             ObscuredPrefs.SetString("Deck", _slotDeck[activateDeckIndex]);
    //             break;
    //         case 1:
    //             ObscuredPrefs.SetString("Deck2", _slotDeck[activateDeckIndex]);
    //             break;
    //         case 2:
    //             ObscuredPrefs.SetString("Deck3", _slotDeck[activateDeckIndex]);
    //             break;
    //     }
    //     
    //     ObscuredPrefs.Save();
    // }

    public void SetDeck(int index, int[] deck)
    {
        //_slotDeck[index] = deck;
        
        // switch (index)
        // {
        //     case 0:
        //         //ObscuredPrefs.SetString("Deck", _slotDeck[index]);
        //         arrDeck[]
        //         break;
        //     case 1:
        //         //ObscuredPrefs.SetString("Deck2", _slotDeck[index]);
        //         break;
        //     case 2:
        //         //ObscuredPrefs.SetString("Deck3", _slotDeck[index]);
        //         break;
        //}
        
        //ObscuredPrefs.Save();
        // List<int> list = new List<int>(deck);
        //
        // arrGuardian[index] = list[0];
        // list.Remove(arrGuardian[index]);
        // arrDeck[index] = list.ToArray();
        arrDeck[index] = deck;
    }

    // public void ResetDeck()
    // {
    //     // ObscuredPrefs.SetString("Deck", "1000/1001/1002/1003/1004");
    //     // ObscuredPrefs.SetString("Deck2", "1000/1001/1002/1003/1004");
    //     // ObscuredPrefs.SetString("Deck3", "1000/1001/1002/1003/1004");
    //     // ObscuredPrefs.Save();
    //     //
    //     // _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004");
    //     // _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004");
    //     // _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004");
    //     
    //     
    // }

    public void AddItem(ItemBaseInfo[] arrayItemBaseInfo, Vector2 ScreenPos)
    {
        List<ItemBaseInfo> list = new List<ItemBaseInfo>();

        foreach (var reward in arrayItemBaseInfo)
        {
            var data = new TDataItemList();
            if (TableManager.Get().ItemList.GetData(reward.ItemId, out data))
            {
                switch (data.id)
                {
                    case 1:             // 골드
                        UserInfoManager.Get().GetUserInfo().gold += reward.Value;
                        UI_GetProduction.Get().Initialize(ITEM_TYPE.GOLD, ScreenPos, Mathf.Clamp(reward.Value, 5, 20));
                        break;
                    case 2:             // 다이아
                        UserInfoManager.Get().GetUserInfo().diamond += reward.Value;
                        UI_GetProduction.Get().Initialize(ITEM_TYPE.DIAMOND, ScreenPos, Mathf.Clamp(reward.Value, 5, 20));
                        break;
                    case 11:
                        UserInfoManager.Get().GetUserInfo().key += reward.Value;
                        UI_GetProduction.Get().Initialize(ITEM_TYPE.KEY, ScreenPos, Mathf.Clamp(reward.Value, 5, 20));
                        break;
                    default: // 주사위
                    {
                        ItemBaseInfo rw = new ItemBaseInfo();
                        rw.ItemId = reward.ItemId;
                        rw.Value = reward.Value;
                        list.Add(rw);
                    }
                        break;
                }
            }
        }

        if (list.Count > 0)
        {
            UI_Main.Get().gerResult.Initialize(list.ToArray(), false, false);
        }
    }
    
    #endregion


    public void RegisterOnSetUserId(Action<string> onSetUserId)
    {
        _onSetUserId = onSetUserId;
    }

    public void RegisterOnSetUserNickName(Action<string> onSetNickName)
    {
        _onSetUserNickName = onSetNickName;
    }
}



public class UserInfoManager : Singleton<UserInfoManager>
{
    #region variable

    private UserInfo _userInfo = null;


    #endregion


    #region unity base

    public override void Awake()
    {   
        if (UserInfoManager.Get() != null && this != UserInfoManager.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        
        base.Awake();
        
        InitializeUserInfo();
    }

    public override void OnDestroy()
    {
        DestroyUserInfo();
        
        base.OnDestroy();
    }
    #endregion


    #region init destroy
    public void InitializeUserInfo()
    {
        _userInfo = new UserInfo();
        
        //print(_userInfo.slotDeck[0]);
        //print(_userInfo.slotDeck[1]);
        //print(_userInfo.slotDeck[2]);
    }

    public void DestroyUserInfo()
    {
        _userInfo = null;
    }
    #endregion
    
    

    #region system
    
    public UserInfo GetUserInfo()
    {
        return _userInfo;
    }

    public void SetUserInfo(MsgUserInfo info, UserSeasonInfo seasonInfo)
    {
        _userInfo.SetUserId(info.UserId);
        _userInfo.SetNickName(info.UserName);

        _userInfo.isEndTutorial = info.EndTutorial;
        _userInfo.diamond = info.Diamond;
        _userInfo.gold = info.Gold;
        _userInfo.key = info.Key;
        _userInfo.trophy = info.Trophy;
        _userInfo.highTrophy = info.HighTrophy;
        _userInfo.winCount = info.WinCount;
        _userInfo.defeatCount = info.DefeatCount;
        _userInfo.nClass = Convert.ToInt32(info.Class);
        _userInfo.winStreak = Convert.ToInt32(info.WinStreak);
        _userInfo.seasonPassId = seasonInfo.SeasonId;
        _userInfo.buyVIP = info.IsBuyVipPass;
        _userInfo.buySeasonPass = seasonInfo.BuySeasonPass;
        _userInfo.seasonTrophy = seasonInfo.SeasonTrophy;
        _userInfo.rankPoint = seasonInfo.RankPoint;
        
        if (seasonInfo.SeasonPassRewardIds != null) _userInfo.seasonPassRewardIds = new List<int>(seasonInfo.SeasonPassRewardIds);
        else _userInfo.seasonPassRewardIds = new List<int>(new int[]{0, 0});
        if (info.TrophyRewardIds != null) _userInfo.trophyRewardIds = new List<int>(info.TrophyRewardIds);
        else _userInfo.trophyRewardIds = new List<int>();
        _userInfo.seasonEndTime = DateTime.Now.AddSeconds(seasonInfo.SeasonResetRemainTime);
        _userInfo.needSeasonReset = seasonInfo.NeedSeasonReset;
        _userInfo.isFreeSeason = seasonInfo.IsFreeSeason;
        _userInfo.seasonPassRewardStep = seasonInfo.SeasonPassRewardStep;

        // if (seasonPassInfo.NeedSeasonReset)
        // {
        //     //UI_Main.Get().seasonEndPopup.Initialize();
        //     UI_Main.Get().ShowMessageBox("시즌 종료", "시즌이 종료되었습니다.", UI_Main.Get().seasonEndPopup.Initialize);
        // }
    }
    
    public void SetUserNickName(string nickname)
    {
        _userInfo.SetNickName(nickname);
    }

    public void SetTicketId(string ticket)
    {
        _userInfo.SetTicketId(ticket);
    }

    public int[] GetActiveDeck()
    {
        return _userInfo.arrDeck[_userInfo.activateDeckIndex];
    }

    public int[] GetSelectDeck(int index)
    {
        return _userInfo.arrDeck[index];
    }

    public void SetActiveDeckIndex(int index)
    {
        _userInfo.SetActiveDeck(index);
    }

    public int GetActiveDeckIndex()
    {
        return _userInfo.activateDeckIndex;
    }

    public void SetDeck(UserDeck[] userDeck)
    {
        for (int i = 0; i < userDeck.Length; i++)
        {
            // string strDeck = string.Format("{0}/{1}/{2}/{3}/{4}", userDeck[i].deckInfo[0], userDeck[i].deckInfo[1],
            //     userDeck[i].deckInfo[2], userDeck[i].deckInfo[3], userDeck[i].deckInfo[4]);
            //Debug.LogFormat("SetDeck[{0}] : {1}", i, strDeck);
            _userInfo.SetDeck(i, userDeck[i].DeckInfo);
        }
    }

    public void SetDice(UserDice[] userDice)
    {
        _userInfo.dicGettedDice.Clear();

        for (int i = 0; i < userDice.Length; i++)
        {
            //Debug.LogFormat("SetDice: ID:{0}, Level:{1}, Count:{2}", userDice[i].diceId, userDice[i].level, userDice[i].count);
            _userInfo.dicGettedDice.Add(userDice[i].DiceId, new int[2] { userDice[i].Level, userDice[i].Count });
        }
    }

    public void SetBox(UserBox[] msgUserBox)
    {
        _userInfo.dicBox.Clear();

        for (int i = 0; i < msgUserBox.Length; i++)
        {
            _userInfo.dicBox.Add(msgUserBox[i].BoxId, msgUserBox[i].Count);
        }
    }

    public void SetItem(UserItemInfo userItemInfo)
    {
        // 상자 아이템
        _userInfo.dicBox.Clear();
        for (int i = 0; i < userItemInfo.listBox.Count; i++)
        {
            _userInfo.dicBox.Add(userItemInfo.listBox[i].ItemId, userItemInfo.listBox[i].Value);
        }

        // 패스 아이템
        for (int i = 0; i < userItemInfo.listPass.Count; i++)
        {
        }

        // 이모티콘
        _userInfo.emotionIds = new List<int>();
        for (int i = 0; i < userItemInfo.listEmoticon.Count; i++)
        {
            _userInfo.emotionIds.Add(userItemInfo.listEmoticon[i].ItemId);
            Debug.Log($"Emotion : id={userItemInfo.listEmoticon[i].ItemId}, value={userItemInfo.listEmoticon[i].Value}");
        }

        // 이모티콘 슬롯
        //userItemInfo.listEmoticonSlot
        _userInfo.emotionDeck = new List<int>();
        foreach (var value in userItemInfo.listEmoticonSlot)
        {
            _userInfo.emotionDeck.Add(value);
            Debug.Log($"Emotion slot : {value}");
        }
    }
    #endregion
}

