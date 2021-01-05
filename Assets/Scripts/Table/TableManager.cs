using System;
using UnityEngine;
using RandomWarsResource;
using RandomWarsResource.Data;
using System.Net;
using System.IO;
public class TableManager : Singleton<TableManager>
{
    public string BucketUrl;
    public string Enviroment = "DEV";

    public TableData<int, TDataBoxProductInfo> BoxProductInfo { get; private set; }
    public TableData<int, TDataCoopMode> CoopMode { get; private set; }
    public TableData<int, TDataCoopModeMinion> CoopModeMinion { get; private set; }
    public TableData<int, TDataCoopModeBossInfo> CoopModeBossInfo { get; private set; }
    public TableData<int, TDataDiceInfo> DiceInfo { get; private set; }
    public TableData<int, TDataDiceUpgrade> DiceUpgrade { get; private set; }
    public TableData<int, TDataDiceLevelInfo> DiceLevelInfo { get; private set; }
    public TableData<int, TDataGuardianInfo> GuardianInfo { get; private set; }
    public TableData<int, TDataVsmode> Vsmode { get; private set; }
    //public TableData<int, TDataLangEN> LangEN { get; private set; }
    public TableData<int, TDataLangKO> LangKO { get; private set; }
    public TableData<int, TDataErrorMessageKO> ErrorMessageKO { get; private set; }
    public TableData<int, TDataRankingReward> RankingReward { get; private set; }
    public TableData<int, TDataSeasonpassInfo> SeasonpassInfo { get; private set; }
    public TableData<int, TDataSeasonpassReward> SeasonpassReward { get; private set; }
    public TableData<int, TDataClassReward> ClassReward { get; private set; }
    public TableData<int, TDataItemList> ItemList { get; private set; }
    public TableData<int, TDataQuestInfo> QuestInfo { get; private set; }
    public TableData<int, TDataQuestData> QuestData { get; private set; }
    public TableData<int, TDataQuestDayReward> QuestDayReward { get; private set; }



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
        BoxProductInfo = new TableData<int, TDataBoxProductInfo>();
        CoopMode = new TableData<int, TDataCoopMode>();
        CoopModeMinion = new TableData<int, TDataCoopModeMinion>();
        CoopModeBossInfo = new TableData<int, TDataCoopModeBossInfo>();
        DiceInfo = new TableData<int, TDataDiceInfo>();
        DiceUpgrade = new TableData<int, TDataDiceUpgrade>();
        DiceLevelInfo = new TableData<int, TDataDiceLevelInfo>();
        GuardianInfo = new TableData<int, TDataGuardianInfo>();
        Vsmode = new TableData<int, TDataVsmode>();
        //LangEN = new TableData<int, TDataLangEN>();
        LangKO = new TableData<int, TDataLangKO>();
        ErrorMessageKO = new TableData<int, TDataErrorMessageKO>();
        RankingReward = new TableData<int, TDataRankingReward>();
        SeasonpassInfo = new TableData<int, TDataSeasonpassInfo>();
        SeasonpassReward = new TableData<int, TDataSeasonpassReward>();
        ClassReward = new TableData<int, TDataClassReward>();
        ItemList = new TableData<int, TDataItemList>();
        QuestInfo = new TableData<int, TDataQuestInfo>();
        QuestData = new TableData<int, TDataQuestData>();
        QuestDayReward = new TableData<int, TDataQuestDayReward>();
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

