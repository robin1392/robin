using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JSONOBJECT;

public class LangKOData
{
	public int id = 0;
	public string textDesc = "";

}

public class LangKO
{
    public Dictionary<int, LangKOData> dicData = new Dictionary<int, LangKOData>();

    public LangKOData GetData(int key)
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
    
    public LangKO(string filePath , Action<string> callBack = null)
    {
        JSONObject jsonData = JsonDataParse.OpenFile(filePath + "LangKO.json");
        if (jsonData == null)
        {
            Debug.LogError("JSON DATA NOT EXIST : "+ filePath + "LangKO.json" );
            return;
        }

        for (int i = 0; i < jsonData.list.Count; i++)
        {
            LangKOData info = new LangKOData();

			info.id = (int)JsonDataParse.GetParseData(info.id.GetType(), jsonData[i]["id"].ToString());
			info.textDesc = (string)JsonDataParse.GetParseData(info.textDesc.GetType(), jsonData[i]["textDesc"].ToString());

            
            dicData.Add(info.id , info);
        }
        
        if(callBack != null)
            callBack("LangKO load Complete");
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}


