using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RandomWarsProtocol;

public class DiceLevelUpNeedInfo
{
    public DICE_GRADE diceGrade;
    public int needDiceCount;
    public int needGold;
    public int addTowerHp;
}


public class DiceLevelUpInfoData
{
    public int level;
    public DiceLevelUpNeedInfo[] levelUpNeedInfo;
}


public class DiceLevelUpInfo
{
    public Dictionary<int, DiceLevelUpInfoData> dicData;

    public DiceLevelUpInfoData GetData(int key)
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

    public DiceLevelUpInfo(string filePath, Action<string> callBack = null)
    {
        string filename = filePath + "DiceLevelUpInfo.json";
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
        dicData = jObject.ToObject<Dictionary<int, DiceLevelUpInfoData>>();


        if (callBack != null)
            callBack("DiceLevelUpInfo load Complete. count: " + dicData.Count);
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}
