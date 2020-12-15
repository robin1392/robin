using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LangKOData
{
	public string stirngKey = "";
	public string textDesc = "";

}

public class LangKO
{
    public Dictionary<string, LangKOData> dicData = new Dictionary<string, LangKOData>();

    public LangKOData GetData(string key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        Debug.Log( "DATA KEY INVALID : " + "<color=yellow>" + key + "</color>");
        return null;
    }
    
    public bool IsContainKey(string key)
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

			info.stirngKey = (string)JsonDataParse.GetParseData(info.stirngKey.GetType(), jsonData[i]["stirngKey"].ToString());
			info.textDesc = (string)JsonDataParse.GetParseData(info.textDesc.GetType(), jsonData[i]["textDesc"].ToString());

            
            dicData.Add(info.stirngKey , info);
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


