using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class WebPacket : Singleton<WebPacket>
{
    
    //
    
    
    #region unity base
    public override void Awake()
    {
        base.Awake();
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


[Serializable]
public class UserAuthReq
{
    public string userId;
}


[Serializable]
public class MatchRequestReq
{
    public string userId;
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
    public string gameSessionId;
}

