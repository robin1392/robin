using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class WebPacket : Singleton<WebPacket>
{
    
    //
    private bool _isPacketSend;
    public bool isPacketSend
    {
        get { return _isPacketSend; }
    }


    public Global.E_MATCHSTEP netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
    
    
    #region unity base
    public override void Awake()
    {
        base.Awake();
        
        _isPacketSend = false;
        netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion
}



#region web packet define

[Serializable]
public class UserAuthReq
{
    public string userId;
}

[Serializable]
public class UserAuthRes
{
    public User userInfo;
    public UserDeck[] userDeck;
    public UserDice[] userDice;
}

[Serializable]
public class MatchRequestReq
{
    public string userId;
}

[Serializable]
public class MatchRequestAck
{
    public string ticketId;
}

[Serializable]
public class MatchStatusReq
{
    public string ticketId;
}

[Serializable]
public class MatchStatusAck
{
    public string serverAddr;
    public int port;
    public string playerSessionId;
}

[Serializable]
public class DeckUpdateReq
{
    public string userId;
    public sbyte deckIndex;
    public int[] diceIds;
}

[Serializable]
public class DeckUpdateAck
{
    public sbyte deckIndex;
    public int[] diceIds = new int[5];
}

[Serializable]
public class UserTotalInfo
{
    public User userInfo;
    public UserDeck[] userDeck;
    public UserDice[] userDice;
}


[Serializable]
public class User
{
    public string userId;
    public string name;
    public int trophy;
    public int gold;
    public int diamond;
}


[Serializable]
public class UserDeck
{
    public sbyte index;
    public int[] deckInfo;
}


[Serializable]
public class UserDice
{
    public int diceId;
    public short level = 1;
    public short count = 1;
}


#endregion
