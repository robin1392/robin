using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RandomWarsProtocol;
using Newtonsoft.Json.Linq;


public class GlobalData
{
    public int value;
}


public class GlobalDataInfo
{
    public Dictionary<GLOBAL_DATA_KEY, GlobalData> dicData;

    public GlobalData GetData(GLOBAL_DATA_KEY key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        Debug.Log("DATA KEY INVALID : " + "<color=yellow>" + key + "</color>");
        return null;
    }

    public bool IsContainKey(GLOBAL_DATA_KEY key)
    {
        return dicData.ContainsKey(key);
    }

    public GlobalDataInfo(string filePath, Action<string> callBack = null)
    {
        string filename = filePath + "GlobalData.json";
        string json = "";
        if (DataPatchManager.Get().eGamePlayMode == E_GamePlayMode.PlayMode_Dev)
        {
            filename = filename.Replace(".json", "");
            var textData = Resources.Load<TextAsset>(filename);
            if (textData != null)
                json = textData.text;
        }
        else
        {
            json = File.ReadAllText(filename);
        }

        JObject jObject = JObject.Parse(json);
        dicData = jObject.ToObject<Dictionary<GLOBAL_DATA_KEY, GlobalData>>();


        if (callBack != null)
            callBack("BoxInfo load Complete. count: " + dicData.Count);
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}