using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RandomWarsProtocol;
using Newtonsoft.Json.Linq;

[Serializable]
public class RewardData
{
    public REWARD_TYPE rewardType;
    public int value;
}

[Serializable]

public class BoxInfoData
{
    public int id;
    public int needKey;
    public BOX_TYPE boxType;
    public RewardData[] rewardRate;
    public RewardData[] rewardKindNum;
    public Dictionary<int, RewardData[]> classRewards;
}


public class BoxInfo
{
    public Dictionary<int, BoxInfoData> dicData;

    public BoxInfoData GetData(int key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        Debug.Log("DATA KEY INVALID : " + "<color=yellow>" + key + "</color>");
        return null;
    }

    public bool IsContainKey(int key)
    {
        return dicData.ContainsKey(key);
    }

    public BoxInfo(string filePath, Action<string> callBack = null)
    {
        string filename = filePath + "BoxInfo.json";
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
        dicData = jObject.ToObject<Dictionary<int, BoxInfoData>>();


        if (callBack != null)
            callBack("BoxInfo load Complete. count: " + dicData.Count);
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}

