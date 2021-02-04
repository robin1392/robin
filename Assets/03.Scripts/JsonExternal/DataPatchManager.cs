using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using System.IO.Compression;



/// <summary>
/// -- By Nevill
/// 
/// Random Wars 의 쓰임에 맞게 만든 매니저 입니다
///
/// 타 프로젝트에서 참고해서 쓰실시에 해당 프로젝트에 맞게 바꿔서 쓰시면 됩니다
///
/// 테스트로 실행해서 해보시고 적절히 바꿔서 쓰시면됩니다~
/// 
/// </summary>
public class DataPatchManager : MonoBehaviour
{
    #region singleton

    private static DataPatchManager _instance;
    public int instanceID;

    public static DataPatchManager Get()
    {
        return _instance;
    }
    #endregion
    
    
    
    #region variable
    
    public Configuration m_configuration = null; // 설정 파일
    // 게임 서비스 상태 -- 개발 모드
    public E_GameServiceMode eGameServiceMode = E_GameServiceMode.Mode_Dev;
    
    // 게임 플레이 데이터 다운모드 상태
    public E_GamePlayMode eGamePlayMode = E_GamePlayMode.PlayMode_Dev;

    private JsonDataManager _jsonDataManager;

    public JsonDataManager JsonData
    {
        get { return _jsonDataManager; }
    }
    
    private string awsDownUrl = "https://randomwars-configuration.s3.ap-northeast-2.amazonaws.com";    // 나중엔 쓰면 안되는 주소
    //private string awsDownUrl = "";    // cloud front

    private string pathDevLive;
    
    
    private string checkSumFile = "DataCheckList.bin";


    public bool isDataLoadComplete = false;

    public List<string> listDownData = new List<string>();
    public List<FileCheckSum> listCheckSum = new List<FileCheckSum>();
    #endregion
    
    
    
    #region unity base

