using System;
using UnityEngine;
using Table;
using Table.Data;

public class TableManager : Singleton<TableManager>
{
    public TableData<int, TDataBoxList> BoxList { get; private set; }
    public TableData<int, TDataBoxProductInfo> BoxProductInfo { get; private set; }
    public TableData<int, TDataCoopMode> CoopMode { get; private set; }
    public TableData<int, TDataCoopModeMinion> CoopModeMinion { get; private set; }
    public TableData<int, TDataCoopModeBossInfo> CoopModeBossInfo { get; private set; }
    public TableData<int, TDataDiceInfo> DiceInfo { get; private set; }
    public TableData<int, TDataDiceUpgrade> DiceUpgrade { get; private set; }
    public TableData<int, TDataDiceLevelInfo> DiceLevelInfo { get; private set; }
    public TableData<int, TDataGuardianInfo> GuardianInfo { get; private set; }
    public TableData<int, TDataVsmode> Vsmode { get; private set; }



    public void Awake()
    {
        if (TableManager.Get() != null && this != TableManager.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        base.Awake();

    }


    void Start()
    {
        BoxList = new TableData<int, TDataBoxList>();
        BoxProductInfo = new TableData<int, TDataBoxProductInfo>();
        CoopMode = new TableData<int, TDataCoopMode>();
        CoopModeMinion = new TableData<int, TDataCoopModeMinion>();
        CoopModeBossInfo = new TableData<int, TDataCoopModeBossInfo>();
        DiceInfo = new TableData<int, TDataDiceInfo>();
        DiceUpgrade = new TableData<int, TDataDiceUpgrade>();
        DiceLevelInfo = new TableData<int, TDataDiceLevelInfo>();
        GuardianInfo = new TableData<int, TDataGuardianInfo>();
        Vsmode = new TableData<int, TDataVsmode>();
    }


    public bool Load(string path)
    {
        BoxList.Init(new TableLoaderLocalCSV<int, TDataBoxList>(), path + "/BoxList.csv");
        BoxProductInfo.Init(new TableLoaderLocalCSV<int, TDataBoxProductInfo>(), path + "/BoxProductInfo.csv");
        CoopMode.Init(new TableLoaderLocalCSV<int, TDataCoopMode>(), path + "/CoopMode.csv");
        CoopModeMinion.Init(new TableLoaderLocalCSV<int, TDataCoopModeMinion>(), path + "/CoopModeMinion.csv");
        CoopModeBossInfo.Init(new TableLoaderLocalCSV<int, TDataCoopModeBossInfo>(), path + "/CoopModeBossInfo.csv");
        DiceInfo.Init(new TableLoaderLocalCSV<int, TDataDiceInfo>(), path + "/DiceInfo.csv");
        DiceUpgrade.Init(new TableLoaderLocalCSV<int, TDataDiceUpgrade>(), path + "/DiceUpgrade.csv");
        DiceLevelInfo.Init(new TableLoaderLocalCSV<int, TDataDiceLevelInfo>(), path + "/DiceLevelInfo.csv");
        GuardianInfo.Init(new TableLoaderLocalCSV<int, TDataGuardianInfo>(), path + "/GuardianInfo.csv");
        //Vsmode.Init(new TableLoaderLocalCSV<int, TDataVsmode>(), path + "/Vsmode.csv");
        return true;
    }
}

