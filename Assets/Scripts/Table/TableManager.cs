using System;
using UnityEngine;
using Table;
using Table.Data;
using System.Net;
using System.IO;
public class TableManager : Singleton<TableManager>
{
    public string BucketUrl;
    public string Enviroment = "DEV";

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
    public TableData<string, TDataLangEN> LangEN { get; private set; }
    public TableData<string, TDataLangKO> LangKO { get; private set; }
    public TableData<int, TDataErrorMessageKO> ErrorMessageKO { get; private set; }


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
        LangEN = new TableData<string, TDataLangEN>();
        LangKO = new TableData<string, TDataLangKO>();
        ErrorMessageKO = new TableData<int, TDataErrorMessageKO>();
    }


    public void Init(string localPath)
    {
        string remoteTDataVersion = string.Empty;
        string localTDataVersion = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BucketUrl + "/AppSettings.dat");
        request.ContentType = "application/json";
        request.Method = "GET";
        var response = (HttpWebResponse)request.GetResponse();
        var resStream = response.GetResponseStream();

        using (BinaryReader reader = new BinaryReader(resStream))
        {
            remoteTDataVersion = reader.ReadString();
        }

        if (File.Exists(localPath + "AppSettings.dat") == true)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(localPath + "AppSettings.dat", FileMode.Open)))
            {
                localTDataVersion = reader.ReadString();
            }
        }

        // 서버 데이터와 로컬 데이터의 버젼을 비교한다.
        if (remoteTDataVersion != localTDataVersion)
        {
            if (Directory.Exists(localPath) == false)
            {
                Directory.CreateDirectory(localPath);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(localPath + "AppSettings.dat", FileMode.Create, FileAccess.ReadWrite)))
            {
                writer.Write(remoteTDataVersion);
            }

            LoadFromBucket(BucketUrl + "/table/" + Enviroment, localPath + "table");
        }
        else
        {
            LoadFromFile(localPath + "table");
        }
    }


    bool LoadFromBucket(string bucketPath, string localPath)
    {
        BoxList.Init(new TableLoaderRemoteCSV<int, TDataBoxList>(), bucketPath, "BoxList.csv", localPath);
        BoxProductInfo.Init(new TableLoaderRemoteCSV<int, TDataBoxProductInfo>(), bucketPath, "BoxProductInfo.csv", localPath);
        CoopMode.Init(new TableLoaderRemoteCSV<int, TDataCoopMode>(), bucketPath, "CoopMode.csv", localPath);
        CoopModeMinion.Init(new TableLoaderRemoteCSV<int, TDataCoopModeMinion>(), bucketPath, "CoopModeMinion.csv", localPath);
        CoopModeBossInfo.Init(new TableLoaderRemoteCSV<int, TDataCoopModeBossInfo>(), bucketPath, "CoopModeBossInfo.csv", localPath);
        DiceInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceInfo>(), bucketPath, "DiceInfo.csv", localPath);
        DiceUpgrade.Init(new TableLoaderRemoteCSV<int, TDataDiceUpgrade>(), bucketPath, "DiceUpgrade.csv", localPath);
        DiceLevelInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceLevelInfo>(), bucketPath, "DiceLevelInfo.csv", localPath);
        GuardianInfo.Init(new TableLoaderRemoteCSV<int, TDataGuardianInfo>(), bucketPath, "GuardianInfo.csv", localPath);
        Vsmode.Init(new TableLoaderRemoteCSV<int, TDataVsmode>(), bucketPath, "Vsmode.csv", localPath);
        return true;
    }


    bool LoadFromFile(string path)
    {
        BoxList.Init(new TableLoaderLocalCSV<int, TDataBoxList>(), path, "BoxList.csv");
        BoxProductInfo.Init(new TableLoaderLocalCSV<int, TDataBoxProductInfo>(), path, "BoxProductInfo.csv");
        CoopMode.Init(new TableLoaderLocalCSV<int, TDataCoopMode>(), path, "CoopMode.csv");
        CoopModeMinion.Init(new TableLoaderLocalCSV<int, TDataCoopModeMinion>(), path, "CoopModeMinion.csv");
        CoopModeBossInfo.Init(new TableLoaderLocalCSV<int, TDataCoopModeBossInfo>(), path, "CoopModeBossInfo.csv");
        DiceInfo.Init(new TableLoaderLocalCSV<int, TDataDiceInfo>(), path, "DiceInfo.csv");
        DiceUpgrade.Init(new TableLoaderLocalCSV<int, TDataDiceUpgrade>(), path, "DiceUpgrade.csv");
        DiceLevelInfo.Init(new TableLoaderLocalCSV<int, TDataDiceLevelInfo>(), path, "DiceLevelInfo.csv");
        GuardianInfo.Init(new TableLoaderLocalCSV<int, TDataGuardianInfo>(), path, "GuardianInfo.csv");
        Vsmode.Init(new TableLoaderLocalCSV<int, TDataVsmode>(), path, "Vsmode.csv");
        return true;
    }
}

