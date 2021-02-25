using System;
using UnityEngine;
using RandomWarsResource;
using RandomWarsResource.Data;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;



public class TableManager : Singleton<TableManager>
{
    public string BucketUrl;
    public string Enviroment = "DEV";

    public TableData<int, TDataBoxOpenInfo> BoxProductInfo { get; private set; }
    public TableData<int, TDataCoopMode> CoopMode { get; private set; }
    public TableData<int, TDataCoopModeMinion> CoopModeMinion { get; private set; }
    public TableData<int, TDataCoopModeBossInfo> CoopModeBossInfo { get; private set; }
    public TableData<int, TDataDiceInfo> DiceInfo { get; private set; }
    public TableData<int, TDataDiceUpgrade> DiceUpgrade { get; private set; }
    public TableData<int, TDataDiceLevelInfo> DiceLevelInfo { get; private set; }
    public TableData<int, TDataGuardianInfo> GuardianInfo { get; private set; }
    public TableData<int, TDataVsmode> Vsmode { get; private set; }
    public TableData<int, TDataLangEN> LangEN { get; private set; }
    public TableData<int, TDataLangKO> LangKO { get; private set; }
    public TableData<int, TDataErrorMessageKO> ErrorMessageKO { get; private set; }
    public TableData<int, TDataRankingReward> RankingReward { get; private set; }
    public TableData<int, TDataRankInfo> RankInfo { get; private set; }
    public TableData<int, TDataSeasonpassInfo> SeasonpassInfo { get; private set; }
    public TableData<int, TDataSeasonpassReward> SeasonpassReward { get; private set; }
    public TableData<int, TDataClassReward> ClassReward { get; private set; }
    public TableData<int, TDataItemList> ItemList { get; private set; }
    public TableData<int, TDataQuestInfo> QuestInfo { get; private set; }
    public TableData<int, TDataQuestData> QuestData { get; private set; }
    public TableData<int, TDataQuestDayReward> QuestDayReward { get; private set; }
    public TableData<int, TDataShopInfo> ShopInfo { get; private set; }
    public TableData<int, TDataShopProductList> ShopProductList { get; private set; }



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
        BoxProductInfo = new TableData<int, TDataBoxOpenInfo>();
        CoopMode = new TableData<int, TDataCoopMode>();
        CoopModeMinion = new TableData<int, TDataCoopModeMinion>();
        CoopModeBossInfo = new TableData<int, TDataCoopModeBossInfo>();
        DiceInfo = new TableData<int, TDataDiceInfo>();
        DiceUpgrade = new TableData<int, TDataDiceUpgrade>();
        DiceLevelInfo = new TableData<int, TDataDiceLevelInfo>();
        GuardianInfo = new TableData<int, TDataGuardianInfo>();
        Vsmode = new TableData<int, TDataVsmode>();
        LangEN = new TableData<int, TDataLangEN>();
        LangKO = new TableData<int, TDataLangKO>();
        ErrorMessageKO = new TableData<int, TDataErrorMessageKO>();
        RankingReward = new TableData<int, TDataRankingReward>();
        RankInfo = new TableData<int, TDataRankInfo>();
        SeasonpassInfo = new TableData<int, TDataSeasonpassInfo>();
        SeasonpassReward = new TableData<int, TDataSeasonpassReward>();
        ClassReward = new TableData<int, TDataClassReward>();
        ItemList = new TableData<int, TDataItemList>();
        QuestInfo = new TableData<int, TDataQuestInfo>();
        QuestData = new TableData<int, TDataQuestData>();
        QuestDayReward = new TableData<int, TDataQuestDayReward>();
        ItemList = new TableData<int, TDataItemList>();
        DiceInfo = new TableData<int, TDataDiceInfo>();
        DiceLevelInfo = new TableData<int, TDataDiceLevelInfo>();
        ShopInfo = new TableData<int, TDataShopInfo>();
        ShopProductList = new TableData<int, TDataShopProductList>();
    }


    public void Init(string localPath)
    {
        string remoteTDataVersion = string.Empty;
        string localTDataVersion = string.Empty;
        string url = BucketUrl + "/Table/" + Enviroment;


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "/version.json");
        request.ContentType = "application/json";
        request.Method = "GET";
        var response = (HttpWebResponse)request.GetResponse();
        var resStream = response.GetResponseStream();
        
        StreamReader streamRead = new StreamReader(resStream);
        string jsonServer = streamRead.ReadToEnd();
        jsonServer = jsonServer.Replace("\\", "");
        var jObjServer = Newtonsoft.Json.Linq.JObject.Parse(jsonServer);
        remoteTDataVersion = (string)jObjServer["dataVersion"];


        string targetPath = localPath + "Table/" + Enviroment;
        if (File.Exists(targetPath + "/version.json") == true)
        {
            using (StreamReader r = new StreamReader(targetPath + "/version.json"))
            {
                string jsonClient = r.ReadToEnd();
                var jObjClient = Newtonsoft.Json.Linq.JObject.Parse(jsonClient);
                localTDataVersion = (string)jObjClient["dataVersion"];
            }
        }

        if (remoteTDataVersion != localTDataVersion)
        {
            // 패치 파일 로컬 저장
            if (RequestPatchFile(url + "/" + remoteTDataVersion + ".zip", targetPath) == false)
            {
                return;
            }

            // 서버 버젼 파일 로컬 저장
            File.WriteAllText(targetPath + "/version.json", jsonServer);
            Debug.Log("Download Table Complete !!! ");
        }

        LoadFromFile(targetPath);
    }


    bool RequestPatchFile(string url, string targetPath)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();


            if (Directory.Exists(targetPath) == false)
            {
                Directory.CreateDirectory(targetPath);
            }


            using (var fileStream = new FileStream(targetPath + "/patchFile.zip", FileMode.Create, FileAccess.Write))
            {
                resStream.CopyTo(fileStream);
            }

            LoadZipFile(targetPath + "/patchFile.zip", targetPath);
            File.Delete(targetPath + "/patchFile.zip");
        }
        catch (Exception e)
        {
            Debug.LogError("Download Table Failed!!! error: " + e.Message);
            return false;
        }

        return true;
    }

    void LoadZipFile(string FilePath, string targetPath)
    {
        if (System.IO.File.Exists(FilePath) == false)
            return;

        // Read file
        FileStream fs = null;
        try
        {
            fs = new FileStream(FilePath, FileMode.Open);
        }
        catch
        {
            Debug.Log("GameData file open exception: " + FilePath);
        }

        if (fs != null)
        {
            try
            {
                // Read zip file
                ZipFile zf = new ZipFile(fs);
                int numFiles = 0;

                if (zf.TestArchive(true) == false)
                {
                    Debug.Log("Zip file failed integrity check!");
                    zf.IsStreamOwner = false;
                    zf.Close();
                    fs.Close();
                }
                else
                {
                    foreach (ZipEntry zipEntry in zf)
                    {
                        // Ignore directories
                        if (!zipEntry.IsFile)
                            continue;

                        String entryFileName = zipEntry.Name;

                        // Skip .DS_Store files (these appear on OSX)
                        if (entryFileName.Contains("DS_Store"))
                            continue;

                        //Debug.Log("Unpacking zip file entry: " + entryFileName);

                        byte[] buffer = new byte[4096];     // 4K is optimum
                        Stream zipStream = zf.GetInputStream(zipEntry);

                        // Manipulate the output filename here as desired.
                        string fullZipToPath = targetPath + "\\" + Path.GetFileName(entryFileName);

                        // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                        // of the file, but does not waste memory.
                        // The "using" will close the stream even if an exception occurs.
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                        numFiles++;
                    }

                    zf.IsStreamOwner = false;
                    zf.Close();
                    fs.Close();
                }
            }
            catch
            {
                Debug.Log("Zip file error!");
            }
        }
    }


    bool LoadFromFile(string path)
    {
        BoxProductInfo.Init(new TableLoaderLocalCSV<int, TDataBoxOpenInfo>(), path, "BoxOpenInfo.csv");
        CoopMode.Init(new TableLoaderLocalCSV<int, TDataCoopMode>(), path, "CoopMode.csv");
        CoopModeMinion.Init(new TableLoaderLocalCSV<int, TDataCoopModeMinion>(), path, "CoopModeMinion.csv");
        CoopModeBossInfo.Init(new TableLoaderLocalCSV<int, TDataCoopModeBossInfo>(), path, "CoopModeBossInfo.csv");
        DiceInfo.Init(new TableLoaderLocalCSV<int, TDataDiceInfo>(), path, "DiceInfo.csv");
        DiceUpgrade.Init(new TableLoaderLocalCSV<int, TDataDiceUpgrade>(), path, "DiceUpgrade.csv");
        DiceLevelInfo.Init(new TableLoaderLocalCSV<int, TDataDiceLevelInfo>(), path, "DiceLevelInfo.csv");
        GuardianInfo.Init(new TableLoaderLocalCSV<int, TDataGuardianInfo>(), path, "GuardianInfo.csv");
        Vsmode.Init(new TableLoaderLocalCSV<int, TDataVsmode>(), path, "Vsmode.csv");
        LangEN.Init(new TableLoaderLocalCSV<int, TDataLangEN>(), path, "LangEN.csv");
        LangKO.Init(new TableLoaderLocalCSV<int, TDataLangKO>(), path, "LangKO.csv");
        RankingReward.Init(new TableLoaderLocalCSV<int, TDataRankingReward>(), path, "RankingReward.csv");
        RankInfo.Init(new TableLoaderLocalCSV<int, TDataRankInfo>(), path, "RankInfo.csv");
        SeasonpassInfo.Init(new TableLoaderLocalCSV<int, TDataSeasonpassInfo>(), path, "SeasonpassInfo.csv");
        SeasonpassReward.Init(new TableLoaderLocalCSV<int, TDataSeasonpassReward>(), path, "SeasonpassReward.csv");
        ClassReward.Init(new TableLoaderLocalCSV<int, TDataClassReward>(), path, "ClassReward.csv");
        ItemList.Init(new TableLoaderLocalCSV<int, TDataItemList>(), path, "ItemList.csv");
        QuestInfo.Init(new TableLoaderLocalCSV<int, TDataQuestInfo>(), path, "QuestInfo.csv");
        QuestData.Init(new TableLoaderLocalCSV<int, TDataQuestData>(), path, "QuestData.csv");
        QuestDayReward.Init(new TableLoaderLocalCSV<int, TDataQuestDayReward>(), path, "QuestDayReward.csv");
        ShopInfo.Init(new TableLoaderLocalCSV<int, TDataShopInfo>(), path, "ShopInfo.csv");
        ShopProductList.Init(new TableLoaderLocalCSV<int, TDataShopProductList>(), path, "ShopProductList.csv");
        return true;
    }
}

