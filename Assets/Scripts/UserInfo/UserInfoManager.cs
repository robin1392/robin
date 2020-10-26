﻿using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using RandomWarsProtocol;

public class UserInfo
{

    #region user info variable
    
    private string _userID;
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

    public int[][] arrDeck
    {
        get => _arrDeck;
        private set => _arrDeck = value;
    }

    public Dictionary<int, int[]> dicGettedDice
    {
        get;
        private set;
    }
    
    #endregion
    
    
    #region init func

    public UserInfo()
    {
        arrDeck[0] = new int[5] {1000, 1001, 1002, 1003, 1004};
        arrDeck[1] = new int[5] {1000, 1001, 1002, 1003, 1004};
        arrDeck[2] = new int[5] {1000, 1001, 1002, 1003, 1004};

        dicGettedDice = new Dictionary<int, int[]>();
        
        // _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004" );
        // _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004" );
        // _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004" );

        //FixDeckOld();
        
        _userNickName = ObscuredPrefs.GetString("Nickname", "" );
        _userID = ObscuredPrefs.GetString("UserKey", "" );
        
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

        if (_userNickName == "")
        {
            ObscuredPrefs.SetString("Nickname", "");
            ObscuredPrefs.Save();
        }

        if (_userID == "")
        {
            ObscuredPrefs.SetString("UserKey", "");
            ObscuredPrefs.Save();    
        }
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

    public void SetUserKey(string id)
    {
        _userID = id;
        
        ObscuredPrefs.SetString("UserKey", _userID);
        ObscuredPrefs.Save();    
    }
    
    public void SetNickName(string nickname)
    {
        _userNickName = nickname;
        
        ObscuredPrefs.SetString("Nickname", _userNickName );
        ObscuredPrefs.Save();
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
    #endregion
    

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
    
    public void SetUserKey(string userid)
    {
        _userInfo.SetUserKey(userid);
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

    public void SetDeck(MsgUserDeck[] userDeck)
    {
        for (int i = 0; i < userDeck.Length; i++)
        {
            // string strDeck = string.Format("{0}/{1}/{2}/{3}/{4}", userDeck[i].deckInfo[0], userDeck[i].deckInfo[1],
            //     userDeck[i].deckInfo[2], userDeck[i].deckInfo[3], userDeck[i].deckInfo[4]);
            //Debug.LogFormat("SetDeck[{0}] : {1}", i, strDeck);
            _userInfo.SetDeck(i, userDeck[i].DeckInfo);
        }
    }

    public void SetDice(MsgUserDice[] userDice)
    {
        _userInfo.dicGettedDice.Clear();

        for (int i = 0; i < userDice.Length; i++)
        {
            //Debug.LogFormat("SetDice: ID:{0}, Level:{1}, Count:{2}", userDice[i].diceId, userDice[i].level, userDice[i].count);
            _userInfo.dicGettedDice.Add(userDice[i].DiceId, new int[2] { userDice[i].Level, userDice[i].Count });
        }
    }
    #endregion
}

