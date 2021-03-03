using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;



public class NetSyncData
{
    public uint userId;
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
    public uint minionId;
    
    public int minionDataId;
    public float minionHp;
    public float minionMaxHp;
    public float minionPower;
    public float minionEffect;
    public float minionEffectUpgrade;
    public float minionEffectIngameUpgrade;
    public float minionDuration;
    public float minionCooltime;
    public Vector3 minionPos;


    public NetSyncMinionData()
    {
        
    }
}