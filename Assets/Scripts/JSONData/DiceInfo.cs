using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiceInfoData
{
	public int id = 0;
	public int grade = 0;
	public int castType = 0;
	public int moveType = 0;
	public string rscName = "";
	public int spawnMultiply = 0;
	public string iconName = "";
	public string cardName = "";
	public float power = 0.0f;
	public float powerByUpgrade = 0.0f;
	public float powerGameUp = 0.0f;
	public float maxHealth = 0.0f;
	public List<int> testArray = new List<int>();

}

public class DiceInfo
{
    public Dictionary<int, DiceInfoData> dicData = new Dictionary<int, DiceInfoData>();

    public DiceInfoData GetData(int key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        return null;
    }
    
    public bool IsContainKey(int key)
    {
        return dicData.ContainsKey(key);
    }
    
    public DiceInfo(string filePath , Action<string> callBack = null)
    {
        JSONObject jsonData = JsonDataParse.OpenFile(filePath + "DiceInfo.json");
        if (jsonData == null)
        {
            Debug.LogError("JSON DATA NOT EXIST : "+ filePath + "DiceInfo.json" );
            return;
        }

        for (int i = 0; i < jsonData.list.Count; i++)
        {
            DiceInfoData info = new DiceInfoData();

			info.id = (int)JsonDataParse.GetParseData(info.id.GetType(), jsonData[i]["id"].ToString());
			info.grade = (int)JsonDataParse.GetParseData(info.grade.GetType(), jsonData[i]["grade"].ToString());
			info.castType = (int)JsonDataParse.GetParseData(info.castType.GetType(), jsonData[i]["castType"].ToString());
			info.moveType = (int)JsonDataParse.GetParseData(info.moveType.GetType(), jsonData[i]["moveType"].ToString());
			info.rscName = (string)JsonDataParse.GetParseData(info.rscName.GetType(), jsonData[i]["rscName"].ToString());
			info.spawnMultiply = (int)JsonDataParse.GetParseData(info.spawnMultiply.GetType(), jsonData[i]["spawnMultiply"].ToString());
			info.iconName = (string)JsonDataParse.GetParseData(info.iconName.GetType(), jsonData[i]["iconName"].ToString());
			info.cardName = (string)JsonDataParse.GetParseData(info.cardName.GetType(), jsonData[i]["cardName"].ToString());
			info.power = (float)JsonDataParse.GetParseData(info.power.GetType(), jsonData[i]["power"].ToString());
			info.powerByUpgrade = (float)JsonDataParse.GetParseData(info.powerByUpgrade.GetType(), jsonData[i]["powerByUpgrade"].ToString());
			info.powerGameUp = (float)JsonDataParse.GetParseData(info.powerGameUp.GetType(), jsonData[i]["powerGameUp"].ToString());
			info.maxHealth = (float)JsonDataParse.GetParseData(info.maxHealth.GetType(), jsonData[i]["maxHealth"].ToString());
			info.testArray = (List<int>)JsonDataParse.GetParseData(info.testArray.GetType(), jsonData[i]["testArray"].ToString());

            
            dicData.Add(info.id , info);
        }
        
        if(callBack != null)
            callBack("DiceInfo load Complete");
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}


