using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;



public class NetSyncData
{
    public int userId;
    public float towerHp;
    public List<NetSyncMinionData> netSyncMinionData = null;
    
    public NetSyncData()
    {
        netSyncMinionData = new List<NetSyncMinionData>();
    }

    public void NetSyncDestroy()
    {
        netSyncMinionData.Clear();
        netSyncMinionData = null;
    }
}

public class NetSyncMinionData
{
    public int minionId;
    
    public int minionDataId;
    public float minionHp;
    public Vector3 minionPos;


    public NetSyncMinionData()
    {
        
    }
}