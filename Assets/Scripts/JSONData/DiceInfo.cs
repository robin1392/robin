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
	public int targetMoveType = 0;
	public string prefabName = "";
	public int spawnMultiply = 0;
	public string iconName = "";
	public string cardName = "";
	public int colorR = 0;
	public int colorG = 0;
	public int colorB = 0;
	public float power = 0.0f;
	public float powerUpgrade = 0.0f;
	public float powerInGameUp = 0.0f;
	public float maxHealth = 0.0f;
	public float maxHpUpgrade = 0.0f;
	public float maxHpInGameUp = 0.0f;
	public float effect = 0.0f;
	public float effectUpgrade = 0.0f;
	public float effectInGameUp = 0.0f;
	public float attackSpeed = 0.0f;
	public float moveSpeed = 0.0f;
	public float range = 0.0f;
	public float searchRange = 0.0f;

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
			info.targetMoveType = (int)JsonDataParse.GetParseData(info.targetMoveType.GetType(), jsonData[i]["targetMoveType"].ToString());
			info.prefabName = (string)JsonDataParse.GetParseData(info.prefabName.GetType(), jsonData[i]["prefabName"].ToString());
			info.spawnMultiply = (int)JsonDataParse.GetParseData(info.spawnMultiply.GetType(), jsonData[i]["spawnMultiply"].ToString());
			info.iconName = (string)JsonDataParse.GetParseData(info.iconName.GetType(), jsonData[i]["iconName"].ToString());
			info.cardName = (string)JsonDataParse.GetParseData(info.cardName.GetType(), jsonData[i]["cardName"].ToString());
			info.colorR = (int)JsonDataParse.GetParseData(info.colorR.GetType(), jsonData[i]["colorR"].ToString());
			info.colorG = (int)JsonDataParse.GetParseData(info.colorG.GetType(), jsonData[i]["colorG"].ToString());
			info.colorB = (int)JsonDataParse.GetParseData(info.colorB.GetType(), jsonData[i]["colorB"].ToString());
			info.power = (float)JsonDataParse.GetParseData(info.power.GetType(), jsonData[i]["power"].ToString());
			info.powerUpgrade = (float)JsonDataParse.GetParseData(info.powerUpgrade.GetType(), jsonData[i]["powerUpgrade"].ToString());
			info.powerInGameUp = (float)JsonDataParse.GetParseData(info.powerInGameUp.GetType(), jsonData[i]["powerInGameUp"].ToString());
			info.maxHealth = (float)JsonDataParse.GetParseData(info.maxHealth.GetType(), jsonData[i]["maxHealth"].ToString());
			info.maxHpUpgrade = (float)JsonDataParse.GetParseData(info.maxHpUpgrade.GetType(), jsonData[i]["maxHpUpgrade"].ToString());
			info.maxHpInGameUp = (float)JsonDataParse.GetParseData(info.maxHpInGameUp.GetType(), jsonData[i]["maxHpInGameUp"].ToString());
			info.effect = (float)JsonDataParse.GetParseData(info.effect.GetType(), jsonData[i]["effect"].ToString());
			info.effectUpgrade = (float)JsonDataParse.GetParseData(info.effectUpgrade.GetType(), jsonData[i]["effectUpgrade"].ToString());
			info.effectInGameUp = (float)JsonDataParse.GetParseData(info.effectInGameUp.GetType(), jsonData[i]["effectInGameUp"].ToString());
			info.attackSpeed = (float)JsonDataParse.GetParseData(info.attackSpeed.GetType(), jsonData[i]["attackSpeed"].ToString());
			info.moveSpeed = (float)JsonDataParse.GetParseData(info.moveSpeed.GetType(), jsonData[i]["moveSpeed"].ToString());
			info.range = (float)JsonDataParse.GetParseData(info.range.GetType(), jsonData[i]["range"].ToString());
			info.searchRange = (float)JsonDataParse.GetParseData(info.searchRange.GetType(), jsonData[i]["searchRange"].ToString());

            
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


