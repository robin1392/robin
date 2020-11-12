using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RandomWarsProtocol;
using Newtonsoft.Json.Linq;

public class BossInfoData
{
    public int id;
    public int hp;
    public short skillCoolTime;
    public short skillInterval;
    public short atk;
    public short skillAtk;
    public BOSS_TYPE bossType;
    public BOSS_TARGET_TYPE targetType;
    public BOSS_ATK_SPEED atkSpeed;
    public BOSS_MOVE_SPEED moveSpeed;
    public string unitPrefabName;
}

public class BossInfo
{
    public Dictionary<int, BossInfoData> dicData;

    public BossInfoData GetData(int key)
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

    public BossInfo(string filePath, Action<string> callBack = null)
    {
        string filename = filePath + "BossInfo.json";
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
        dicData = jObject.ToObject<Dictionary<int, BossInfoData>>();


        if (callBack != null)
            callBack("BoxInfo load Complete. count: " + dicData.Count);
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}