        // ���� �����Ϳ� ���� �������� ������ ���Ѵ�.
        if (remoteTDataVersion != localTDataVersion)
        {
            if (Directory.Exists(localPath) == false)
            {
                Directory.CreateDirectory(localPath);
            }

            if (LoadFromBucket(BucketUrl + "/table/" + Enviroment, localPath + "table") == true)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(localPath + "AppSettings.dat", FileMode.Create, FileAccess.ReadWrite)))
                {
                    writer.Write(remoteTDataVersion);
                }

                Debug.Log("Download Table Complete !!! ");
            }
        }
        else
        {
            LoadFromFile(localPath + "table");
        }
    }


    bool LoadFromBucket(string bucketPath, string localPath)
    {
        BoxProductInfo.Init(new TableLoaderRemoteCSV<int, TDataBoxProductInfo>(), bucketPath, "BoxProductInfo.csv", localPath);
        CoopMode.Init(new TableLoaderRemoteCSV<int, TDataCoopMode>(), bucketPath, "CoopMode.csv", localPath);
        CoopModeMinion.Init(new TableLoaderRemoteCSV<int, TDataCoopModeMinion>(), bucketPath, "CoopModeMinion.csv", localPath);
        CoopModeBossInfo.Init(new TableLoaderRemoteCSV<int, TDataCoopModeBossInfo>(), bucketPath, "CoopModeBossInfo.csv", localPath);
        DiceInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceInfo>(), bucketPath, "DiceInfo.csv", localPath);
        DiceUpgrade.Init(new TableLoaderRemoteCSV<int, TDataDiceUpgrade>(), bucketPath, "DiceUpgrade.csv", localPath);
        DiceLevelInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceLevelInfo>(), bucketPath, "DiceLevelInfo.csv", localPath);
        GuardianInfo.Init(new TableLoaderRemoteCSV<int, TDataGuardianInfo>(), bucketPath, "GuardianInfo.csv", localPath);
        Vsmode.Init(new TableLoaderRemoteCSV<int, TDataVsmode>(), bucketPath, "Vsmode.csv", localPath);
       // LangEN.Init(new TableLoaderRemoteCSV<int, TDataLangEN>(), bucketPath, "LangEN.csv", localPath);
        LangKO.Init(new TableLoaderRemoteCSV<int, TDataLangKO>(), bucketPath, "LangKO.csv", localPath);
        RankingReward.Init(new TableLoaderRemoteCSV<int, TDataRankingReward>(), bucketPath, "RankingReward.csv", localPath);
        SeasonpassInfo.Init(new TableLoaderRemoteCSV<int, TDataSeasonpassInfo>(), bucketPath, "SeasonpassInfo.csv", localPath);
        SeasonpassReward.Init(new TableLoaderRemoteCSV<int, TDataSeasonpassReward>(), bucketPath, "SeasonpassReward.csv", localPath);
        ClassReward.Init(new TableLoaderRemoteCSV<int, TDataClassReward>(), bucketPath, "ClassReward.csv", localPath);
        ItemList.Init(new TableLoaderRemoteCSV<int, TDataItemList>(), bucketPath, "ItemList.csv", localPath);
        QuestInfo.Init(new TableLoaderRemoteCSV<int, TDataQuestInfo>(), bucketPath, "QuestInfo.csv", localPath);
        QuestData.Init(new TableLoaderRemoteCSV<int, TDataQuestData>(), bucketPath, "QuestData.csv", localPath);
        QuestDayReward.Init(new TableLoaderRemoteCSV<int, TDataQuestDayReward>(), bucketPath, "QuestDayReward.csv", localPath);
        return true;
    }


    bool LoadFromFile(string path)
    {
        BoxProductInfo.Init(new TableLoaderLocalCSV<int, TDataBoxProductInfo>(), path, "BoxProductInfo.csv");
        CoopMode.Init(new TableLoaderLocalCSV<int, TDataCoopMode>(), path, "CoopMode.csv");
        CoopModeMinion.Init(new TableLoaderLocalCSV<int, TDataCoopModeMinion>(), path, "CoopModeMinion.csv");
        CoopModeBossInfo.Init(new TableLoaderLocalCSV<int, TDataCoopModeBossInfo>(), path, "CoopModeBossInfo.csv");
        DiceInfo.Init(new TableLoaderLocalCSV<int, TDataDiceInfo>(), path, "DiceInfo.csv");
        DiceUpgrade.Init(new TableLoaderLocalCSV<int, TDataDiceUpgrade>(), path, "DiceUpgrade.csv");
        DiceLevelInfo.Init(new TableLoaderLocalCSV<int, TDataDiceLevelInfo>(), path, "DiceLevelInfo.csv");
        GuardianInfo.Init(new TableLoaderLocalCSV<int, TDataGuardianInfo>(), path, "GuardianInfo.csv");
        Vsmode.Init(new TableLoaderLocalCSV<int, TDataVsmode>(), path, "Vsmode.csv");
        //LangEN.Init(new TableLoaderLocalCSV<int, TDataLangEN>(), path, "LangEN.csv");
        LangKO.Init(new TableLoaderLocalCSV<int, TDataLangKO>(), path, "LangKO.csv");
        RankingReward.Init(new TableLoaderLocalCSV<int, TDataRankingReward>(), path, "RankingReward.csv");
        SeasonpassInfo.Init(new TableLoaderLocalCSV<int, TDataSeasonpassInfo>(), path, "SeasonpassInfo.csv");
        SeasonpassReward.Init(new TableLoaderLocalCSV<int, TDataSeasonpassReward>(), path, "SeasonpassReward.csv");
        ClassReward.Init(new TableLoaderLocalCSV<int, TDataClassReward>(), path, "ClassReward.csv");
        ItemList.Init(new TableLoaderLocalCSV<int, TDataItemList>(), path, "ItemList.csv");
        QuestInfo.Init(new TableLoaderLocalCSV<int, TDataQuestInfo>(), path, "QuestInfo.csv");
        QuestData.Init(new TableLoaderLocalCSV<int, TDataQuestData>(), path, "QuestData.csv");
        QuestDayReward.Init(new TableLoaderLocalCSV<int, TDataQuestDayReward>(), path, "QuestDayReward.csv");

        return true;
    }
}

