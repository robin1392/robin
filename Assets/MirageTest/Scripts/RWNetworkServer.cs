using System;
using System.Collections;
using System.Collections.Generic;
using Mirage;
using MirageTest.Scripts;
using UnityEngine;

public class RWNetworkServer : NetworkServer
{
    public ServerGameLogic serverGameLogic;
    private void Awake()
    {
        serverGameLogic = GetComponent<ServerGameLogic>();
    }
}
