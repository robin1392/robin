using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LangDiceDescData
{
	public int id = 0;
	public string DiceNameKO = "";
	public string DiceNameEN = "";
	public string DiceDescKO = "";
	public string DiceDescEN = "";

}

public class LangDiceDesc
{
    public Dictionary<int, LangDiceDescData> dicData = new Dictionary<int, LangDiceDescData>();

    public LangDiceDescData GetData(int key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        Debug.Log( "DATA KEY INVALID : " + "<color=yellow>" + key + "</color>");
        return null;
    }
    
    public bool IsContainKey(int key)
    {
        return dicData.ContainsKey(key);
    }
    
    public LangDiceDesc(string filePath , Action<string> callBack = null)
    {
        JSONObject jsonData = JsonDataParse.OpenFile(filePath + "LangDiceDesc.json");
        if (jsonData == null)
        {
            Debug.LogError("JSON DATA NOT EXIST : "+ filePath + "LangDiceDesc.json" );
            return;
        }

        for (int i = 0; i < jsonData.list.Count; i++)
        {
            LangDiceDescData info = new LangDiceDescData();

			info.id = (int)JsonDataParse.GetParseData(info.id.GetType(), jsonData[i]["id"].ToString());
			info.DiceNameKO = (string)JsonDataParse.GetParseData(info.DiceNameKO.GetType(), jsonData[i]["DiceNameKO"].ToString());
			info.DiceNameEN = (string)JsonDataParse.GetParseData(info.DiceNameEN.GetType(), jsonData[i]["DiceNameEN"].ToString());
			info.DiceDescKO = (string)JsonDataParse.GetParseData(info.DiceDescKO.GetType(), jsonData[i]["DiceDescKO"].ToString());
			info.DiceDescEN = (string)JsonDataParse.GetParseData(info.DiceDescEN.GetType(), jsonData[i]["DiceDescEN"].ToString());

            
            dicData.Add(info.id , info);
        }
        
        if(callBack != null)
            callBack("LangDiceDesc load Complete");
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}