    private void Awake()
    {
        // 개발용으로 각 씬에 있을수도 있어서...대비용으로
        if (DataPatchManager.Get() != null && this != DataPatchManager.Get())
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        
        DontDestroyOnLoad(this);
        
        if(_instance == null)
            _instance = this as DataPatchManager;
        
        if (instanceID == 0)
            instanceID = GetInstanceID();
        
        
        // configuration
        m_configuration = Configuration.GetConfigFile();//config 파일 불러오기
        eGameServiceMode = m_configuration.eGameServiceMode;
        eGamePlayMode = m_configuration.eGamePlayMode;
        
        
        // data manager add
        this.gameObject.AddComponent<JsonDataManager>();

        InitPatchManager();
        
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDestroy()
    {
        //if (JsonDataManager.Get() != null)
        if(this.transform.GetComponent<JsonDataManager>() != null && isDataLoadComplete == true) 
        {
            this.transform.GetComponent<JsonDataManager>().DestroyJsonData();
            GameObject.Destroy(this.transform.GetComponent<JsonDataManager>());    
        }
    }

    #endregion
    
    #region init destroy

    public void InitPatchManager()
    {
        if (eGameServiceMode == E_GameServiceMode.Mode_Dev)
        {
            pathDevLive = "/DEV/";    
        }
        else
        {
            pathDevLive = "/LIVE/";    
        }


        isDataLoadComplete = false;
    }
    #endregion

    /*public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "serveDown"))
        {
            //StartCoroutine(ExcutePatchDown());
            //print(Application.persistentDataPath);
            //print(Application.dataPath);
            JsonDownLoad();
        }
    }*/

    #region down order
    /*
     * 아마존 링크로 접속
     * 데이터 파일 리스트 ( 체크썸 파일 )을 다운     
     * 다운 리스트 파일중 로컬에 저장된 파일 잇는가?
     * 없으면 다운리스트 추가 --
     * 잇으면 체크썸 생성 후 비교
     * 같으면 패스
     * 틀리면 다운리스트 추가 ---
     */

    public void JsonDownLoad( Action callBack = null)
    {
        if (eGamePlayMode == E_GamePlayMode.PlayMode_Online)
        {
            StartCoroutine(ExcutePatchDown(JsonDataLoad));
        }
        else if (eGamePlayMode == E_GamePlayMode.PlayMode_Dev)
        {
            JsonDataLoad();
        }

        if (callBack != null)
            callBack();
    }




    public IEnumerator ExcutePatchDown(Action callBack = null)
    {
        yield return null;

        string serUrl = awsDownUrl + pathDevLive + checkSumFile;
        //print(serUrl);
        yield return DownServerPatch(serUrl);

        //
        if (listCheckSum.Count == 0)
        {
            Debug.Log("<color=red>" + checkSumFile + "</color>" + " Download Fail !!!!!!!!!! ");
            yield break;
        }

        CompairLocalData();

        if (listDownData.Count > 0)
        {
            int downCount = 0;
            while (true)
            {
                string downPath = awsDownUrl + pathDevLive + listDownData[downCount];
                string localSavePath = Application.persistentDataPath + "/" + listDownData[downCount];

                yield return DownloadAndSave(downPath, localSavePath, downCount);

                downCount++;

                if (downCount >= listDownData.Count)
                {
                    break;
                }
            }
        }
        
        Debug.Log("Download Data Complete !!! ");

        if (callBack != null)
            callBack();
    }

    public IEnumerator DownServerPatch(string patchUrl)
    {
        yield return null;

        UnityWebRequest request = null;
        using (request = UnityWebRequest.Get(patchUrl))
        {
            yield return request.SendWebRequest();

            byte[] bytes = request.downloadHandler.data;

            //File.WriteAllBytes(Application.persistentDataPath + "/" + checkSumFile, bytes);
            listCheckSum = BinaryDeserializeByte<FileCheckSum>(bytes);
        }

    }


    public void CompairLocalData()
    {
        listDownData.Clear();
        
        for (int i = 0; i < listCheckSum.Count; i++)
        {
            string dataFile = Application.persistentDataPath + "/" + listCheckSum[i].FileName;
            if (File.Exists(dataFile) == false)
            {
                listDownData.Add(listCheckSum[i].FileName);
                continue;
            }

            // 파일이 있다면..
            string checkSumLocal = GetMD5(dataFile);
            print("<color=yellow>" + checkSumLocal + "   " + listCheckSum[i].FileMD5 + "    " + listCheckSum[i].FileName+ "</color>");
            if (listCheckSum[i].FileMD5 != checkSumLocal)// checksum 이 틀릴경우만...
            {
                listDownData.Add(listCheckSum[i].FileName);
            }
        }
    }

    public IEnumerator DownloadAndSave(string serverUrl , string localPath , int curCount)
    {
        UnityWebRequest request = null;
        using (request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();
            // 만약에 프로그레스 체크를 한다면...여기서~
            // max count = listDownData.Count
            // cur count = curCount;

            byte[] dataByte = request.downloadHandler.data;
            File.WriteAllBytes(localPath, dataByte);
        }
    }
    #endregion
    
    
    
    #region json data load
    
    
    public void JsonDataLoad()
    {
        string loadPath = "";
        if (eGamePlayMode == E_GamePlayMode.PlayMode_Online)
        {
            loadPath = Application.persistentDataPath + "/";
        }
        else if (eGamePlayMode == E_GamePlayMode.PlayMode_Dev)
        {
            //loadPath = Application.dataPath + "/Resources/JsonData/";
            loadPath = "JsonData/";
        }

        int loadCount = 0;
        int maxLoadCount = JsonDataManager.Get().loadMaxCount;
        JsonDataManager.Get().LoadJsonData(loadPath , (loadClassName) =>
        {
            // 만약에 ui 에 프로그래스 표시를 해야된다면 여기서 한다
            Debug.Log(loadClassName);
            loadCount++;
        });

        isDataLoadComplete = true;
    }

    
    #endregion
    
    
    #region util func
    // 파일의 md5 값을 가져온다
    /*public string GetMD5(string path)
    {
        try
        {
            MD5 md5 = MD5.Create();
            FileStream fs = File.OpenRead(path);
            string md5Value = string.Join("", md5.ComputeHash(fs).ToArray().Select(i => i.ToString("X2")));
            fs.Close();

            return md5Value;
        }
        catch
        {
            return "";
        }
    }*/
    
    public string GetMD5(string path)
    {
        using (FileStream fs = File.OpenRead(path))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] byteChecksum = md5.ComputeHash(fs);
            return BitConverter.ToString(byteChecksum).Replace("-", string.Empty);
        }
    }
    
    
    public void BinarySerialize<T>(List<T> tList, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(stream, tList);
        stream.Close();
    }

    public List<T> BinaryDeserialize<T>(string filename)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filename, FileMode.OpenOrCreate);
        List<T> tList = null;
        try
        {
            tList = (List<T>)formatter.Deserialize(stream);
        }
        catch
        {
            tList = new List<T>();
            Debug.Log("Empty File  New Create");
        }
        stream.Close();
        return tList;
    }
    
    
    public List<T> BinaryDeserializeByte<T>(byte[] strByte)
    {
        Stream mstream = new MemoryStream(strByte);
        BinaryFormatter formatter = new BinaryFormatter();
        List<T> patchinfo = null;
        try
        {
            patchinfo = (List<T>)formatter.Deserialize(mstream);
        }
        catch
        {
            patchinfo = new List<T>();
            Debug.Log("Empty File  New Create");
        }
        
        return patchinfo;
    
    }
    
    #endregion
}
