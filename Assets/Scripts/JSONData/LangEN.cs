using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LangENData
{
	public int id = 0;
	public string textDesc = "";

}

public class LangEN
{
    public Dictionary<int, LangENData> dicData = new Dictionary<int, LangENData>();

    public LangENData GetData(int key)
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
    
    public LangEN(string filePath , Action<string> callBack = null)
    {
        JSONObject jsonData = JsonDataParse.OpenFile(filePath + "LangEN.json");
        if (jsonData == null)
        {
            Debug.LogError("JSON DATA NOT EXIST : "+ filePath + "LangEN.json" );
            return;
        }

        for (int i = 0; i < jsonData.list.Count; i++)
        {
            LangENData info = new LangENData();

			info.id = (int)JsonDataParse.GetParseData(info.id.GetType(), jsonData[i]["id"].ToString());
			info.textDesc = (string)JsonDataParse.GetParseData(info.textDesc.GetType(), jsonData[i]["textDesc"].ToString());

            
            dicData.Add(info.id , info);
        }
        
        if(callBack != null)
            callBack("LangEN load Complete");
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}


