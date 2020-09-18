﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class UserInfo
{

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

    private int _activateDeckIndex;
    public int activateDeckIndex
    {
        get => _activateDeckIndex;
        private set => _activateDeckIndex = value;
    }

    // 서버가 정식으로 붙기 전까진 playerpref 에 저장해두고 가져오고 쓰자
    private string[] _slotDeck = new string[Global.g_countDeck];
    public string[] slotDeck
    {
        get => _slotDeck;
        private set => _slotDeck = value;
    }

    public UserInfo()
    {
        _activateDeckIndex = 0;
        
        _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004" );
        _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004" );
        _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004" );

        FixDeckOld();
        
        _userNickName = ObscuredPrefs.GetString("Nickname", "" );
        _userID = ObscuredPrefs.GetString("UserKey", "" );
        
        if (_slotDeck[1] == "" || _slotDeck[2] == "")
        {
            ObscuredPrefs.SetString("Deck2", "1000/1001/1002/1003/1004" );
            ObscuredPrefs.SetString("Deck3", "1000/1001/1002/1003/1004" );
            ObscuredPrefs.Save();    
            
            _slotDeck[0] = ObscuredPrefs.GetString("Deck", "1000/1001/1002/1003/1004" );
            _slotDeck[1] = ObscuredPrefs.GetString("Deck2", "1000/1001/1002/1003/1004" );
            _slotDeck[2] = ObscuredPrefs.GetString("Deck3", "1000/1001/1002/1003/1004" );
        }

        if (_userNickName == "")
        {
            ObscuredPrefs.SetString("Nickname", "" );
            ObscuredPrefs.Save();
        }

        if (_userID == "")
        {
            ObscuredPrefs.SetString("UserKey", "" );
            ObscuredPrefs.Save();    
        }
    }

    public void FixDeckOld()
    {
        string[] deckstr =_slotDeck[0].Split('/');
        if (int.Parse(deckstr[0]) < 1000)
        {
            ObscuredPrefs.SetString("Deck", "1000/1001/1002/1003/1004" );
        }
        
        deckstr =_slotDeck[1].Split('/');
        if (int.Parse(deckstr[0]) < 1000)
        {
            ObscuredPrefs.SetString("Deck2", "1000/1001/1002/1003/1004" );
        }
        
        deckstr =_slotDeck[2].Split('/');
        if (int.Parse(deckstr[0]) < 1000)
        {
            ObscuredPrefs.SetString("Deck3", "1000/1001/1002/1003/1004" );
        }
        
        ObscuredPrefs.Save();    
    }
    
    #region set

    public void SetUserKey(string id)
    {
        _userID = id;
        
        ObscuredPrefs.SetString("UserKey", _userID );
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
        _activateDeckIndex = index;
    }

    public void SetDeck(string deck)
    {
        _slotDeck[_activateDeckIndex] = deck;

        switch (_activateDeckIndex)
        {
            case 0:
                ObscuredPrefs.SetString("Deck", _slotDeck[_activateDeckIndex] );
                break;
            case 1:
                ObscuredPrefs.SetString("Deck2", _slotDeck[_activateDeckIndex] );
                break;
            case 2:
                ObscuredPrefs.SetString("Deck3", _slotDeck[_activateDeckIndex] );
                break;
        }
        
        ObscuredPrefs.Save();
    }

    public void SetDeck(int index, string deck)
    {
        _slotDeck[index] = deck;
        
        switch (index)
        {
            case 0:
                ObscuredPrefs.SetString("Deck", _slotDeck[index] );
                break;
            case 1:
                ObscuredPrefs.SetString("Deck2", _slotDeck[index] );
                break;
            case 2:
                ObscuredPrefs.SetString("Deck3", _slotDeck[index] );
                break;
        }
        
        ObscuredPrefs.Save();
    }
    
    
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

    public void Start()
    {
        
    }

    public void Update()
    {
        
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
    public void SetDeck(int deckIndex , string deck)
    {
        _userInfo.SetDeck(deck);
    }

    public string GetActiveDeck()
    {
        return _userInfo.slotDeck[_userInfo.activateDeckIndex];
    }

    public string GetSelectDeck(int index)
    {
        return _userInfo.slotDeck[index];
    }

    public void SetActiveDeckIndex(int index)
    {
        _userInfo.SetActiveDeck(index);
    }

    public int GetActiveDeckIndex()
    {
        return _userInfo.activateDeckIndex;
    }
    #endregion
}

