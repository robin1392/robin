using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{

    private string _userID;
    public string userID
    {
        get => _userID;
        private set => _userID = value;
    }

    private int _activateDeckIndex;
    public int activateDeckIndex
    {
        get => _activateDeckIndex;
        private set => _activateDeckIndex = value;
    }

    private string[] _slotDeck = new string[Global.g_countDeck];
    public string[] slotDeck
    {
        get => _slotDeck;
        private set => _slotDeck = value;
    }

    public UserInfo()
    {
    }

}



public class UserInfoManager : Singleton<UserInfoManager>
{
    #region variable

    

    #endregion


    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion


    #region init destroy

    

    #endregion


    #region system

    

    #endregion
}

